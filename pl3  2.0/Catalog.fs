module Catalog

open Types

// Catalog Developer - Initialize product list and structure Map
let private sampleProducts = [
    { Id = 1; Name = "Laptop"; Price = 999.99m; Category = "Electronics"; Stock = 10 }
    { Id = 2; Name = "Mouse"; Price = 25.50m; Category = "Electronics"; Stock = 50 }
    { Id = 3; Name = "Keyboard"; Price = 75.00m; Category = "Electronics"; Stock = 30 }
    { Id = 4; Name = "Monitor"; Price = 299.99m; Category = "Electronics"; Stock = 15 }
    { Id = 5; Name = "USB Cable"; Price = 9.99m; Category = "Accessories"; Stock = 100 }
    { Id = 6; Name = "Headphones"; Price = 49.99m; Category = "Electronics"; Stock = 25 }
    { Id = 7; Name = "Webcam"; Price = 79.99m; Category = "Electronics"; Stock = 20 }
    { Id = 8; Name = "Mouse Pad"; Price = 12.99m; Category = "Accessories"; Stock = 60 }
    { Id = 9; Name = "Desk Lamp"; Price = 35.00m; Category = "Accessories"; Stock = 40 }
    { Id = 10; Name = "Phone Stand"; Price = 15.99m; Category = "Accessories"; Stock = 45 }
]

// Initialize catalog as a Map with product Id as key
let initializeCatalog () : Map<int, Product> =
    sampleProducts
    |> List.map (fun p -> (p.Id, p))
    |> Map.ofList

// Get a product by Id
let getProduct (catalog: Map<int, Product>) (productId: int) : Product option =
    Map.tryFind productId catalog

// Get all products as a list
let getAllProducts (catalog: Map<int, Product>) : Product list =
    catalog
    |> Map.toList
    |> List.map snd

// Add a new product to catalog (for future expansion)
let addProduct (catalog: Map<int, Product>) (product: Product) : Map<int, Product> =
    Map.add product.Id product catalog

// Update product stock after purchase
let updateStock (catalog: Map<int, Product>) (productId: int) (newStock: int) : Map<int, Product> =
    match Map.tryFind productId catalog with
    | Some product ->
        let updatedProduct = { product with Stock = newStock }
        Map.add productId updatedProduct catalog
    | None -> catalog
