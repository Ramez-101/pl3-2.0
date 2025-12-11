module StoreSimulator.Services.Auth

open StoreSimulator.Core
open StoreSimulator.Services.UserManager

/// Authenticate user (legacy - uses UserManager)
let authenticate (username: string) (password: string) : StoreResult<User> =
    let request: LoginRequest = { Username = username; Password = password }
    match UserManager.login request with
    | Success session -> Success session.User
    | Error msg -> Error msg

/// Create session (legacy)
let createSession (user: User) : UserSession =
    { 
        User = user
        LoginTime = System.DateTime.Now
        SessionId = UserManager.generateSessionId()
    }

/// Check if user is admin
let isAdmin (session: UserSession) : bool =
    UserManager.isAdmin session

/// Check if user is customer
let isCustomer (session: UserSession) : bool =
    UserManager.isCustomer session

/// Quick login for mode selection
let quickAdminLogin () : UserSession =
    UserManager.quickLogin Admin

let quickCustomerLogin () : UserSession =
    UserManager.quickLogin Customer
