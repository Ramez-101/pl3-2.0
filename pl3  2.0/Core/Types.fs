namespace StoreSimulator.Core

open System

/// User access modes
type UserMode =
    | Admin
    | Customer

/// User information with enhanced fields
type User = {
    Id: int
    Username: string
    PasswordHash: string
    Email: string
    FullName: string
    Mode: UserMode
    CreatedAt: DateTime
}

/// User session
type UserSession = {
    User: User
    LoginTime: DateTime
    SessionId: string
}

/// Registration request
type RegistrationRequest = {
    Username: string
    Password: string
    Email: string
    FullName: string
}

/// Login request
type LoginRequest = {
    Username: string
    Password: string
}

/// Enhanced product information with more details
type Product = {
    Id: int
    Name: string
    Description: string
    Price: decimal
    Category: string
    Stock: int
    Brand: string
    Rating: decimal
    ImageUrl: string
    IsAvailable: bool
    Tags: string list
}

/// Cart item with quantity
type CartItem = {
    Product: Product
    Quantity: int
}

/// Store state with mutable catalog for stock updates
type Store = {
    Catalog: Map<int, Product>
    Cart: CartItem list
}

/// Order for purchase history
type Order = {
    OrderId: string
    UserId: int
    Items: CartItem list
    Subtotal: decimal
    Discount: decimal
    Tax: decimal
    Total: decimal
    OrderDate: DateTime
    Status: string
}

/// Discount types for price calculator
type DiscountType =
    | NoDiscount
    | PercentageOff of decimal
    | BuyXGetYFree of int * int
    | FixedAmount of decimal

/// Result type for operations
type StoreResult<'T> =
    | Success of 'T
    | Error of string
