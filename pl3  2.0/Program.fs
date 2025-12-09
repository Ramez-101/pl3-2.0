open System
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open StoreSimulator.Core
open StoreSimulator.Data.Catalog
open StoreSimulator.Data.Cart
open StoreSimulator.Services.PriceCalculator
open StoreSimulator.Services.SearchFilter
open StoreSimulator.Data.FileManager
open StoreSimulator.UI.ConsoleUI

module UI = StoreSimulator.UI.ConsoleUI
module Catalog = StoreSimulator.Data.Catalog
module Cart = StoreSimulator.Data.Cart
module SearchFilter = StoreSimulator.Services.SearchFilter
module PriceCalculator = StoreSimulator.Services.PriceCalculator
module FileManager = StoreSimulator.Data.FileManager

// Main store state
type AppState = {
    Store: Store
    TaxRate: decimal
}

// Initialize the application
let initApp () : AppState =
    {
        Store = {
            Catalog = Catalog.initializeCatalog()
            Cart = Cart.empty
        }
        TaxRate = 8.5m
    }

// Handle view all products
let handleViewProducts (state: AppState) : AppState =
    let products = Catalog.getAllProducts state.Store.Catalog
    UI.displayCatalog products
    UI.waitForEnter()
    state

// Handle search/filter menu
let rec handleSearchFilter (state: AppState) : AppState =
    UI.displaySearchMenu()
    
    match UI.getUserInput "" with
    | "1" ->
        let searchTerm = UI.getUserInput "\nEnter product name to search: "
        let products = Catalog.getAllProducts state.Store.Catalog
        let filtered = SearchFilter.filterByName products searchTerm
        UI.displayCatalog filtered
        UI.waitForEnter()
        handleSearchFilter state
        
    | "2" ->
        let products = Catalog.getAllProducts state.Store.Catalog
        let categories = SearchFilter.getCategories products
        UI.displayCategories categories
        let category = UI.getUserInput "\nEnter category name: "
        let filtered = SearchFilter.filterByCategory products category
        UI.displayCatalog filtered
        UI.waitForEnter()
        handleSearchFilter state
        
    | "3" ->
        match UI.getUserDecimal "\nEnter minimum price: ", UI.getUserDecimal "Enter maximum price: " with
        | Some minPrice, Some maxPrice ->
            let products = Catalog.getAllProducts state.Store.Catalog
            let filtered = SearchFilter.filterByPriceRange products minPrice maxPrice
            UI.displayCatalog filtered
            UI.waitForEnter()
            handleSearchFilter state
        | _ ->
            UI.displayError "Invalid price range"
            UI.waitForEnter()
            handleSearchFilter state
            
    | "4" ->
        let products = Catalog.getAllProducts state.Store.Catalog
        let filtered = SearchFilter.filterInStock products
        UI.displayCatalog filtered
        UI.waitForEnter()
        handleSearchFilter state
        
    | "5" ->
        let products = Catalog.getAllProducts state.Store.Catalog
        let sorted = SearchFilter.sortByPriceAsc products
        UI.displayCatalog sorted
        UI.waitForEnter()
        handleSearchFilter state
        
    | "6" ->
        let products = Catalog.getAllProducts state.Store.Catalog
        let sorted = SearchFilter.sortByPriceDesc products
        UI.displayCatalog sorted
        UI.waitForEnter()
        handleSearchFilter state
        
    | "7" ->
        let products = Catalog.getAllProducts state.Store.Catalog
        let sorted = SearchFilter.sortByName products
        UI.displayCatalog sorted
        UI.waitForEnter()
        handleSearchFilter state
        
    | "8" -> state
        
    | _ ->
        UI.displayError "Invalid choice"
        UI.waitForEnter()
        handleSearchFilter state

// Handle add to cart
let handleAddToCart (state: AppState) : AppState =
    match UI.getUserInt "\nEnter product ID: " with
    | Some productId ->
        match Catalog.getProduct state.Store.Catalog productId with
        | Some product ->
            match UI.getUserInt "Enter quantity: " with
            | Some quantity ->
                match Cart.addToCart state.Store.Cart product quantity with
                | Success newCart ->
                    UI.displaySuccess (sprintf "Added %d x %s to cart" quantity product.Name)
                    UI.waitForEnter()
                    { state with Store = { state.Store with Cart = newCart } }
                | Error msg ->
                    UI.displayError msg
                    UI.waitForEnter()
                    state
            | None ->
                UI.displayError "Invalid quantity"
                UI.waitForEnter()
                state
        | None ->
            UI.displayError "Product not found"
            UI.waitForEnter()
            state
    | None ->
        UI.displayError "Invalid product ID"
        UI.waitForEnter()
        state

// Handle view cart
let handleViewCart (state: AppState) : AppState =
    UI.displayCart state.Store.Cart
    
    if not (Cart.isEmpty state.Store.Cart) then
        let subtotal = PriceCalculator.calculateSubtotal state.Store.Cart
        let discount = PriceCalculator.getAutomaticDiscount subtotal
        let breakdown = PriceCalculator.calculateBreakdown state.Store.Cart discount state.TaxRate
        UI.displayPriceBreakdown breakdown
    
    UI.waitForEnter()
    state

// Handle remove from cart
let handleRemoveFromCart (state: AppState) : AppState =
    if Cart.isEmpty state.Store.Cart then
        UI.displayInfo "Your cart is empty"
        UI.waitForEnter()
        state
    else
        UI.displayCart state.Store.Cart
        match UI.getUserInt "\nEnter product ID to remove: " with
        | Some productId ->
            let newCart = Cart.removeFromCart state.Store.Cart productId
            UI.displaySuccess "Product removed from cart"
            UI.waitForEnter()
            { state with Store = { state.Store with Cart = newCart } }
        | None ->
            UI.displayError "Invalid product ID"
            UI.waitForEnter()
            state

// Handle update quantity
let handleUpdateQuantity (state: AppState) : AppState =
    if Cart.isEmpty state.Store.Cart then
        UI.displayInfo "Your cart is empty"
        UI.waitForEnter()
        state
    else
        UI.displayCart state.Store.Cart
        match UI.getUserInt "\nEnter product ID: " with
        | Some productId ->
            match UI.getUserInt "Enter new quantity (0 to remove): " with
            | Some quantity ->
                match Cart.updateQuantity state.Store.Cart productId quantity with
                | Success newCart ->
                    UI.displaySuccess "Cart updated"
                    UI.waitForEnter()
                    { state with Store = { state.Store with Cart = newCart } }
                | Error msg ->
                    UI.displayError msg
                    UI.waitForEnter()
                    state
            | None ->
                UI.displayError "Invalid quantity"
                UI.waitForEnter()
                state
        | None ->
            UI.displayError "Invalid product ID"
            UI.waitForEnter()
            state

// Handle checkout
let handleCheckout (state: AppState) : AppState =
    if Cart.isEmpty state.Store.Cart then
        UI.displayInfo "Your cart is empty"
        UI.waitForEnter()
        state
    else
        UI.displayCart state.Store.Cart
        
        let subtotal = PriceCalculator.calculateSubtotal state.Store.Cart
        let discount = PriceCalculator.getAutomaticDiscount subtotal
        let breakdown = PriceCalculator.calculateBreakdown state.Store.Cart discount state.TaxRate
        
        UI.displayPriceBreakdown breakdown
        
        let confirm = UI.getUserInput "Proceed with checkout? (y/n): "
        
        if confirm.ToLower() = "y" then
            let receipt = FileManager.createReceipt state.Store.Cart breakdown
            let fileName = FileManager.generateFileName "receipt"
            
            match FileManager.saveReceipt receipt fileName with
            | Success msg ->
                UI.displaySuccess msg
                UI.displaySuccess "Thank you for your purchase!"
                UI.waitForEnter()
                { state with Store = { state.Store with Cart = Cart.empty } }
            | Error msg ->
                UI.displayError (sprintf "Could not save receipt: %s" msg)
                UI.displayInfo "Checkout cancelled"
                UI.waitForEnter()
                state
        else
            UI.displayInfo "Checkout cancelled"
            UI.waitForEnter()
            state

// Handle view receipt history
let handleViewReceipts (state: AppState) : AppState =
    UI.clearScreen()
    printfn "================================================================================"
    printfn "                           RECEIPT HISTORY                                      "
    printfn "================================================================================"
    
    let receiptFiles = 
        System.IO.Directory.GetFiles(".", "receipt_*.json")
        |> Array.sort |> Array.rev |> Array.toList
    
    if receiptFiles.IsEmpty then
        UI.displayInfo "\nNo receipts found. Complete a checkout to create a receipt."
    else
        printfn "\nFound %d receipt(s):\n" receiptFiles.Length
        receiptFiles |> List.iteri (fun i file -> 
            let fileName = System.IO.Path.GetFileName(file)
            printfn "  %d. %s" (i + 1) fileName)
        
        let fileNumber = UI.getUserInput "\nEnter receipt number to view (or 0 to cancel): "
        
        match System.Int32.TryParse(fileNumber) with
        | true, num when num > 0 && num <= receiptFiles.Length ->
            let selectedFile = receiptFiles.[num - 1]
            match FileManager.loadReceipt selectedFile with
            | Success receipt ->
                UI.clearScreen()
                printfn "================================================================================"
                printfn "                            RECEIPT DETAILS                                     "
                printfn "================================================================================"
                printfn "\nDate: %s" (receipt.Date.ToString("yyyy-MM-dd HH:mm:ss"))
                printfn "\nItems:\n"
                for item in receipt.Items do
                    let itemTotal = item.Product.Price * decimal item.Quantity
                    printfn "  %s x%d = $%.2f" item.Product.Name item.Quantity itemTotal
                printfn "\nSubtotal: $%.2f" receipt.Subtotal
                if receipt.Discount > 0m then printfn "Discount: -$%.2f" receipt.Discount
                printfn "Tax: $%.2f" receipt.Tax
                printfn "Total: $%.2f" receipt.Total
                UI.waitForEnter()
            | Error msg ->
                UI.displayError (sprintf "Could not load receipt: %s" msg)
                UI.waitForEnter()
        | _ ->
            UI.displayError "Invalid receipt number"
            UI.waitForEnter()
    state

// Custom Avalonia Application
type App() =
    inherit Application()
    
    override this.Initialize() =
        this.Styles.Add(Avalonia.Themes.Fluent.FluentTheme())
    
    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->
            let loginWindow = StoreSimulator.UI.StoreGui.LoginWindow()
            desktop.MainWindow <- loginWindow
        | _ -> ()
        base.OnFrameworkInitializationCompleted()

// Main application loop (Console mode)
let rec mainLoop (state: AppState) : unit =
    UI.displayMainMenu()
    
    let newState =
        match UI.getUserInput "" with
        | "1" -> handleViewProducts state
        | "2" -> handleSearchFilter state
        | "3" -> handleAddToCart state
        | "4" -> handleViewCart state
        | "5" -> handleRemoveFromCart state
        | "6" -> handleUpdateQuantity state
        | "7" -> handleCheckout state
        | "8" -> handleViewReceipts state
        | "9" -> 
            UI.displayInfo "Thank you for visiting our store!"
            Environment.Exit(0)
            state
        | _ -> 
            UI.displayError "Invalid choice. Please try again."
            UI.waitForEnter()
            state
    
    mainLoop newState

// Console mode
let runConsoleMode () =
    UI.clearScreen()
    printfn "================================================================================"
    printfn "                    Welcome to Simple Store Simulator!                          "
    printfn "                         Made with F# by Students                               "
    printfn "================================================================================"
    printfn "\nPress Enter to start..."
    Console.ReadLine() |> ignore
    let initialState = initApp()
    mainLoop initialState

// GUI mode
let runGuiMode (args: string[]) =
    AppBuilder
        .Configure<App>()
        .UsePlatformDetect()
        .UseSkia()
        .StartWithClassicDesktopLifetime(args)

// Entry point
[<EntryPoint>]
let main argv =
    printfn "================================================================================"
    printfn "                    Simple Store Simulator - F# Edition                         "
    printfn "================================================================================"
    printfn "\nSelect mode:"
    printfn "1. GUI Mode (Graphical Interface)"
    printfn "2. Console Mode (Text Interface)"
    printf "\nEnter your choice (1 or 2): "
    
    match Console.ReadLine() with
    | "1" ->
        printfn "\nStarting GUI mode..."
        runGuiMode argv
    | "2" ->
        runConsoleMode()
        0
    | _ ->
        printfn "\nInvalid choice. Starting GUI mode by default..."
        runGuiMode argv
