module StoreSimulator.UI.ConsoleUI

open System
open StoreSimulator.Core
open StoreSimulator.Services.PriceCalculator
open StoreSimulator.Data.Cart

/// Display a single product
let displayProduct (product: Product) =
    printfn "  ID: %-3d | %-20s | $%-8.2f | Category: %-15s | Stock: %d" 
        product.Id product.Name product.Price product.Category product.Stock

/// Display all products
let displayCatalog (products: Product list) =
    printfn "\n================================================================================"
    printfn "                            PRODUCT CATALOG                                     "
    printfn "================================================================================"
    if List.isEmpty products then printfn "  No products found."
    else products |> List.iter displayProduct
    printfn ""

/// Display a single cart item
let displayCartItem (item: CartItem) =
    let itemTotal = item.Product.Price * decimal item.Quantity
    printfn "  %-20s | Qty: %-3d | $%-8.2f each | Total: $%-8.2f" 
        item.Product.Name item.Quantity item.Product.Price itemTotal

/// Display shopping cart
let displayCart (cart: CartItem list) =
    printfn "\n================================================================================"
    printfn "                            SHOPPING CART                                       "
    printfn "================================================================================"
    if List.isEmpty cart then printfn "  Your cart is empty."
    else
        cart |> List.iter displayCartItem
        let itemCount = getItemCount cart
        printfn "  ------------------------------------------------------------------------------"
        printfn "  Total Items: %d" itemCount
    printfn ""

/// Display price breakdown
let displayPriceBreakdown (breakdown: PriceBreakdown) =
    printfn "\n================================================================================"
    printfn "                          PRICE BREAKDOWN                                       "
    printfn "================================================================================"
    printfn "  Subtotal:        $%.2f" breakdown.Subtotal
    if breakdown.Discount > 0m then
        printfn "  Discount:       -$%.2f" breakdown.Discount
    printfn "  Tax (%.0f%%):       $%.2f" breakdown.TaxRate breakdown.Tax
    printfn "  ------------------------------------------------------------------------------"
    printfn "  TOTAL:           $%.2f" breakdown.Total
    printfn ""

/// Display search menu
let displaySearchMenu () =
    printfn "\n================================================================================"
    printfn "                         SEARCH & FILTER                                        "
    printfn "================================================================================"
    printfn "  1. Search by Name"
    printfn "  2. Filter by Category"
    printfn "  3. Filter by Price Range"
    printfn "  4. View In-Stock Products Only"
    printfn "  5. Sort by Price (Low to High)"
    printfn "  6. Sort by Price (High to Low)"
    printfn "  7. Sort by Name"
    printfn "  8. Back to Main Menu"
    printfn "================================================================================"
    printf "\nEnter your choice: "

/// Display categories
let displayCategories (categories: string list) =
    printfn "\nAvailable Categories:"
    categories |> List.iteri (fun i cat -> printfn "  %d. %s" (i + 1) cat)

/// Get user input
let getUserInput (prompt: string) : string =
    printf "%s" prompt
    Console.ReadLine()

/// Get integer input
let getUserInt (prompt: string) : int option =
    printf "%s" prompt
    match Int32.TryParse(Console.ReadLine()) with
    | true, value -> Some value
    | false, _ -> None

/// Get decimal input
let getUserDecimal (prompt: string) : decimal option =
    printf "%s" prompt
    match Decimal.TryParse(Console.ReadLine()) with
    | true, value -> Some value
    | false, _ -> None

/// Display success message
let displaySuccess (message: string) =
    printfn "\n[SUCCESS] %s" message

/// Display error message
let displayError (message: string) =
    printfn "\n[ERROR] %s" message

/// Display info message
let displayInfo (message: string) =
    printfn "\n[INFO] %s" message

/// Wait for user to press enter
let waitForEnter () =
    printf "\nPress Enter to continue..."
    Console.ReadLine() |> ignore

/// Clear console
let clearScreen () =
    Console.Clear()

/// Display main menu
let displayMainMenu () =
    clearScreen()
    printfn "================================================================================"
    printfn "                        SIMPLE STORE SIMULATOR                                  "
    printfn "================================================================================"
    printfn "  1. View All Products"
    printfn "  2. Search/Filter Products"
    printfn "  3. Add Product to Cart"
    printfn "  4. View Cart"
    printfn "  5. Remove Product from Cart"
    printfn "  6. Update Cart Item Quantity"
    printfn "  7. Checkout"
    printfn "  8. View Receipt History"
    printfn "  9. Exit"
    printfn "================================================================================"
    printf "\nEnter your choice: "
