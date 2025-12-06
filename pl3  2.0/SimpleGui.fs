module SimpleGui

open System
open System.IO
open Avalonia
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open Types

type StoreWindow() as this =
    inherit Window()
    
    // State
    let mutable catalog = Catalog.initializeCatalog()
    let mutable cart = Cart.empty
    let mutable selectedProduct: Product option = None
    let mutable selectedCartItem: CartItem option = None
    let taxRate = 8.5m
    
    // Main Controls
    let productListBox = ListBox(Height = 350.0, MinWidth = 250.0)
    let searchBox = TextBox(Watermark = "Search products...", MinWidth = 200.0)
    let searchButton = Button(Content = "Search", Padding = Thickness(10.0, 5.0))
    let showAllButton = Button(Content = "Show All", Padding = Thickness(10.0, 5.0))
    let advancedSearchButton = Button(Content = "Advanced...", Padding = Thickness(10.0, 5.0))
    
    let productNameLabel = TextBlock(FontSize = 18.0, FontWeight = FontWeight.Bold)
    let productPriceLabel = TextBlock(FontSize = 14.0)
    let productCategoryLabel = TextBlock(FontSize = 14.0)
    let productStockLabel = TextBlock(FontSize = 14.0)
    let quantityBox = NumericUpDown(Minimum = 1.0m, Maximum = 100.0m, Value = System.Nullable<decimal>(1.0m), Width = 100.0)
    let addToCartButton = Button(Content = "Add to Cart", Padding = Thickness(20.0, 10.0), FontSize = 14.0)
    
    let cartListBox = ListBox(Height = 250.0, MinWidth = 250.0)
    let removeFromCartButton = Button(Content = "Remove Item", Padding = Thickness(10.0, 5.0), IsEnabled = false)
    let updateQuantityButton = Button(Content = "Update Qty", Padding = Thickness(10.0, 5.0), IsEnabled = false)
    let clearCartButton = Button(Content = "Clear Cart", Padding = Thickness(10.0, 5.0))
    
    let subtotalLabel = TextBlock(FontSize = 14.0)
    let discountLabel = TextBlock(FontSize = 14.0, Foreground = Brushes.Green)
    let taxLabel = TextBlock(FontSize = 14.0)
    let totalLabel = TextBlock(FontSize = 18.0, FontWeight = FontWeight.Bold)
    let checkoutButton = Button(Content = "Checkout", Padding = Thickness(30.0, 12.0), FontSize = 16.0)
    let viewReceiptsButton = Button(Content = "View Receipt History", Padding = Thickness(20.0, 8.0), FontSize = 12.0)
    
    let statusBar = TextBlock(Background = Brushes.LightBlue, Padding = Thickness(10.0), FontSize = 12.0)
    
    do
        this.Title <- "Simple Store Simulator - F# Edition"
        this.Width <- 1100.0
        this.Height <- 650.0
        this.MinWidth <- 900.0
        this.MinHeight <- 550.0
        
        this.LoadProducts()
        this.SetupUI()
        this.AttachEventHandlers()
        this.UpdateProductDetails()
        this.UpdateCartDisplay()
        this.UpdateStatus "Welcome to Store Simulator! ??"
    
    member private this.LoadProducts() =
        let products = Catalog.getAllProducts catalog
        productListBox.Items.Clear()
        for product in products do
            productListBox.Items.Add($"{product.Name} - ${product.Price:F2} (Stock: {product.Stock})") |> ignore
    
    member private this.SetupUI() =
        let mainGrid = Grid()
        mainGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(2.0, GridUnitType.Star)))
        mainGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(1.5, GridUnitType.Star)))
        mainGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(2.0, GridUnitType.Star)))
        
        // Left Panel - Product List with Advanced Search
        let leftPanel = StackPanel(Margin = Thickness(10.0))
        leftPanel.Children.Add(TextBlock(Text = "Products", FontSize = 16.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 10.0))) |> ignore
        
        let searchPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 0.0, 0.0, 5.0))
        searchPanel.Children.Add(searchBox) |> ignore
        searchPanel.Children.Add(searchButton) |> ignore
        leftPanel.Children.Add(searchPanel) |> ignore
        
        let buttonPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 0.0, 0.0, 10.0))
        buttonPanel.Children.Add(showAllButton) |> ignore
        buttonPanel.Children.Add(advancedSearchButton) |> ignore
        leftPanel.Children.Add(buttonPanel) |> ignore
        
        leftPanel.Children.Add(productListBox) |> ignore
        
        let leftBorder = Border(BorderBrush = Brushes.Gray, BorderThickness = Thickness(1.0), Padding = Thickness(10.0), Child = leftPanel)
        Grid.SetColumn(leftBorder, 0)
        mainGrid.Children.Add(leftBorder) |> ignore
        
        // Middle Panel - Product Details
        let middlePanel = StackPanel(Margin = Thickness(10.0))
        middlePanel.Children.Add(TextBlock(Text = "Product Details", FontSize = 16.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 10.0))) |> ignore
        
        let detailsBorder = Border(BorderBrush = Brushes.LightGray, BorderThickness = Thickness(1.0), Padding = Thickness(10.0), Margin = Thickness(0.0, 10.0))
        let detailsStack = StackPanel()
        detailsStack.Children.Add(productNameLabel) |> ignore
        detailsStack.Children.Add(productPriceLabel) |> ignore
        detailsStack.Children.Add(productCategoryLabel) |> ignore
        detailsStack.Children.Add(productStockLabel) |> ignore
        detailsStack.Children.Add(TextBlock(Text = "Quantity:", Margin = Thickness(0.0, 10.0, 0.0, 5.0))) |> ignore
        detailsStack.Children.Add(quantityBox) |> ignore
        detailsStack.Children.Add(addToCartButton) |> ignore
        detailsBorder.Child <- detailsStack
        middlePanel.Children.Add(detailsBorder) |> ignore
        
        let middleBorder = Border(BorderBrush = Brushes.Gray, BorderThickness = Thickness(1.0), Padding = Thickness(10.0), Child = middlePanel)
        Grid.SetColumn(middleBorder, 1)
        mainGrid.Children.Add(middleBorder) |> ignore
        
        // Right Panel - Cart with Management
        let rightPanel = StackPanel(Margin = Thickness(10.0))
        rightPanel.Children.Add(TextBlock(Text = "Shopping Cart", FontSize = 16.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 10.0))) |> ignore
        rightPanel.Children.Add(cartListBox) |> ignore
        
        let cartButtonPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 5.0, 0.0, 10.0))
        cartButtonPanel.Children.Add(removeFromCartButton) |> ignore
        cartButtonPanel.Children.Add(updateQuantityButton) |> ignore
        cartButtonPanel.Children.Add(clearCartButton) |> ignore
        rightPanel.Children.Add(cartButtonPanel) |> ignore
        
        let pricePanel = StackPanel(Margin = Thickness(0.0, 10.0))
        pricePanel.Children.Add(TextBlock(Text = "Price Breakdown", FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 5.0))) |> ignore
        pricePanel.Children.Add(subtotalLabel) |> ignore
        pricePanel.Children.Add(discountLabel) |> ignore
        pricePanel.Children.Add(taxLabel) |> ignore
        pricePanel.Children.Add(totalLabel) |> ignore
        pricePanel.Children.Add(checkoutButton) |> ignore
        pricePanel.Children.Add(viewReceiptsButton) |> ignore
        rightPanel.Children.Add(pricePanel) |> ignore
        
        let rightBorder = Border(BorderBrush = Brushes.Gray, BorderThickness = Thickness(1.0), Padding = Thickness(10.0), Child = rightPanel)
        Grid.SetColumn(rightBorder, 2)
        mainGrid.Children.Add(rightBorder) |> ignore
        
        // Main layout
        let dockPanel = DockPanel()
        DockPanel.SetDock(statusBar, Dock.Bottom)
        dockPanel.Children.Add(statusBar) |> ignore
        dockPanel.Children.Add(mainGrid) |> ignore
        
        this.Content <- dockPanel
    
    member private this.AttachEventHandlers() =
        productListBox.SelectionChanged.Add(fun _ -> this.OnProductSelected())
        searchButton.Click.Add(fun _ -> this.OnSearch())
        showAllButton.Click.Add(fun _ -> this.OnShowAll())
        advancedSearchButton.Click.Add(fun _ -> this.OnAdvancedSearch())
        addToCartButton.Click.Add(fun _ -> this.OnAddToCart())
        
        cartListBox.SelectionChanged.Add(fun _ -> this.OnCartItemSelected())
        removeFromCartButton.Click.Add(fun _ -> this.OnRemoveFromCart())
        updateQuantityButton.Click.Add(fun _ -> this.OnUpdateQuantity())
        clearCartButton.Click.Add(fun _ -> this.OnClearCart())
        
        checkoutButton.Click.Add(fun _ -> this.OnCheckout())
        viewReceiptsButton.Click.Add(fun _ -> this.OnViewReceipts())
    
    member private this.OnProductSelected() =
        if productListBox.SelectedIndex >= 0 then
            let products = Catalog.getAllProducts catalog
            if productListBox.SelectedIndex < products.Length then
                selectedProduct <- Some products.[productListBox.SelectedIndex]
                this.UpdateProductDetails()
    
    member private this.OnCartItemSelected() =
        if cartListBox.SelectedIndex >= 0 && not (Cart.isEmpty cart) then
            selectedCartItem <- Some cart.[cartListBox.SelectedIndex]
            removeFromCartButton.IsEnabled <- true
            updateQuantityButton.IsEnabled <- true
        else
            selectedCartItem <- None
            removeFromCartButton.IsEnabled <- false
            updateQuantityButton.IsEnabled <- false
    
    member private this.OnSearch() =
        let searchTerm = searchBox.Text
        if String.IsNullOrWhiteSpace(searchTerm) then
            this.LoadProducts()
        else
            let products = Catalog.getAllProducts catalog
            let filtered = SearchFilter.filterByName products searchTerm
            productListBox.Items.Clear()
            for product in filtered do
                productListBox.Items.Add($"{product.Name} - ${product.Price:F2} (Stock: {product.Stock})") |> ignore
            this.UpdateStatus $"Found {filtered.Length} product(s) matching '{searchTerm}'"
    
    member private this.OnShowAll() =
        searchBox.Text <- ""
        this.LoadProducts()
        this.UpdateStatus "Showing all products"
    
    member private this.OnAdvancedSearch() =
        let dialog = Window(Title = "Advanced Search & Filter", Width = 400.0, Height = 500.0, WindowStartupLocation = WindowStartupLocation.CenterOwner)
        
        let mainPanel = StackPanel(Margin = Thickness(20.0))
        
        // Category Filter
        mainPanel.Children.Add(TextBlock(Text = "Filter by Category:", FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 10.0, 0.0, 5.0))) |> ignore
        let products = Catalog.getAllProducts catalog
        let categories = SearchFilter.getCategories products
        let categoryCombo = ComboBox(MinWidth = 200.0)
        categoryCombo.Items.Add("All Categories") |> ignore
        for cat in categories do
            categoryCombo.Items.Add(cat) |> ignore
        categoryCombo.SelectedIndex <- 0
        mainPanel.Children.Add(categoryCombo) |> ignore
        
        // Price Range Filter
        mainPanel.Children.Add(TextBlock(Text = "Price Range:", FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 10.0, 0.0, 5.0))) |> ignore
        let pricePanel = StackPanel(Orientation = Orientation.Horizontal)
        pricePanel.Children.Add(TextBlock(Text = "Min: $", VerticalAlignment = VerticalAlignment.Center)) |> ignore
        let minPriceBox = NumericUpDown(Minimum = 0m, Maximum = 10000m, Value = Nullable(0m), Width = 80.0, Margin = Thickness(5.0, 0.0))
        pricePanel.Children.Add(minPriceBox) |> ignore
        pricePanel.Children.Add(TextBlock(Text = "Max: $", VerticalAlignment = VerticalAlignment.Center, Margin = Thickness(10.0, 0.0, 0.0, 0.0))) |> ignore
        let maxPriceBox = NumericUpDown(Minimum = 0m, Maximum = 10000m, Value = Nullable(10000m), Width = 80.0, Margin = Thickness(5.0, 0.0))
        pricePanel.Children.Add(maxPriceBox) |> ignore
        mainPanel.Children.Add(pricePanel) |> ignore
        
        // Stock Filter
        let inStockCheck = CheckBox(Content = "In-Stock Only", Margin = Thickness(0.0, 15.0))
        mainPanel.Children.Add(inStockCheck) |> ignore
        
        // Sort Options
        mainPanel.Children.Add(TextBlock(Text = "Sort By:", FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 10.0, 0.0, 5.0))) |> ignore
        let sortCombo = ComboBox(MinWidth = 200.0)
        sortCombo.Items.Add("Name (A-Z)") |> ignore
        sortCombo.Items.Add("Price (Low to High)") |> ignore
        sortCombo.Items.Add("Price (High to Low)") |> ignore
        sortCombo.SelectedIndex <- 0
        mainPanel.Children.Add(sortCombo) |> ignore
        
        // Buttons
        let buttonPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 20.0), HorizontalAlignment = HorizontalAlignment.Center)
        let applyButton = Button(Content = "Apply", Padding = Thickness(20.0, 10.0), Margin = Thickness(5.0))
        let cancelButton = Button(Content = "Cancel", Padding = Thickness(20.0, 10.0), Margin = Thickness(5.0))
        buttonPanel.Children.Add(applyButton) |> ignore
        buttonPanel.Children.Add(cancelButton) |> ignore
        mainPanel.Children.Add(buttonPanel) |> ignore
        
        applyButton.Click.Add(fun _ ->
            let mutable filtered = Catalog.getAllProducts catalog
            
            // Apply category filter
            if categoryCombo.SelectedIndex > 0 then
                let selectedCat = categoryCombo.SelectedItem :?> string
                filtered <- SearchFilter.filterByCategory filtered selectedCat
            
            // Apply price range filter
            if minPriceBox.Value.HasValue && maxPriceBox.Value.HasValue then
                filtered <- SearchFilter.filterByPriceRange filtered minPriceBox.Value.Value maxPriceBox.Value.Value
            
            // Apply stock filter
            if inStockCheck.IsChecked.HasValue && inStockCheck.IsChecked.Value then
                filtered <- SearchFilter.filterInStock filtered
            
            // Apply sorting
            filtered <- match sortCombo.SelectedIndex with
                        | 1 -> SearchFilter.sortByPriceAsc filtered
                        | 2 -> SearchFilter.sortByPriceDesc filtered
                        | _ -> SearchFilter.sortByName filtered
            
            // Update product list
            productListBox.Items.Clear()
            for product in filtered do
                productListBox.Items.Add($"{product.Name} - ${product.Price:F2} (Stock: {product.Stock})") |> ignore
            
            this.UpdateStatus $"Filter applied: {filtered.Length} product(s) found"
            dialog.Close()
        )
        
        cancelButton.Click.Add(fun _ -> dialog.Close())
        
        dialog.Content <- mainPanel
        dialog.ShowDialog(this) |> ignore
    
    member private this.OnAddToCart() =
        match selectedProduct with
        | Some product ->
            let quantity = int quantityBox.Value.Value
            match Cart.addToCart cart product quantity with
            | Success newCart ->
                cart <- newCart
                this.UpdateCartDisplay()
                this.UpdateStatus $"? Added {quantity} x {product.Name} to cart"
            | Error msg ->
                this.UpdateStatus $"? Error: {msg}"
        | None ->
            this.UpdateStatus "Please select a product first"
    
    member private this.OnRemoveFromCart() =
        match selectedCartItem with
        | Some item ->
            cart <- Cart.removeFromCart cart item.Product.Id
            selectedCartItem <- None
            this.UpdateCartDisplay()
            this.UpdateStatus $"? Removed {item.Product.Name} from cart"
        | None ->
            this.UpdateStatus "Please select a cart item to remove"
    
    member private this.OnUpdateQuantity() =
        match selectedCartItem with
        | Some item ->
            let dialog = Window(Title = "Update Quantity", Width = 300.0, Height = 200.0, WindowStartupLocation = WindowStartupLocation.CenterOwner)
            let panel = StackPanel(Margin = Thickness(20.0))
            
            panel.Children.Add(TextBlock(Text = $"Product: {item.Product.Name}", Margin = Thickness(0.0, 10.0))) |> ignore
            panel.Children.Add(TextBlock(Text = $"Current Quantity: {item.Quantity}", Margin = Thickness(0.0, 10.0))) |> ignore
            panel.Children.Add(TextBlock(Text = "New Quantity:", Margin = Thickness(0.0, 10.0))) |> ignore
            
            let newQtyBox = NumericUpDown(Minimum = 0m, Maximum = decimal item.Product.Stock, Value = Nullable(decimal item.Quantity), Width = 100.0)
            panel.Children.Add(newQtyBox) |> ignore
            
            let buttonPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 20.0), HorizontalAlignment = HorizontalAlignment.Center)
            let updateBtn = Button(Content = "Update", Padding = Thickness(20.0, 10.0), Margin = Thickness(5.0))
            let cancelBtn = Button(Content = "Cancel", Padding = Thickness(20.0, 10.0), Margin = Thickness(5.0))
            buttonPanel.Children.Add(updateBtn) |> ignore
            buttonPanel.Children.Add(cancelBtn) |> ignore
            panel.Children.Add(buttonPanel) |> ignore
            
            updateBtn.Click.Add(fun _ ->
                let newQty = int newQtyBox.Value.Value
                match Cart.updateQuantity cart item.Product.Id newQty with
                | Success newCart ->
                    cart <- newCart
                    this.UpdateCartDisplay()
                    if newQty = 0 then
                        this.UpdateStatus $"? Removed {item.Product.Name} from cart"
                    else
                        this.UpdateStatus $"? Updated {item.Product.Name} quantity to {newQty}"
                    dialog.Close()
                | Error msg ->
                    this.UpdateStatus $"? Error: {msg}"
            )
            
            cancelBtn.Click.Add(fun _ -> dialog.Close())
            
            dialog.Content <- panel
            dialog.ShowDialog(this) |> ignore
        | None ->
            this.UpdateStatus "Please select a cart item to update"
    
    member private this.OnClearCart() =
        if not (Cart.isEmpty cart) then
            cart <- Cart.empty
            selectedCartItem <- None
            this.UpdateCartDisplay()
            this.UpdateStatus "? Cart cleared"
    
    member private this.OnCheckout() =
        if Cart.isEmpty cart then
            this.UpdateStatus "Cart is empty!"
        else
            let subtotal = PriceCalculator.calculateSubtotal cart
            let discount = PriceCalculator.getAutomaticDiscount subtotal
            let breakdown = PriceCalculator.calculateBreakdown cart discount taxRate
            let receipt = FileManager.createReceipt cart breakdown
            let fileName = FileManager.generateFileName "receipt"
            
            match FileManager.saveReceipt receipt fileName with
            | Success msg ->
                cart <- Cart.empty
                selectedCartItem <- None
                this.UpdateCartDisplay()
                this.UpdateStatus $"? Checkout complete! {msg}"
            | Error msg ->
                this.UpdateStatus $"Checkout failed: {msg}"
    
    member private this.OnViewReceipts() =
        let receiptFiles = 
            Directory.GetFiles(".", "receipt_*.json")
            |> Array.sort
            |> Array.rev
            |> Array.toList
        
        if receiptFiles.IsEmpty then
            this.UpdateStatus "No receipts found. Complete a checkout to create a receipt."
        else
            let dialog = Window(Title = "Receipt History", Width = 500.0, Height = 600.0, WindowStartupLocation = WindowStartupLocation.CenterOwner)
            let mainPanel = DockPanel(Margin = Thickness(10.0))
            
            let listBox = ListBox(MinHeight = 400.0)
            for file in receiptFiles do
                let fileName = Path.GetFileName(file)
                listBox.Items.Add(fileName) |> ignore
            
            let detailsText = TextBox(IsReadOnly = true, MinHeight = 150.0, TextWrapping = TextWrapping.Wrap, Margin = Thickness(0.0, 10.0))
            DockPanel.SetDock(detailsText, Dock.Bottom)
            
            let closeButton = Button(Content = "Close", Padding = Thickness(20.0, 10.0), HorizontalAlignment = HorizontalAlignment.Center, Margin = Thickness(0.0, 10.0))
            DockPanel.SetDock(closeButton, Dock.Bottom)
            
            listBox.SelectionChanged.Add(fun _ =>
                if listBox.SelectedIndex >= 0 then
                    let selectedFile = receiptFiles.[listBox.SelectedIndex]
                    match FileManager.loadReceipt selectedFile with
                    | Success receipt ->
                        let details = 
                            $"Date: {receipt.Date:yyyy-MM-dd HH:mm:ss}\n\n" +
                            $"Items: {receipt.Items.Length}\n" +
                            String.concat "\n" [for item in receipt.Items -> 
                                $"  • {item.Product.Name} x{item.Quantity} @ ${item.Product.Price:F2}"] +
                            $"\n\nSubtotal: ${receipt.Subtotal:F2}\n" +
                            (if receipt.Discount > 0m then $"Discount: -${receipt.Discount:F2}\n" else "") +
                            $"Tax: ${receipt.Tax:F2}\n" +
                            $"TOTAL: ${receipt.Total:F2}"
                        detailsText.Text <- details
                    | Error msg ->
                        detailsText.Text <- $"Error loading receipt: {msg}"
            )
            
            closeButton.Click.Add(fun _ -> dialog.Close())
            
            mainPanel.Children.Add(detailsText) |> ignore
            mainPanel.Children.Add(closeButton) |> ignore
            mainPanel.Children.Add(listBox) |> ignore
            
            dialog.Content <- mainPanel
            dialog.ShowDialog(this) |> ignore
    
    member private this.UpdateProductDetails() =
        match selectedProduct with
        | Some product ->
            productNameLabel.Text <- product.Name
            productPriceLabel.Text <- $"Price: ${product.Price:F2}"
            productCategoryLabel.Text <- $"Category: {product.Category}"
            productStockLabel.Text <- $"Stock: {product.Stock}"
            quantityBox.Maximum <- decimal product.Stock
            addToCartButton.IsEnabled <- true
        | None ->
            productNameLabel.Text <- "No product selected"
            productPriceLabel.Text <- ""
            productCategoryLabel.Text <- ""
            productStockLabel.Text <- ""
            addToCartButton.IsEnabled <- false
    
    member private this.UpdateCartDisplay() =
        cartListBox.Items.Clear()
        
        if Cart.isEmpty cart then
            cartListBox.Items.Add("Cart is empty") |> ignore
            subtotalLabel.Text <- ""
            discountLabel.Text <- ""
            taxLabel.Text <- ""
            totalLabel.Text <- ""
            checkoutButton.IsEnabled <- false
            clearCartButton.IsEnabled <- false
        else
            for item in cart do
                let itemTotal = PriceCalculator.calculateItemTotal item
                cartListBox.Items.Add($"{item.Product.Name} x{item.Quantity} = ${itemTotal:F2}") |> ignore
            
            let subtotal = PriceCalculator.calculateSubtotal cart
            let discount = PriceCalculator.getAutomaticDiscount subtotal
            let breakdown = PriceCalculator.calculateBreakdown cart discount taxRate
            
            subtotalLabel.Text <- $"Subtotal: ${breakdown.Subtotal:F2}"
            discountLabel.Text <- if breakdown.Discount > 0m then $"Discount: -${breakdown.Discount:F2}" else ""
            taxLabel.Text <- $"Tax ({breakdown.TaxRate:F1}%): ${breakdown.Tax:F2}"
            totalLabel.Text <- $"TOTAL: ${breakdown.Total:F2}"
            checkoutButton.IsEnabled <- true
            clearCartButton.IsEnabled <- true
    
    member private this.UpdateStatus(message: string) =
        statusBar.Text <- message
