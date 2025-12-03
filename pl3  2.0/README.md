# Simple Store Simulator

A feature-rich store simulator application built with F# that supports both console and GUI modes.

## Features

? **Dual Interface** - Choose between Console or GUI mode  
?? **Shopping Cart** - Add, remove, and update items  
?? **Smart Pricing** - Automatic discounts and tax calculation  
?? **Search & Filter** - Find products easily  
?? **Receipt Export** - Save receipts as text files  

## Quick Start

```bash
# Build
dotnet build

# Run
dotnet run
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
??? Types.fs           # Core data models
??? Catalog.fs         # Product catalog management
??? Cart.fs            # Shopping cart operations
??? PriceCalculator.fs # Price and discount calculations
??? SearchFilter.fs    # Product search and filtering
??? FileManager.fs     # Receipt file operations
??? UI.fs              # Console user interface
??? SimpleGui.fs       # Avalonia GUI window
??? Program.fs         # Application entry point
??? README.md          # This file
```

## Module Organization

### Core Domain (Business Logic)
- **Types.fs** - Product, Cart, Store data models
- **Catalog.fs** - Product inventory management
- **Cart.fs** - Shopping cart logic with validation
- **PriceCalculator.fs** - Pricing, discounts, tax calculations

### Features
- **SearchFilter.fs** - Search and filter products by various criteria
- **FileManager.fs** - Save and load receipts as text files

### User Interface
- **UI.fs** - Console-based text interface
- **SimpleGui.fs** - Avalonia-based graphical interface
- **Program.fs** - Application coordinator and entry point

## Sample Products

The store includes 10 products across categories:
- **Electronics**: Laptop ($999.99), Monitor ($299.99), Keyboard ($75.00), Mouse ($25.50)
- **Accessories**: USB Cable ($9.99), Mouse Pad ($12.99), Phone Stand ($15.99), Desk Lamp ($35.00)

## GUI Mode

**Three-Panel Layout:**
- ?? **Left Panel**: Product list with search functionality
- ?? **Middle Panel**: Product details and quantity selector
- ?? **Right Panel**: Shopping cart with live price breakdown

**Features:**
- Real-time search filtering
- Click to select products
- Add to cart with quantity validation
- Visual status updates
- One-click checkout

## Console Mode

**Main Menu:**
1. View All Products
2. Search/Filter Products
3. Add to Cart
4. View Cart
5. Remove from Cart
6. Update Quantity
7. Checkout
8. Exit

**Search Options:**
- Search by name
- Filter by category
- Filter by price range
- View in-stock only
- Sort by price or name

## Key Features

### Automatic Discounts
- ?? 5% off orders over $200
- ?? 10% off orders over $500

### Price Breakdown
- Subtotal - Sum of all items
- Discount - Automatically applied
- Tax (8.5%) - Calculated on discounted total
- **Total** - Final amount

### Receipt Export
- Saved as `receipt_YYYYMMDD_HHMMSS.txt`
- Contains complete order details
- Includes price breakdown
- Timestamped for records

## Technologies

- **F# 8.0+** - Functional programming language
- **.NET 10.0** - Runtime framework
- **Avalonia UI 11.0** - Cross-platform GUI framework
- **System.Text.Json** - JSON serialization
- **FSharp.SystemTextJson** - F# JSON integration

## F# Concepts Demonstrated

- ? **Immutable data structures** - All data is immutable
- ? **Pattern matching** - Extensive use throughout
- ? **Discriminated unions** - For discount types and results
- ? **Pure functions** - Side-effect free calculations
- ? **Function composition** - Pipe operator for data flow
- ? **Result types** - Proper error handling
- ? **Module organization** - Clean separation of concerns
- ? **Records** - Immutable data containers
- ? **OOP Integration** - Avalonia GUI with F# functional core

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

## Extension Ideas

- ?? **User Accounts** - Login system with order history
- ?? **Inventory Management** - Admin panel for stock control
- ?? **Themes** - Multiple UI themes and color schemes
- ?? **Unit Tests** - Add tests with xUnit or Expecto
- ?? **Web Version** - Convert to Fable/Elmish
- ?? **Mobile App** - Use Fabulous for iOS/Android
- ??? **Database** - Replace JSON with SQL/NoSQL database
- ??? **Product Images** - Add image support in GUI
- ?? **Analytics** - Sales reporting and charts
- ?? **Notifications** - Low stock alerts

## Team Development

Each module can be developed independently by different team members:

| Module | Responsibility | Key Skills |
|--------|---------------|-----------|
| **Types.fs** | Define data models | Record types, unions |
| **Catalog.fs** | Product management | Map operations |
| **Cart.fs** | Shopping cart | List operations, validation |
| **PriceCalculator.fs** | Pricing logic | Pure functions, math |
| **SearchFilter.fs** | Search features | List filtering, sorting |
| **FileManager.fs** | File I/O | JSON, file operations |
| **UI.fs** | Console interface | Console formatting, input |
| **SimpleGui.fs** | GUI interface | Avalonia, event handling |
| **Program.fs** | Coordination | State management, flow |

## Building and Running

### Build the project
```bash
dotnet build
```

### Run the application
```bash
dotnet run
```

### Clean build artifacts
```bash
dotnet clean
```

## Troubleshooting

**Issue**: GUI window doesn't appear  
**Solution**: Make sure you selected option 1 and wait a few seconds for Avalonia to initialize

**Issue**: Build errors  
**Solution**: Ensure .NET 10.0 SDK is installed: `dotnet --version`

**Issue**: Receipt files not saving  
**Solution**: Check write permissions in the application directory

## Contributing

Students are encouraged to:
- ?? Report bugs and issues
- ? Add new features
- ?? Improve documentation
- ?? Write unit tests
- ?? Enhance the UI design
- ?? Refactor code for better readability

## Learning Path

### Beginners
1. Study `Types.fs` to understand the data model
2. Explore `PriceCalculator.fs` for pure functions
3. Try the GUI mode first for intuitive interaction
4. Read through `Cart.fs` for list operations

### Intermediate
1. Understand pattern matching in `Program.fs`
2. Study error handling with `StoreResult`
3. Explore function composition in `SearchFilter.fs`
4. Learn event handling in `SimpleGui.fs`

### Advanced
1. Implement new features from extension ideas
2. Add unit tests for all modules
3. Optimize performance for large catalogs
4. Integrate with external APIs

## License

Educational project - Free to use and modify

---

**Made with F# by Students** ??
