# Simple Store Simulator

A feature-rich store simulator application built with F# that supports both console and GUI modes.

## Features

✨ **Dual Interface** - Choose between Console or GUI mode  
🛒 **Shopping Cart** - Add, remove, and update items  
💰 **Smart Pricing** - Automatic discounts and tax calculation  
🔍 **Search & Filter** - Find products easily  
📄 **Receipt Export** - Save receipts as JSON files  

## Quick Start

```bash
# Navigate to the project directory
cd "pl3  2.0"

# Build
dotnet build

# Run
cd "C:\Users\amr emad 2\source\repos\pl3  2.0\pl3  2.0"
dotnet run
```

**Alternative** - Run from parent directory:
```bash
dotnet run --project "pl3  2.0/pl3  2.0.fsproj"
```

Select your preferred mode:
- **Option 1**: GUI Mode (Graphical Interface) 
- **Option 2**: Console Mode (Text Interface)

## Requirements

- .NET 10.0 or higher
- F# 8.0 or higher

## Project Structure

```
pl3  2.0/
├── Types.fs           # Core data models
├── Catalog.fs         # Product catalog management
├── Cart.fs            # Shopping cart operations
├── PriceCalculator.fs # Price and discount calculations
├── SearchFilter.fs    # Product search and filtering
├── FileManager.fs     # Receipt file operations (JSON)
├── UI.fs              # Console user interface
├── SimpleGui.fs       # Avalonia GUI window
├── Program.fs         # Application entry point
└── README.md          # This file
```

## Module Organization

### Core Domain (Business Logic)
- **Types.fs** - Product, Cart, Store data models
- **Catalog.fs** - Product inventory management
- **Cart.fs** - Shopping cart logic with validation
- **PriceCalculator.fs** - Pricing, discounts, tax calculations

### Features
- **SearchFilter.fs** - Search and filter products by various criteria
- **FileManager.fs** - Save and load receipts as JSON files

### User Interface
- **UI.fs** - Console-based text interface
- **SimpleGui.fs** - Avalonia-based graphical interface
- **Program.fs** - Application coordinator and entry point

## Sample Products

The store includes 10 products across categories:
- **Electronics**: Laptop ($999.99), Monitor ($299.99), Keyboard ($75.00), Mouse ($25.50)
- **Accessories**: USB Cable ($9.99), Mouse Pad ($12.99), Phone Stand ($15.99), Desk Lamp ($35.00)

## GUI Mode

**Enhanced Three-Panel Layout:**
- 📋 **Left Panel**: Product list with search and advanced filtering
- 📝 **Middle Panel**: Product details and quantity selector
- 🛒 **Right Panel**: Shopping cart with full management and price breakdown

**Core Features:**
- ✅ Real-time product search by name
- ✅ Advanced search & filter dialog with:
  - Category filtering
  - Price range filtering
  - In-stock only filter
  - Multiple sort options (name, price ascending/descending)
- ✅ Click to select products
- ✅ Add to cart with quantity validation
- ✅ Visual status updates

**Cart Management:**
- ✅ **Remove items** - Select and remove individual items from cart
- ✅ **Update quantity** - Change quantities with dialog (supports 0 to remove)
- ✅ **Clear cart** - Empty entire cart with one click
- ✅ Live price breakdown with automatic discounts
- ✅ One-click checkout with receipt generation

**Receipt History:**
- ✅ **View all saved receipts** - Browse complete purchase history
- ✅ **Receipt details viewer** - See full order information including:
  - Transaction date and time
  - Item list with quantities and prices
  - Price breakdown (subtotal, discount, tax, total)
- ✅ Easy-to-use dialog interface

**Status Bar:**
- Real-time feedback for all operations
- Success/error messages with visual indicators (✓/✗)
- Helpful hints and confirmations

All console mode features are now available in the GUI with an intuitive point-and-click interface!

## Console Mode

**Main Menu:**
1. View All Products
2. Search/Filter Products
3. Add to Cart
4. View Cart
5. Remove from Cart
6. Update Quantity
7. Checkout
8. **View Receipt History** 🆕
9. Exit

**Search Options:**
- Search by name
- Filter by category
- Filter by price range
- View in-stock only
- Sort by price or name

**Receipt History Features:**
- Browse all saved receipts
- View detailed receipt information
- See complete order history
- Review past transactions with full pricing breakdown

## Key Features

### Automatic Discounts
- 💵 5% off orders over $200
- 💰 10% off orders over $500

### Price Breakdown
- Subtotal - Sum of all items
- Discount - Automatically applied
- Tax (8.5%) - Calculated on discounted total
- **Total** - Final amount

### Receipt Export (JSON Format)
- **Saved as**: `receipt_YYYYMMDD_HHMMSS.json`
- **Format**: Pretty-printed JSON with indentation
- **Contains**: 
  - Transaction date and timestamp
  - Complete item list with product details
  - Price breakdown (subtotal, discount, tax, total)
  - Full order history

**Sample Receipt Structure:**
```json
{
  "Date": "2025-12-03T19:35:56.034028+02:00",
  "Items": [
    {
      "Product": {
        "Id": 6,
        "Name": "Headphones",
        "Price": 49.99,
        "Category": "Electronics",
        "Stock": 25
      },
      "Quantity": 3
    }
  ],
  "Subtotal": 1045.05,
  "Discount": 104.505,
  "Tax": 79.946325,
  "Total": 1020.491325
}
```

## Technologies

- **F# 8.0+** - Functional programming language
- **.NET 10.0** - Runtime framework
- **Avalonia UI 11.0** - Cross-platform GUI framework
- **System.Text.Json** - JSON serialization
- **FSharp.SystemTextJson** - F# JSON integration

## F# Concepts Demonstrated

- ✅ **Immutable data structures** - All data is immutable
- ✅ **Pattern matching** - Extensive use throughout
- ✅ **Discriminated unions** - For discount types and results
- ✅ **Pure functions** - Side-effect free calculations
- ✅ **Function composition** - Pipe operator for data flow
- ✅ **Result types** - Proper error handling
- ✅ **Module organization** - Clean separation of concerns
- ✅ **Records** - Immutable data containers
- ✅ **OOP Integration** - Avalonia GUI with F# functional core

## Code Examples

### Immutable Cart Operations
```fsharp
let addToCart cart product quantity = 
    // Returns new cart, doesn't modify original
    Success (newItem :: cart)
```

### Pattern Matching
```fsharp
match Cart.addToCart cart product quantity with
| Success newCart -> // Handle success
| Error msg -> // Handle error
```

### Function Composition
```fsharp
products
|> filterByName searchTerm
|> filterByCategory category
|> sortByPrice
```

### JSON Serialization (FileManager.fs)
```fsharp
// Save receipt with pretty-printed JSON
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
```

### Loading JSON Receipts Programmatically
```fsharp
// Load a specific receipt
let result = FileManager.loadReceipt "receipt_20251203_193556.json"

match result with
| Success receipt ->
    printfn "Receipt loaded successfully!"
    printfn "Date: %s" (receipt.Date.ToString())
    printfn "Total: $%.2f" receipt.Total
    printfn "Items: %d" receipt.Items.Length
| Error msg ->
    printfn "Error loading receipt: %s" msg

// List all receipt files
let allReceipts = 
    Directory.GetFiles(".", "receipt_*.json")
    |> Array.map (fun file -> FileManager.loadReceipt file)
    |> Array.choose (fun result -> 
        match result with
        | Success r -> Some r
        | Error _ -> None)
```

## Extension Ideas

- 👤 **User Accounts** - Login system with order history
- 📊 **Inventory Management** - Admin panel for stock control
- 🎨 **Themes** - Multiple UI themes and color schemes
- 🧪 **Unit Tests** - ✅ **COMPLETED! 84 tests with ~93% coverage**
- 🌐 **Web Version** - Convert to Fable/Elmish
- 📱 **Mobile App** - Use Fabulous for iOS/Android
- 🗄️ **Database** - Replace JSON with SQL/NoSQL database
- 🖼️ **Product Images** - Add image support in GUI
- 📈 **Analytics** - Sales reporting and charts
- 🔔 **Notifications** - Low stock alerts
- 📧 **Email Receipts** - Send receipts via email
- 🧾 **PDF Export** - Convert JSON receipts to PDF format

## 🧪 Testing

### Comprehensive Test Suite ✅

The project includes a complete test suite with **84 passing tests** covering all core modules:

| Module | Tests | Coverage | Status |
|--------|-------|----------|--------|
| Cart | 19 | ~95% | ✅ Passing |
| PriceCalculator | 18 | ~95% | ✅ Passing |
| Catalog | 20 | ~90% | ✅ Passing |
| SearchFilter | 27 | ~95% | ✅ Passing |
| **Total** | **84** | **~93%** | ✅ **All Passing** |

### Running Tests

```bash
cd "pl3  2.0/Tests"
dotnet test
```

**Expected Output:**
```
Test summary: total: 84, failed: 0, succeeded: 84, skipped: 0
```

### What's Tested

- ✅ **Cart Operations** - Add, remove, update with stock validation
- ✅ **Price Calculations** - Subtotals, discounts (5%/10%), tax (8.5%)
- ✅ **Catalog Management** - Product lookup, stock updates, immutability
- ✅ **Search & Filter** - Name search, category/price filtering, sorting
- ✅ **Immutability** - All operations preserve original data
- ✅ **Error Handling** - Invalid inputs handled correctly
- ✅ **Boundary Conditions** - Edge cases covered

### Test Documentation

- 📖 **Tests/README.md** - Comprehensive test documentation
- 🚀 **Tests/QUICKSTART.md** - Quick start guide
- 📊 **Tests/TEST_SUMMARY.md** - Complete test summary

See [Tests/README.md](Tests/README.md) for detailed testing documentation.
