# Store Simulator - F# Edition

A professional shopping store simulator built with F# featuring both console and GUI interfaces, demonstrating clean architecture principles and functional programming concepts.

## ✨ Features

- 🖥️ **Dual Interface** - Choose between modern GUI or classic console mode
- 👤 **User Authentication** - Login system with Admin and Customer roles
- 🛒 **Smart Shopping Cart** - Add, remove, and update items with validation
- 💰 **Intelligent Pricing** - Automatic discounts and real-time tax calculation
- 🔍 **Advanced Search** - Filter by category, brand, price range, and more
- 📄 **Receipt Management** - Save and view order history as JSON
- 📊 **Admin Panel** - Product management, inventory alerts, and order tracking

---

## 🚀 Quick Start

### Prerequisites
- .NET 10.0 SDK or higher
- Visual Studio 2022 or VS Code with F# extension

### Run the Application

```bash
cd "pl3  2.0"
dotnet run
```

**Select your mode:**
1. **GUI Mode** - Modern Avalonia-based graphical interface
2. **Console Mode** - Traditional terminal-based interface

### Quick Login Credentials

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Administrator |
| customer | customer123 | Customer |

---

## 📁 Project Structure

```
pl3  2.0/
├── Core/                         # 🎯 Domain Layer
│   └── Types.fs                  # All domain types and models
│
├── Services/                     # 💼 Business Logic Layer
│   ├── UserManager.fs            # User authentication & sessions
│   ├── Auth.fs                   # Authentication helpers
│   ├── PriceCalculator.fs        # Pricing & discount logic
│   └── SearchFilter.fs           # Search & filtering algorithms
│
├── Data/                         # 💾 Data Access Layer
│   ├── Catalog.fs                # Product catalog management
│   ├── Cart.fs                   # Shopping cart operations
│   └── FileManager.fs            # File I/O & persistence
│
├── UI/                           # 🎨 Presentation Layer
│   ├── ConsoleUI.fs              # Terminal interface
│   └── StoreGui.fs               # Avalonia GUI interface
│
├── Program.fs                    # Entry point
├── pl3  2.0.fsproj               # Project configuration
└── README.md                     # This file
```

### Architecture Layers

| Layer | Responsibility | Dependencies |
|-------|---------------|--------------|
| **Core** | Domain types & models | None |
| **Services** | Business logic | Core |
| **Data** | Data access & persistence | Core, Services |
| **UI** | User interfaces | All layers |

---

## 🖥️ GUI Mode Features

### Three-Panel Layout
- **📋 Left**: Product catalog with real-time search
- **📝 Center**: Product details with quantity selector
- **🛒 Right**: Shopping cart with price breakdown

### Key Features
- ✅ **Real-time Search** - Instant product filtering
- ✅ **Advanced Filters** - Category, brand, price range
- ✅ **Stock Indicators** - Visual stock status (OK/LOW/OUT)
- ✅ **Cart Management** - Add, remove, update quantities
- ✅ **Automatic Discounts** - 5% over $200, 10% over $500
- ✅ **Receipt History** - View all past orders
- ✅ **Status Updates** - Real-time operation feedback

### Admin Panel Features
- ➕ **Add New Product** - Easy product creation dialog
- 📊 **View All Orders** - Complete customer order history
- ⚠️ **Low Stock Alerts** - Inventory monitoring
- 💰 **Revenue Tracking** - Total sales dashboard

---

## 💻 Console Mode Features

### Main Menu
1. View All Products
2. Search/Filter Products
3. Add Product to Cart
4. View Cart
5. Remove from Cart
6. Update Cart Quantity
7. Checkout
8. View Receipt History
9. Exit

### Search & Filter Options
- 🔍 Search by product name
- 📂 Filter by category
- 💵 Filter by price range
- ✅ Show in-stock only
- ⬆️⬇️ Sort by price or name

---

## 💰 Pricing System

### Automatic Discounts
| Order Total | Discount |
|-------------|----------|
| $200+ | 5% off |
| $500+ | 10% off |

### Price Breakdown
```
Subtotal:  $299.97
Discount:  -$0.00
Tax (8.5%): $25.50
─────────────────────
Total:     $325.47
```

---

## 📄 Receipt Management

### JSON Receipt Format
Receipts are saved as `receipt_YYYYMMDD_HHMMSS.json`:

```json
{
  "OrderId": "ORD-20251203194530-1234",
  "CustomerId": 2,
  "CustomerName": "Demo Customer",
  "Date": "2025-12-03T19:45:30.123456+02:00",
  "Items": [
    {
      "Product": {
        "Id": 1,
        "Name": "MacBook Pro 14 inch",
        "Price": 1999.99,
        "Category": "Laptops",
        "Stock": 9
      },
      "Quantity": 1
    }
  ],
  "Subtotal": 1999.99,
  "Discount": 199.999,
  "Tax": 153.00,
  "Total": 1952.99,
  "Status": "Completed"
}
```

### Loading Receipts Programmatically
```fsharp
// Load specific receipt
match FileManager.loadReceipt "receipt_20251203.json" with
| Success receipt -> 
    printfn "Total: $%.2f" receipt.Total
| Error msg -> 
    printfn "Error: %s" msg

// Get all receipts
let allOrders = FileManager.getAllReceipts()
let totalRevenue = allOrders |> List.sumBy (fun o -> o.Total)
```

---

## 🛠️ Technologies

- **F# 8.0+** - Functional-first programming language
- **.NET 10.0** - Cross-platform runtime
- **Avalonia UI 11.0** - Modern cross-platform GUI framework
- **System.Text.Json** - High-performance JSON serialization
- **FSharp.SystemTextJson** - F# type support for JSON

---

## 🎓 F# Concepts Demonstrated

### Immutable Data
```fsharp
// Operations return new data, never mutate
let addToCart cart product quantity =
    Success (newItem :: cart)  // New list, original unchanged
```

### Pattern Matching
```fsharp
match Cart.addToCart cart product quantity with
| Success newCart -> updateDisplay newCart
| Error msg -> showError msg
```

### Discriminated Unions
```fsharp
type DiscountType =
    | NoDiscount
    | PercentageOff of decimal
    | BuyXGetYFree of int * int
```

### Result Type for Error Handling
```fsharp
type StoreResult<'T> =
    | Success of 'T
    | Error of string
```

### Function Composition
```fsharp
products
|> SearchFilter.filterByName "laptop"
|> SearchFilter.filterByCategory "Electronics"
|> SearchFilter.sortByPrice
```

### Module Organization
- Clean separation of concerns
- Pure functions (no side effects)
- Immutable data structures
- Type-safe domain modeling

---

## 📦 Sample Products

The store includes **15 products** across multiple categories:

| Category | Products | Price Range |
|----------|----------|-------------|
| Laptops | MacBook Pro, iPad Pro | $1,099 - $1,999 |
| Monitors | Dell UltraSharp 4K | $549 |
| Audio | Sony WH-1000XM5, AirPods Pro | $249 - $349 |
| Accessories | Keyboards, Mice, Hubs, Stands | $29 - $149 |
| Storage | Samsung 980 PRO SSD | $129 |

---

## 🔧 Development

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Clean
```bash
dotnet clean
```

---

## 🚀 Extension Ideas

- [ ] 🗄️ **Database Integration** - Replace JSON with SQL/PostgreSQL
- [ ] 🔐 **Enhanced Security** - JWT tokens, bcrypt hashing
- [ ] 📧 **Email Notifications** - Send receipts via email
- [ ] 📊 **Analytics Dashboard** - Sales charts and reports
- [ ] 🌐 **Web API** - REST API with Giraffe/Saturn
- [ ] 📱 **Mobile App** - Xamarin.Forms or MAUI
- [ ] 🧪 **Unit Tests** - xUnit test suite
- [ ] 🐳 **Docker Support** - Containerized deployment
- [ ] 📈 **Inventory Forecasting** - ML.NET predictions
- [ ] 🎨 **Theme System** - Multiple UI themes

---

## 📚 Learning Resources

This project demonstrates:
- ✅ Clean Architecture principles
- ✅ Domain-Driven Design (DDD)
- ✅ Functional programming patterns
- ✅ Immutable data structures
- ✅ Type-safe error handling
- ✅ Module-based organization
- ✅ OOP/FP hybrid (Avalonia + F#)

---

## 📝 License

Educational project - Free to use and modify

---

## 👨‍💻 Author

Built with ❤️ using F# and functional programming principles

**Made for learning functional programming, clean architecture, and F# development**
