open System
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Types
open Catalog
open Cart
open PriceCalculator
open SearchFilter
open FileManager
open UI

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
        TaxRate = 8.5m // 8.5% tax rate
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
    | "1" -> // Search by name
        let searchTerm = UI.getUserInput "\nEnter product name to search: "
        let products = Catalog.getAllProducts state.Store.Catalog
        let filtered = SearchFilter.filterByName products searchTerm
        UI.displayCatalog filtered
        UI.waitForEnter()
        handleSearchFilter state
        
    | "2" -> // Filter by category
        let products = Catalog.getAllProducts state.Store.Catalog
        let categories = SearchFilter.getCategories products
        UI.displayCategories categories
        let category = UI.getUserInput "\nEnter category name: "
        let filtered = SearchFilter.filterByCategory products category
        UI.displayCatalog filtered
        UI.waitForEnter()
        handleSearchFilter state
        
    | "3" -> // Filter by price range
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
            
    | "4" -> // View in-stock only
        let products = Catalog.getAllProducts state.Store.Catalog
        let filtered = SearchFilter.filterInStock products
        UI.displayCatalog filtered
        UI.waitForEnter()
        handleSearchFilter state
        
    | "5" -> // Sort by price ascending
        let products = Catalog.getAllProducts state.Store.Catalog
        let sorted = SearchFilter.sortByPriceAsc products
        UI.displayCatalog sorted
        UI.waitForEnter()
        handleSearchFilter state
        
    | "6" -> // Sort by price descending
        let products = Catalog.getAllProducts state.Store.Catalog
        let sorted = SearchFilter.sortByPriceDesc products
        UI.displayCatalog sorted
        UI.waitForEnter()
        handleSearchFilter state
        
    | "7" -> // Sort by name
        let products = Catalog.getAllProducts state.Store.Catalog
        let sorted = SearchFilter.sortByName products
        UI.displayCatalog sorted
        UI.waitForEnter()
        handleSearchFilter state
        
    | "8" -> // Back to main menu
        state
        
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
                    UI.displaySuccess $"Added {quantity} x {product.Name} to cart"
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
            // Create receipt
            let receipt = FileManager.createReceipt state.Store.Cart breakdown
            let fileName = FileManager.generateFileName "receipt"
            
            match FileManager.saveReceipt receipt fileName with
            | Success msg ->
                UI.displaySuccess msg
                UI.displaySuccess "Thank you for your purchase!"
                UI.waitForEnter()
                // Clear cart after successful checkout
                { state with Store = { state.Store with Cart = Cart.empty } }
            | Error msg ->
                UI.displayError $"Could not save receipt: {msg}"
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
    printfn "╔══════════════════════════════════════════════════════════════════════════════╗"
    printfn "║                           RECEIPT HISTORY                                    ║"
    printfn "╚══════════════════════════════════════════════════════════════════════════════╝"
    
    // Get all JSON receipt files in current directory
    let receiptFiles = 
        System.IO.Directory.GetFiles(".", "receipt_*.json")
        |> Array.sort
        |> Array.rev  // Most recent first
        |> Array.toList
    
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
                printfn "╔══════════════════════════════════════════════════════════════════════════════╗"
                printfn "║                            RECEIPT DETAILS                                   ║"
                printfn "╚══════════════════════════════════════════════════════════════════════════════╝"
                printfn "\n📅 Date: %s" (receipt.Date.ToString("yyyy-MM-dd HH:mm:ss"))
                printfn "\n📦 Items:\n"
                printfn "%-35s %5s %10s %12s" "Product" "Qty" "Price" "Total"
                printfn "%s" (String.replicate 78 "─")
                
                for item in receipt.Items do
                    let itemTotal = item.Product.Price * decimal item.Quantity
                    printfn "%-35s x%-4d $%9.2f  = $%10.2f" 
                        item.Product.Name 
                        item.Quantity 
                        item.Product.Price
                        itemTotal
                
                printfn "\n%s" (String.replicate 78 "─")
                printfn "%51s $%10.2f" "Subtotal:" receipt.Subtotal
                
                if receipt.Discount > 0m then
                    printfn "%51s -$%9.2f" "Discount:" receipt.Discount
                
                printfn "%51s $%10.2f" "Tax (8.5%):" receipt.Tax
                printfn "%s" (String.replicate 78 "═")
                printfn "%51s $%10.2f" "TOTAL:" receipt.Total
                printfn "%s" (String.replicate 78 "═")
            | Error msg ->
                UI.displayError $"\nCould not load receipt: {msg}"
        | true, 0 ->
            UI.displayInfo "\nCancelled"
        | _ ->
            UI.displayError "\nInvalid selection"
    
    UI.waitForEnter()
    state

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
    printfn "╔══════════════════════════════════════════════════════════════════════════════╗"
    printfn "║                    Welcome to Simple Store Simulator!                        ║"
    printfn "║                         Made with F# by Students                             ║"
    printfn "╚══════════════════════════════════════════════════════════════════════════════╝"
    printfn "\nPress Enter to start..."
    Console.ReadLine() |> ignore
    
    let initialState = initApp()
    mainLoop initialState

// Custom Avalonia Application
type App() =
    inherit Application()
    
    override this.Initialize() =
        this.Styles.Add(Avalonia.Themes.Fluent.FluentTheme())
    
    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->
            desktop.MainWindow <- SimpleGui.StoreWindow()
        | _ -> ()
        
        base.OnFrameworkInitializationCompleted()

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
    printfn "╔══════════════════════════════════════════════════════════════════════════════╗"
    printfn "║                    Simple Store Simulator - F# Edition                       ║"
    printfn "╚══════════════════════════════════════════════════════════════════════════════╝"
    printfn "\nSelect mode:"
    printfn "1. GUI Mode (Graphical Interface)"
    printfn "2. Console Mode (Text Interface)"
    printf "\nEnter your choice (1 or 2): "
    
    let choice = Console.ReadLine()
    
    match choice with
    | "1" ->
        printfn "\nStarting GUI mode..."
        runGuiMode argv
    | "2" ->
        runConsoleMode()
        0
    | _ ->
        printfn "\nInvalid choice. Starting GUI mode by default..."
        runGuiMode argv
