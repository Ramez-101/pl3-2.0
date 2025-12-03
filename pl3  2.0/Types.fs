module Types

// Basic product information
type Product = {
    Id: int
    Name: string
    Price: decimal
    Category: string
    Stock: int
}

// Cart item with quantity
type CartItem = {
    Product: Product
    Quantity: int
}

// Store state
type Store = {
    Catalog: Map<int, Product>
    Cart: CartItem list
}

// Discount types for price calculator
type DiscountType =
    | NoDiscount
    | PercentageOff of decimal
    | BuyXGetYFree of int * int

// Result type for operations
type StoreResult<'T> =
    | Success of 'T
    | Error of string
