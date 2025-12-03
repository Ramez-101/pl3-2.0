module SimpleGui

open System
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
    let taxRate = 8.5m
    
    // Controls
    let productListBox = ListBox(Height = 400.0, MinWidth = 250.0)
    let searchBox = TextBox(Watermark = "Search products...", MinWidth = 200.0)
    let searchButton = Button(Content = "Search", Padding = Thickness(10.0, 5.0))
    let showAllButton = Button(Content = "Show All", Padding = Thickness(10.0, 5.0))
    
    let productNameLabel = TextBlock(FontSize = 18.0, FontWeight = FontWeight.Bold)
    let productPriceLabel = TextBlock(FontSize = 14.0)
    let productCategoryLabel = TextBlock(FontSize = 14.0)
    let productStockLabel = TextBlock(FontSize = 14.0)
    let quantityBox = NumericUpDown(Minimum = 1.0m, Maximum = 100.0m, Value = System.Nullable<decimal>(1.0m), Width = 100.0)
    let addToCartButton = Button(Content = "Add to Cart", Padding = Thickness(20.0, 10.0), FontSize = 14.0)
    
    let cartListBox = ListBox(Height = 300.0, MinWidth = 250.0)
    let subtotalLabel = TextBlock(FontSize = 14.0)
    let discountLabel = TextBlock(FontSize = 14.0, Foreground = Brushes.Green)
    let taxLabel = TextBlock(FontSize = 14.0)
    let totalLabel = TextBlock(FontSize = 18.0, FontWeight = FontWeight.Bold)
    let checkoutButton = Button(Content = "Checkout", Padding = Thickness(30.0, 12.0), FontSize = 16.0)
    
    let statusBar = TextBlock(Background = Brushes.LightBlue, Padding = Thickness(10.0), FontSize = 12.0)
    
    do
        this.Title <- "Simple Store Simulator"
        this.Width <- 1000.0
        this.Height <- 600.0
        this.MinWidth <- 800.0
        this.MinHeight <- 500.0
        
        this.LoadProducts()
        this.SetupUI()
        this.AttachEventHandlers()
        this.UpdateProductDetails()
        this.UpdateCartDisplay()
        this.UpdateStatus "Welcome to Store Simulator!"
    
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
        
        // Left Panel - Product List
        let leftPanel = StackPanel(Margin = Thickness(10.0))
        leftPanel.Children.Add(TextBlock(Text = "Products", FontSize = 16.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 10.0))) |> ignore
        
        let searchPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 0.0, 0.0, 10.0))
        searchPanel.Children.Add(searchBox) |> ignore
        searchPanel.Children.Add(searchButton) |> ignore
        searchPanel.Children.Add(showAllButton) |> ignore
        leftPanel.Children.Add(searchPanel) |> ignore
        
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
        
        // Right Panel - Cart
        let rightPanel = StackPanel(Margin = Thickness(10.0))
        rightPanel.Children.Add(TextBlock(Text = "Shopping Cart", FontSize = 16.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 10.0))) |> ignore
        rightPanel.Children.Add(cartListBox) |> ignore
        
        let pricePanel = StackPanel(Margin = Thickness(0.0, 10.0))
        pricePanel.Children.Add(TextBlock(Text = "Price Breakdown", FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 5.0))) |> ignore
        pricePanel.Children.Add(subtotalLabel) |> ignore
        pricePanel.Children.Add(discountLabel) |> ignore
        pricePanel.Children.Add(taxLabel) |> ignore
        pricePanel.Children.Add(totalLabel) |> ignore
        pricePanel.Children.Add(checkoutButton) |> ignore
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
        addToCartButton.Click.Add(fun _ -> this.OnAddToCart())
        checkoutButton.Click.Add(fun _ -> this.OnCheckout())
    
    member private this.OnProductSelected() =
        if productListBox.SelectedIndex >= 0 then
            let products = Catalog.getAllProducts catalog
            if productListBox.SelectedIndex < products.Length then
                selectedProduct <- Some products.[productListBox.SelectedIndex]
                this.UpdateProductDetails()
    
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
    
    member private this.OnShowAll() =
        searchBox.Text <- ""
        this.LoadProducts()
    
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
                this.UpdateCartDisplay()
                this.UpdateStatus $"? Checkout complete! {msg}"
            | Error msg ->
                this.UpdateStatus $"Checkout failed: {msg}"
    
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
        else
            for item in cart do
                let itemTotal = PriceCalculator.calculateItemTotal item
                cartListBox.Items.Add($"{item.Product.Name} x{item.Quantity} = ${itemTotal:F2}") |> ignore
            
            let subtotal = PriceCalculator.calculateSubtotal cart
            let discount = PriceCalculator.getAutomaticDiscount subtotal
            let breakdown = PriceCalculator.calculateBreakdown cart discount taxRate
            
            subtotalLabel.Text <- $"Subtotal: ${breakdown.Subtotal:F2}"
            discountLabel.Text <- if breakdown.Discount > 0m then $"Discount: -${breakdown.Discount:F2}" else ""
            taxLabel.Text <- $"Tax ({breakdown.TaxRate:F1}%%): ${breakdown.Tax:F2}"
            totalLabel.Text <- $"TOTAL: ${breakdown.Total:F2}"
            checkoutButton.IsEnabled <- true
    
    member private this.UpdateStatus(message: string) =
        statusBar.Text <- message
