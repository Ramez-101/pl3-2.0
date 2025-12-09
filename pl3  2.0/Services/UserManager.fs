module StoreSimulator.Services.UserManager

open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open System.Security.Cryptography
open System.Text
open StoreSimulator.Core

/// User storage file
let private usersFilePath = "users.json"

/// JSON options
let private jsonOptions = 
    let options = JsonSerializerOptions()
    options.WriteIndented <- true
    options.Converters.Add(JsonFSharpConverter())
    options

/// Simple password hashing (use proper hashing in production)
let hashPassword (password: string) : string =
    use sha256 = SHA256.Create()
    let bytes = Encoding.UTF8.GetBytes(password)
    let hash = sha256.ComputeHash(bytes)
    Convert.ToBase64String(hash)

/// Verify password
let verifyPassword (password: string) (hash: string) : bool =
    hashPassword password = hash

/// Generate session ID
let generateSessionId () : string =
    Guid.NewGuid().ToString("N")

/// Default admin and customer accounts
let private defaultUsers = [
    { 
        Id = 1
        Username = "admin"
        PasswordHash = hashPassword "admin123"
        Email = "admin@store.com"
        FullName = "Store Administrator"
        Mode = Admin
        CreatedAt = DateTime.Now
    }
    { 
        Id = 2
        Username = "customer"
        PasswordHash = hashPassword "customer123"
        Email = "customer@email.com"
        FullName = "Demo Customer"
        Mode = Customer
        CreatedAt = DateTime.Now
    }
]

/// Mutable user storage
let mutable private users: User list = defaultUsers
let mutable private nextUserId = 3

/// Load users from file
let loadUsers () : StoreResult<User list> =
    try
        if File.Exists(usersFilePath) then
            let json = File.ReadAllText(usersFilePath)
            let loadedUsers = JsonSerializer.Deserialize<User list>(json, jsonOptions)
            users <- loadedUsers
            if users.Length > 0 then
                nextUserId <- (users |> List.map (fun u -> u.Id) |> List.max) + 1
            Success users
        else
            users <- defaultUsers
            Success users
    with
    | ex -> Error (sprintf "Failed to load users: %s" ex.Message)

/// Save users to file
let saveUsers () : StoreResult<string> =
    try
        let json = JsonSerializer.Serialize(users, jsonOptions)
        File.WriteAllText(usersFilePath, json)
        Success "Users saved successfully"
    with
    | ex -> Error (sprintf "Failed to save users: %s" ex.Message)

/// Get all users (admin only)
let getAllUsers () : User list = users

/// Find user by username
let findUserByUsername (username: string) : User option =
    users |> List.tryFind (fun u -> 
        u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))

/// Find user by email
let findUserByEmail (email: string) : User option =
    users |> List.tryFind (fun u -> 
        u.Email.Equals(email, StringComparison.OrdinalIgnoreCase))

/// Register new user
let register (request: RegistrationRequest) : StoreResult<User> =
    if String.IsNullOrWhiteSpace(request.Username) then
        Error "Username is required"
    elif request.Username.Length < 3 then
        Error "Username must be at least 3 characters"
    elif findUserByUsername request.Username |> Option.isSome then
        Error "Username already exists"
    elif String.IsNullOrWhiteSpace(request.Password) then
        Error "Password is required"
    elif request.Password.Length < 6 then
        Error "Password must be at least 6 characters"
    elif String.IsNullOrWhiteSpace(request.Email) then
        Error "Email is required"
    elif not (request.Email.Contains("@")) then
        Error "Invalid email format"
    elif findUserByEmail request.Email |> Option.isSome then
        Error "Email already registered"
    elif String.IsNullOrWhiteSpace(request.FullName) then
        Error "Full name is required"
    else
        let newUser = {
            Id = nextUserId
            Username = request.Username.Trim()
            PasswordHash = hashPassword request.Password
            Email = request.Email.Trim().ToLower()
            FullName = request.FullName.Trim()
            Mode = Customer
            CreatedAt = DateTime.Now
        }
        nextUserId <- nextUserId + 1
        users <- newUser :: users
        match saveUsers() with
        | Success _ -> Success newUser
        | Error msg -> Error msg

/// Login user
let login (request: LoginRequest) : StoreResult<UserSession> =
    if String.IsNullOrWhiteSpace(request.Username) then
        Error "Username is required"
    elif String.IsNullOrWhiteSpace(request.Password) then
        Error "Password is required"
    else
        match findUserByUsername request.Username with
        | Some user ->
            if verifyPassword request.Password user.PasswordHash then
                let session = {
                    User = user
                    LoginTime = DateTime.Now
                    SessionId = generateSessionId()
                }
                Success session
            else
                Error "Invalid password"
        | None ->
            Error "User not found"

/// Quick login (for mode selection without full auth)
let quickLogin (mode: UserMode) : UserSession =
    let user = 
        match mode with
        | Admin -> 
            { 
                Id = 0
                Username = "admin"
                PasswordHash = ""
                Email = "admin@store.com"
                FullName = "Store Admin"
                Mode = Admin
                CreatedAt = DateTime.Now
            }
        | Customer ->
            { 
                Id = 0
                Username = "guest"
                PasswordHash = ""
                Email = "guest@store.com"
                FullName = "Guest Customer"
                Mode = Customer
                CreatedAt = DateTime.Now
            }
    {
        User = user
        LoginTime = DateTime.Now
        SessionId = generateSessionId()
    }

/// Check if user is admin
let isAdmin (session: UserSession) : bool =
    session.User.Mode = Admin

/// Check if user is customer
let isCustomer (session: UserSession) : bool =
    session.User.Mode = Customer

/// Change password
let changePassword (userId: int) (oldPassword: string) (newPassword: string) : StoreResult<string> =
    match users |> List.tryFind (fun u -> u.Id = userId) with
    | Some user ->
        if not (verifyPassword oldPassword user.PasswordHash) then
            Error "Current password is incorrect"
        elif newPassword.Length < 6 then
            Error "New password must be at least 6 characters"
        else
            let updatedUser = { user with PasswordHash = hashPassword newPassword }
            users <- users |> List.map (fun u -> if u.Id = userId then updatedUser else u)
            match saveUsers() with
            | Success _ -> Success "Password changed successfully"
            | Error msg -> Error msg
    | None ->
        Error "User not found"

/// Initialize - load users on module load
let initialize () =
    loadUsers() |> ignore
