module FileManager

open Types
open System.IO
open System.Text.Json
open System.Text.Json.Serialization

// File Save/Load Developer - JSON output

// Receipt data structure for saving
type Receipt = {
    Date: System.DateTime
    Items: CartItem list
    Subtotal: decimal
    Discount: decimal
    Tax: decimal
    Total: decimal
}

// Configure JSON serialization options
let private jsonOptions = 
    let options = JsonSerializerOptions()
    options.WriteIndented <- true
    options.Converters.Add(JsonFSharpConverter())
    options

// Save receipt to JSON file
let saveReceipt (receipt: Receipt) (filePath: string) : StoreResult<string> =
    try
        let json = JsonSerializer.Serialize(receipt, jsonOptions)
        File.WriteAllText(filePath, json)
        Success $"Receipt saved to {filePath}"
    with
    | ex -> Error $"Failed to save receipt: {ex.Message}"

// Load receipt from JSON file
let loadReceipt (filePath: string) : StoreResult<Receipt> =
    try
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            let receipt = JsonSerializer.Deserialize<Receipt>(json, jsonOptions)
            Success receipt
        else
            Error "Receipt file not found"
    with
    | ex -> Error $"Failed to load receipt: {ex.Message}"

// Generate receipt from cart and price breakdown
let createReceipt (cart: CartItem list) (breakdown: PriceCalculator.PriceBreakdown) : Receipt =
    {
        Date = System.DateTime.Now
        Items = cart
        Subtotal = breakdown.Subtotal
        Discount = breakdown.Discount
        Tax = breakdown.Tax
        Total = breakdown.Total
    }

// Save cart summary (simpler version without price details)
let saveCartSummary (cart: CartItem list) (filePath: string) : StoreResult<string> =
    try
        let json = JsonSerializer.Serialize(cart, jsonOptions)
        File.WriteAllText(filePath, json)
        Success $"Cart saved to {filePath}"
    with
    | ex -> Error $"Failed to save cart: {ex.Message}"

// Generate default filename with timestamp
let generateFileName (prefix: string) : string =
    let timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss")
    $"{prefix}_{timestamp}.json"
