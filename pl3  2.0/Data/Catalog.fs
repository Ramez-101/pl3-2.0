module StoreSimulator.Data.Catalog

open System
open StoreSimulator.Core

/// Enhanced products with descriptions, brands, ratings, and tags
let private sampleProducts = [
    { 
        Id = 1
        Name = "MacBook Pro 14 inch"
        Description = "Powerful laptop with M3 chip, 16GB RAM, 512GB SSD. Perfect for professionals and creators."
        Price = 1999.99m
        Category = "Laptops"
        Stock = 10
        Brand = "Apple"
        Rating = 4.8m
        ImageUrl = "macbook_pro.jpg"
        IsAvailable = true
        Tags = ["laptop"; "apple"; "professional"; "m3"]
    }
    { 
        Id = 2
        Name = "Logitech MX Master 3S"
        Description = "Advanced wireless mouse with precision tracking, ergonomic design, and quiet clicks."
        Price = 99.99m
        Category = "Accessories"
        Stock = 50
        Brand = "Logitech"
        Rating = 4.7m
        ImageUrl = "mx_master.jpg"
        IsAvailable = true
        Tags = ["mouse"; "wireless"; "ergonomic"; "logitech"]
    }
    { 
        Id = 3
        Name = "Mechanical Gaming Keyboard"
        Description = "RGB backlit mechanical keyboard with Cherry MX switches and programmable keys."
        Price = 149.99m
        Category = "Accessories"
        Stock = 30
        Brand = "Corsair"
        Rating = 4.6m
        ImageUrl = "keyboard.jpg"
        IsAvailable = true
        Tags = ["keyboard"; "gaming"; "mechanical"; "rgb"]
    }
    { 
        Id = 4
        Name = "Dell UltraSharp 27 inch 4K"
        Description = "Professional 4K monitor with USB-C hub, 99 percent sRGB color accuracy."
        Price = 549.99m
        Category = "Monitors"
        Stock = 15
        Brand = "Dell"
        Rating = 4.5m
        ImageUrl = "dell_monitor.jpg"
        IsAvailable = true
        Tags = ["monitor"; "4k"; "usb-c"; "professional"]
    }
    { 
        Id = 5
        Name = "USB-C Hub 7-in-1"
        Description = "Multi-port USB-C hub with HDMI, USB 3.0, SD card reader, and PD charging."
        Price = 49.99m
        Category = "Accessories"
        Stock = 100
        Brand = "Anker"
        Rating = 4.4m
        ImageUrl = "usb_hub.jpg"
        IsAvailable = true
        Tags = ["usb-c"; "hub"; "adapter"; "charging"]
    }
    { 
        Id = 6
        Name = "Sony WH-1000XM5"
        Description = "Industry-leading noise cancelling headphones with 30-hour battery life."
        Price = 349.99m
        Category = "Audio"
        Stock = 25
        Brand = "Sony"
        Rating = 4.9m
        ImageUrl = "sony_headphones.jpg"
        IsAvailable = true
        Tags = ["headphones"; "wireless"; "noise-cancelling"; "sony"]
    }
    { 
        Id = 7
        Name = "Logitech C920 HD Pro"
        Description = "Full HD 1080p webcam with autofocus and dual stereo microphones."
        Price = 79.99m
        Category = "Accessories"
        Stock = 20
        Brand = "Logitech"
        Rating = 4.3m
        ImageUrl = "webcam.jpg"
        IsAvailable = true
        Tags = ["webcam"; "1080p"; "streaming"; "logitech"]
    }
    { 
        Id = 8
        Name = "Extended Gaming Mouse Pad"
        Description = "Extra-large mouse pad with smooth surface and anti-slip rubber base."
        Price = 29.99m
        Category = "Accessories"
        Stock = 60
        Brand = "SteelSeries"
        Rating = 4.5m
        ImageUrl = "mousepad.jpg"
        IsAvailable = true
        Tags = ["mousepad"; "gaming"; "large"; "steelseries"]
    }
    { 
        Id = 9
        Name = "BenQ LED Desk Lamp"
        Description = "Smart LED desk lamp with adjustable color temperature and brightness."
        Price = 199.99m
        Category = "Accessories"
        Stock = 40
        Brand = "BenQ"
        Rating = 4.6m
        ImageUrl = "desk_lamp.jpg"
        IsAvailable = true
        Tags = ["lamp"; "led"; "desk"; "benq"]
    }
    { 
        Id = 10
        Name = "Adjustable Laptop Stand"
        Description = "Ergonomic aluminum laptop stand with height and angle adjustment."
        Price = 59.99m
        Category = "Accessories"
        Stock = 45
        Brand = "Rain Design"
        Rating = 4.4m
        ImageUrl = "laptop_stand.jpg"
        IsAvailable = true
        Tags = ["stand"; "laptop"; "ergonomic"; "aluminum"]
    }
    { 
        Id = 11
        Name = "Samsung 980 PRO SSD 1TB"
        Description = "NVMe M.2 SSD with read speeds up to 7000 MB/s for gaming and content creation."
        Price = 129.99m
        Category = "Storage"
        Stock = 35
        Brand = "Samsung"
        Rating = 4.8m
        ImageUrl = "ssd.jpg"
        IsAvailable = true
        Tags = ["ssd"; "nvme"; "storage"; "samsung"]
    }
    { 
        Id = 12
        Name = "iPad Pro 12.9 inch"
        Description = "Powerful tablet with M2 chip, Liquid Retina XDR display, and Apple Pencil support."
        Price = 1099.99m
        Category = "Tablets"
        Stock = 12
        Brand = "Apple"
        Rating = 4.7m
        ImageUrl = "ipad_pro.jpg"
        IsAvailable = true
        Tags = ["tablet"; "ipad"; "apple"; "m2"]
    }
    { 
        Id = 13
        Name = "AirPods Pro 2"
        Description = "Active noise cancellation earbuds with spatial audio and adaptive transparency."
        Price = 249.99m
        Category = "Audio"
        Stock = 30
        Brand = "Apple"
        Rating = 4.6m
        ImageUrl = "airpods.jpg"
        IsAvailable = true
        Tags = ["earbuds"; "wireless"; "airpods"; "apple"]
    }
    { 
        Id = 14
        Name = "Elgato Stream Deck"
        Description = "15 customizable LCD keys for streaming, content creation, and productivity."
        Price = 149.99m
        Category = "Accessories"
        Stock = 18
        Brand = "Elgato"
        Rating = 4.7m
        ImageUrl = "stream_deck.jpg"
        IsAvailable = true
        Tags = ["streaming"; "productivity"; "elgato"; "customizable"]
    }
    { 
        Id = 15
        Name = "CalDigit TS4 Thunderbolt 4 Dock"
        Description = "Premium docking station with 18 ports including 2.5Gb Ethernet and 98W charging."
        Price = 399.99m
        Category = "Accessories"
        Stock = 8
        Brand = "CalDigit"
        Rating = 4.8m
        ImageUrl = "dock.jpg"
        IsAvailable = true
        Tags = ["dock"; "thunderbolt"; "usb-c"; "professional"]
    }
]

/// Mutable catalog for stock updates
let mutable private catalogData: Map<int, Product> = Map.empty

/// Initialize catalog as a Map with product Id as key
let initializeCatalog () : Map<int, Product> =
    catalogData <- sampleProducts |> List.map (fun p -> (p.Id, p)) |> Map.ofList
    catalogData

/// Get current catalog
let getCatalog () : Map<int, Product> =
    if catalogData.IsEmpty then initializeCatalog() else catalogData

/// Get a product by Id
let getProduct (catalog: Map<int, Product>) (productId: int) : Product option =
    Map.tryFind productId catalog

/// Get all products as a list
let getAllProducts (catalog: Map<int, Product>) : Product list =
    catalog |> Map.toList |> List.map snd

/// Add a new product to catalog
let addProduct (catalog: Map<int, Product>) (product: Product) : Map<int, Product> =
    let updated = Map.add product.Id product catalog
    catalogData <- updated
    updated

/// Update product stock after purchase
let updateStock (catalog: Map<int, Product>) (productId: int) (newStock: int) : Map<int, Product> =
    match Map.tryFind productId catalog with
    | Some product ->
        let updatedProduct = { product with Stock = newStock; IsAvailable = newStock > 0 }
        let updated = Map.add productId updatedProduct catalog
        catalogData <- updated
        updated
    | None -> catalog

/// Decrease stock when item is purchased
let decreaseStock (catalog: Map<int, Product>) (productId: int) (quantity: int) : StoreResult<Map<int, Product>> =
    match Map.tryFind productId catalog with
    | Some product ->
        if product.Stock >= quantity then
            let newStock = product.Stock - quantity
            let updatedProduct = { product with Stock = newStock; IsAvailable = newStock > 0 }
            let updated = Map.add productId updatedProduct catalog
            catalogData <- updated
            Success updated
        else
            Error (sprintf "Not enough stock for %s. Available: %d, Requested: %d" product.Name product.Stock quantity)
    | None -> Error (sprintf "Product with ID %d not found" productId)

/// Restore stock (for cancelled orders)
let restoreStock (catalog: Map<int, Product>) (productId: int) (quantity: int) : Map<int, Product> =
    match Map.tryFind productId catalog with
    | Some product ->
        let newStock = product.Stock + quantity
        let updatedProduct = { product with Stock = newStock; IsAvailable = true }
        let updated = Map.add productId updatedProduct catalog
        catalogData <- updated
        updated
    | None -> catalog

/// Get products by category
let getProductsByCategory (catalog: Map<int, Product>) (category: string) : Product list =
    getAllProducts catalog |> List.filter (fun p -> p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))

/// Get all categories
let getCategories (catalog: Map<int, Product>) : string list =
    getAllProducts catalog |> List.map (fun p -> p.Category) |> List.distinct |> List.sort

/// Get all brands
let getBrands (catalog: Map<int, Product>) : string list =
    getAllProducts catalog |> List.map (fun p -> p.Brand) |> List.distinct |> List.sort

/// Search products by tag
let searchByTag (catalog: Map<int, Product>) (tag: string) : Product list =
    let lowerTag = tag.ToLower()
    getAllProducts catalog |> List.filter (fun p -> p.Tags |> List.exists (fun t -> t.ToLower().Contains(lowerTag)))

/// Get top rated products
let getTopRated (catalog: Map<int, Product>) (count: int) : Product list =
    getAllProducts catalog |> List.sortByDescending (fun p -> p.Rating) |> List.truncate count

/// Get products in stock only
let getInStock (catalog: Map<int, Product>) : Product list =
    getAllProducts catalog |> List.filter (fun p -> p.Stock > 0 && p.IsAvailable)

/// Get next available product ID
let getNextProductId (catalog: Map<int, Product>) : int =
    if catalog.IsEmpty then 1
    else (catalog |> Map.toList |> List.map fst |> List.max) + 1

/// Add a new product to catalog (admin function)
let addNewProduct (catalog: Map<int, Product>) (name: string) (description: string) (price: decimal) (category: string) (stock: int) (brand: string) : StoreResult<Map<int, Product> * Product> =
    if String.IsNullOrWhiteSpace(name) then
        Error "Product name is required"
    elif price <= 0m then
        Error "Price must be greater than 0"
    elif stock < 0 then
        Error "Stock cannot be negative"
    elif String.IsNullOrWhiteSpace(category) then
        Error "Category is required"
    elif String.IsNullOrWhiteSpace(brand) then
        Error "Brand is required"
    else
        let newId = getNextProductId catalog
        let newProduct = {
            Id = newId
            Name = name.Trim()
            Description = if String.IsNullOrWhiteSpace(description) then "No description" else description.Trim()
            Price = price
            Category = category.Trim()
            Stock = stock
            Brand = brand.Trim()
            Rating = 0m
            ImageUrl = ""
            IsAvailable = stock > 0
            Tags = []
        }
        let updated = Map.add newId newProduct catalog
        catalogData <- updated
        Success (updated, newProduct)

/// Update existing product (admin function)
let updateProduct (catalog: Map<int, Product>) (productId: int) (name: string option) (description: string option) (price: decimal option) (category: string option) (stock: int option) (brand: string option) : StoreResult<Map<int, Product>> =
    match Map.tryFind productId catalog with
    | Some product ->
        let updatedProduct = {
            product with
                Name = defaultArg name product.Name
                Description = defaultArg description product.Description
                Price = defaultArg price product.Price
                Category = defaultArg category product.Category
                Stock = defaultArg stock product.Stock
                Brand = defaultArg brand product.Brand
                IsAvailable = (defaultArg stock product.Stock) > 0
        }
        let updated = Map.add productId updatedProduct catalog
        catalogData <- updated
        Success updated
    | None -> Error (sprintf "Product with ID %d not found" productId)

/// Delete product (admin function - marks as unavailable)
let deleteProduct (catalog: Map<int, Product>) (productId: int) : StoreResult<Map<int, Product>> =
    match Map.tryFind productId catalog with
    | Some product ->
        let updatedProduct = { product with IsAvailable = false; Stock = 0 }
        let updated = Map.add productId updatedProduct catalog
        catalogData <- updated
        Success updated
    | None -> Error (sprintf "Product with ID %d not found" productId)

/// Get low stock products (admin view)
let getLowStockProducts (catalog: Map<int, Product>) (threshold: int) : Product list =
    getAllProducts catalog |> List.filter (fun p -> p.Stock <= threshold && p.Stock > 0) |> List.sortBy (fun p -> p.Stock)

/// Get out of stock products (admin view)
let getOutOfStockProducts (catalog: Map<int, Product>) : Product list =
    getAllProducts catalog |> List.filter (fun p -> p.Stock = 0 || not p.IsAvailable)
