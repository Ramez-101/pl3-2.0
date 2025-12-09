module StoreSimulator.Data.FileManager

open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open StoreSimulator.Core
open StoreSimulator.Services.PriceCalculator

/// Receipt data structure for saving (with customer info)
type Receipt = {
    OrderId: string
    CustomerId: int
    CustomerName: string
    Date: DateTime
    Items: CartItem list
    Subtotal: decimal
    Discount: decimal
    Tax: decimal
    Total: decimal
    Status: string
}

/// Configure JSON serialization options
let private jsonOptions = 
    let options = JsonSerializerOptions()
    options.WriteIndented <- true
    options.Converters.Add(JsonFSharpConverter())
    options

/// Generate unique order ID
let generateOrderId () : string =
    let timestamp = DateTime.Now.ToString("yyyyMMddHHmmss")
    let random = Random().Next(1000, 9999)
    sprintf "ORD-%s-%d" timestamp random

/// Save receipt to JSON file
let saveReceipt (receipt: Receipt) (filePath: string) : StoreResult<string> =
    try
        let json = JsonSerializer.Serialize(receipt, jsonOptions)
        File.WriteAllText(filePath, json)
        Success (sprintf "Receipt saved to %s" filePath)
    with
    | ex -> Error (sprintf "Failed to save receipt: %s" ex.Message)

/// Load receipt from JSON file
let loadReceipt (filePath: string) : StoreResult<Receipt> =
    try
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            let receipt = JsonSerializer.Deserialize<Receipt>(json, jsonOptions)
            Success receipt
        else
            Error "Receipt file not found"
    with
    | ex -> Error (sprintf "Failed to load receipt: %s" ex.Message)

/// Generate receipt from cart and price breakdown (with user info)
let createReceipt (cart: CartItem list) (breakdown: PriceBreakdown) : Receipt =
    {
        OrderId = generateOrderId()
        CustomerId = 0
        CustomerName = "Guest"
        Date = DateTime.Now
        Items = cart
        Subtotal = breakdown.Subtotal
        Discount = breakdown.Discount
        Tax = breakdown.Tax
        Total = breakdown.Total
        Status = "Completed"
    }

/// Create receipt with user session
let createReceiptWithUser (cart: CartItem list) (breakdown: PriceBreakdown) (user: User) : Receipt =
    {
        OrderId = generateOrderId()
        CustomerId = user.Id
        CustomerName = user.FullName
        Date = DateTime.Now
        Items = cart
        Subtotal = breakdown.Subtotal
        Discount = breakdown.Discount
        Tax = breakdown.Tax
        Total = breakdown.Total
        Status = "Completed"
    }

/// Get all receipts/orders
let getAllReceipts () : Receipt list =
    try
        let files = Directory.GetFiles(".", "receipt_*.json") |> Array.sort |> Array.rev
        files 
        |> Array.choose (fun file ->
            match loadReceipt file with
            | Success receipt -> Some receipt
            | Error _ -> None
        )
        |> Array.toList
    with
    | _ -> []

/// Get receipts by customer ID
let getReceiptsByCustomer (customerId: int) : Receipt list =
    getAllReceipts() |> List.filter (fun r -> r.CustomerId = customerId)

/// Save cart summary
let saveCartSummary (cart: CartItem list) (filePath: string) : StoreResult<string> =
    try
        let json = JsonSerializer.Serialize(cart, jsonOptions)
        File.WriteAllText(filePath, json)
        Success (sprintf "Cart saved to %s" filePath)
    with
    | ex -> Error (sprintf "Failed to save cart: %s" ex.Message)

/// Generate default filename with timestamp
let generateFileName (prefix: string) : string =
    let timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss")
    sprintf "%s_%s.json" prefix timestamp

/// Save product catalog to file (for admin)
let saveCatalog (catalog: Map<int, Product>) (filePath: string) : StoreResult<string> =
    try
        let products = catalog |> Map.toList |> List.map snd
        let json = JsonSerializer.Serialize(products, jsonOptions)
        File.WriteAllText(filePath, json)
        Success (sprintf "Catalog saved to %s" filePath)
    with
    | ex -> Error (sprintf "Failed to save catalog: %s" ex.Message)

/// Load product catalog from file
let loadCatalog (filePath: string) : StoreResult<Product list> =
    try
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            let products = JsonSerializer.Deserialize<Product list>(json, jsonOptions)
            Success products
        else
            Error "Catalog file not found"
    with
    | ex -> Error (sprintf "Failed to load catalog: %s" ex.Message)
