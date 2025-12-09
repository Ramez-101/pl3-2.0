module StoreSimulator.UI.StoreGui

open System
open System.IO
open Avalonia
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Controls.Primitives
open StoreSimulator.Core
open StoreSimulator.Services
open StoreSimulator.Data

module UserManager = StoreSimulator.Services.UserManager
module Catalog = StoreSimulator.Data.Catalog
module Cart = StoreSimulator.Data.Cart
module PriceCalculator = StoreSimulator.Services.PriceCalculator
module SearchFilter = StoreSimulator.Services.SearchFilter
module FileManager = StoreSimulator.Data.FileManager

/// Color scheme for professional look
module Colors =
    let Primary = Color.FromRgb(59uy, 130uy, 246uy)
    let PrimaryDark = Color.FromRgb(37uy, 99uy, 235uy)
    let Success = Color.FromRgb(34uy, 197uy, 94uy)
    let Warning = Color.FromRgb(251uy, 191uy, 36uy)
    let Danger = Color.FromRgb(239uy, 68uy, 68uy)
    let Background = Color.FromRgb(249uy, 250uy, 251uy)
    let Surface = Color.FromRgb(255uy, 255uy, 255uy)
    let TextPrimary = Color.FromRgb(17uy, 24uy, 39uy)
    let TextSecondary = Color.FromRgb(107uy, 114uy, 128uy)
    let Border = Color.FromRgb(229uy, 231uy, 235uy)

/// Login Window
type LoginWindow() as this =
    inherit Window()
    
    let mutable loginResult: UserSession option = None
    let usernameBox = TextBox(Watermark = "Username", Width = 280.0, Height = 40.0, FontSize = 14.0)
    let passwordBox = TextBox(Watermark = "Password", Width = 280.0, Height = 40.0, FontSize = 14.0, PasswordChar = '*')
    let statusLabel = TextBlock(FontSize = 12.0, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center)
    
    do
        this.Title <- "Store Simulator - Login"
        this.Width <- 450.0
        this.Height <- 500.0
        this.WindowStartupLocation <- WindowStartupLocation.CenterScreen
        this.Background <- SolidColorBrush(Colors.Background)
        
        let mainPanel = StackPanel(Margin = Thickness(40.0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center)
        
        mainPanel.Children.Add(TextBlock(Text = "Store Simulator", FontSize = 28.0, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, Margin = Thickness(0.0, 0.0, 0.0, 5.0))) |> ignore
        mainPanel.Children.Add(TextBlock(Text = "Professional Shopping Experience", FontSize = 14.0, Foreground = SolidColorBrush(Colors.TextSecondary), HorizontalAlignment = HorizontalAlignment.Center, Margin = Thickness(0.0, 0.0, 0.0, 30.0))) |> ignore
        
        mainPanel.Children.Add(TextBlock(Text = "Username", FontSize = 12.0, Margin = Thickness(0.0, 0.0, 0.0, 5.0))) |> ignore
        mainPanel.Children.Add(usernameBox) |> ignore
        mainPanel.Children.Add(TextBlock(Text = "Password", FontSize = 12.0, Margin = Thickness(0.0, 15.0, 0.0, 5.0))) |> ignore
        mainPanel.Children.Add(passwordBox) |> ignore
        
        let loginBtn = Button(Content = "Sign In", Width = 280.0, Height = 45.0, Margin = Thickness(0.0, 25.0, 0.0, 0.0), Background = SolidColorBrush(Colors.Primary), Foreground = Brushes.White, FontSize = 16.0, FontWeight = FontWeight.SemiBold)
        loginBtn.Click.Add(fun _ -> this.HandleLogin())
        mainPanel.Children.Add(loginBtn) |> ignore
        
        statusLabel.Margin <- Thickness(0.0, 15.0, 0.0, 0.0)
        mainPanel.Children.Add(statusLabel) |> ignore
        mainPanel.Children.Add(TextBlock(Text = "Demo: admin/admin123 or customer/customer123", FontSize = 10.0, Foreground = SolidColorBrush(Colors.TextSecondary), HorizontalAlignment = HorizontalAlignment.Center, Margin = Thickness(0.0, 10.0, 0.0, 0.0))) |> ignore
        
        let quickSection = StackPanel(Margin = Thickness(0.0, 30.0, 0.0, 0.0))
        quickSection.Children.Add(TextBlock(Text = "--- Quick Access ---", FontSize = 12.0, Foreground = SolidColorBrush(Colors.TextSecondary), HorizontalAlignment = HorizontalAlignment.Center, Margin = Thickness(0.0, 0.0, 0.0, 15.0))) |> ignore
        
        let quickButtonPanel = StackPanel(Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center)
        let adminQuickBtn = Button(Content = "Admin Mode", Width = 130.0, Height = 40.0, Margin = Thickness(5.0), Background = SolidColorBrush(Color.FromRgb(99uy, 102uy, 241uy)), Foreground = Brushes.White, FontSize = 12.0)
        adminQuickBtn.Click.Add(fun _ -> this.QuickLogin Admin)
        quickButtonPanel.Children.Add(adminQuickBtn) |> ignore
        
        let customerQuickBtn = Button(Content = "Customer Mode", Width = 130.0, Height = 40.0, Margin = Thickness(5.0), Background = SolidColorBrush(Colors.Success), Foreground = Brushes.White, FontSize = 12.0)
        customerQuickBtn.Click.Add(fun _ -> this.QuickLogin Customer)
        quickButtonPanel.Children.Add(customerQuickBtn) |> ignore
        
        quickSection.Children.Add(quickButtonPanel) |> ignore
        mainPanel.Children.Add(quickSection) |> ignore
        this.Content <- mainPanel
    
    member private this.HandleLogin() =
        let username = usernameBox.Text
        let password = passwordBox.Text
        if String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password) then
            statusLabel.Foreground <- SolidColorBrush(Colors.Danger)
            statusLabel.Text <- "Please enter username and password"
        else
            let request: LoginRequest = { Username = username; Password = password }
            match UserManager.login request with
            | Success session ->
                loginResult <- Some session
                statusLabel.Foreground <- SolidColorBrush(Colors.Success)
                statusLabel.Text <- "Login successful"
                let storeWindow = StoreWindow(session)
                storeWindow.Show()
                this.Close()
            | Error msg ->
                statusLabel.Foreground <- SolidColorBrush(Colors.Danger)
                statusLabel.Text <- msg
                passwordBox.Text <- ""
    
    member private this.QuickLogin(mode: UserMode) =
        let session = UserManager.quickLogin mode
        loginResult <- Some session
        let storeWindow = StoreWindow(session)
        storeWindow.Show()
        this.Close()
    
    member this.GetLoginResult() = loginResult

and StoreWindow(session: UserSession) as this =
    inherit Window()
    
    let mutable catalog = Catalog.initializeCatalog()
    let mutable cart = Cart.empty
    let mutable selectedProduct: Product option = None
    let mutable selectedCartItem: CartItem option = None
    let taxRate = 8.5m
    let userSession = session
    
    let productListBox = ListBox(MinHeight = 400.0)
    let searchBox = TextBox(Watermark = "Search products...", Width = 250.0, Height = 35.0)
    let categoryCombo = ComboBox(Width = 150.0, Height = 35.0)
    let brandCombo = ComboBox(Width = 150.0, Height = 35.0)
    
    let productNameLabel = TextBlock(FontSize = 22.0, FontWeight = FontWeight.Bold, TextWrapping = TextWrapping.Wrap)
    let productBrandLabel = TextBlock(FontSize = 14.0, Foreground = SolidColorBrush(Colors.TextSecondary))
    let productPriceLabel = TextBlock(FontSize = 24.0, FontWeight = FontWeight.Bold, Foreground = SolidColorBrush(Colors.Primary))
    let productDescLabel = TextBlock(FontSize = 13.0, TextWrapping = TextWrapping.Wrap, Foreground = SolidColorBrush(Colors.TextSecondary))
    let productRatingLabel = TextBlock(FontSize = 14.0)
    let productStockLabel = TextBlock(FontSize = 14.0)
    let productCategoryLabel = TextBlock(FontSize = 12.0, Foreground = SolidColorBrush(Colors.TextSecondary))
    let quantityBox = NumericUpDown(Minimum = 1.0m, Maximum = 100.0m, Value = Nullable<decimal>(1.0m), Width = 100.0, Height = 35.0)
    let addToCartButton = Button(Content = "Add to Cart", Height = 45.0, FontSize = 16.0, Background = SolidColorBrush(Colors.Primary), Foreground = Brushes.White)
    
    let cartListBox = ListBox(MinHeight = 200.0)
    let cartCountLabel = TextBlock(FontSize = 14.0, FontWeight = FontWeight.SemiBold)
    let subtotalLabel = TextBlock(FontSize = 14.0)
    let discountLabel = TextBlock(FontSize = 14.0, Foreground = SolidColorBrush(Colors.Success))
    let taxLabel = TextBlock(FontSize = 14.0)
    let totalLabel = TextBlock(FontSize = 20.0, FontWeight = FontWeight.Bold)
    let checkoutButton = Button(Content = "Checkout", Height = 50.0, FontSize = 18.0, Background = SolidColorBrush(Colors.Success), Foreground = Brushes.White)
    let clearCartButton = Button(Content = "Clear", Height = 35.0, FontSize = 12.0)
    
    let statusBar = TextBlock(FontSize = 12.0, Padding = Thickness(15.0, 10.0))
    let userInfoLabel = TextBlock(FontSize = 12.0, Padding = Thickness(15.0, 10.0))
    
    do
        let modeStr = if UserManager.isAdmin userSession then "ADMIN" else "CUSTOMER"
        this.Title <- sprintf "Store Simulator - %s (%s)" modeStr userSession.User.Username
        this.Width <- 1300.0
        this.Height <- 750.0
        this.MinWidth <- 1000.0
        this.MinHeight <- 600.0
        this.Background <- SolidColorBrush(Colors.Background)
        this.WindowStartupLocation <- WindowStartupLocation.CenterScreen
        
        UserManager.initialize()
        this.SetupUI()
        this.LoadProducts()
        this.LoadFilters()
        this.AttachEventHandlers()
        this.UpdateProductDetails()
        this.UpdateCartDisplay()
        this.UpdateStatus (sprintf "Welcome, %s (%s Mode)" userSession.User.FullName modeStr)
    
    member private this.SetupUI() =
        let mainGrid = Grid()
        mainGrid.RowDefinitions.Add(RowDefinition(Height = GridLength.Auto))
        mainGrid.RowDefinitions.Add(RowDefinition(Height = GridLength(1.0, GridUnitType.Star)))
        mainGrid.RowDefinitions.Add(RowDefinition(Height = GridLength.Auto))
        
        let header = this.CreateHeader()
        Grid.SetRow(header, 0)
        mainGrid.Children.Add(header) |> ignore
        
        let contentGrid = Grid(Margin = Thickness(15.0))
        contentGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(1.0, GridUnitType.Star)))
        contentGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(350.0, GridUnitType.Pixel)))
        contentGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(320.0, GridUnitType.Pixel)))
        
        let productsPanel = this.CreateProductsPanel()
        Grid.SetColumn(productsPanel, 0)
        contentGrid.Children.Add(productsPanel) |> ignore
        
        let detailsPanel = this.CreateDetailsPanel()
        Grid.SetColumn(detailsPanel, 1)
        contentGrid.Children.Add(detailsPanel) |> ignore
        
        let cartPanel = this.CreateCartPanel()
        Grid.SetColumn(cartPanel, 2)
        contentGrid.Children.Add(cartPanel) |> ignore
        
        Grid.SetRow(contentGrid, 1)
        mainGrid.Children.Add(contentGrid) |> ignore
        
        let statusBarPanel = this.CreateStatusBar()
        Grid.SetRow(statusBarPanel, 2)
        mainGrid.Children.Add(statusBarPanel) |> ignore
        
        this.Content <- mainGrid
    
    member private this.CreateHeader() =
        let header = Border(Background = SolidColorBrush(Colors.Surface), Padding = Thickness(20.0, 15.0), BorderBrush = SolidColorBrush(Colors.Border), BorderThickness = Thickness(0.0, 0.0, 0.0, 1.0))
        let headerGrid = Grid()
        headerGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength.Auto))
        headerGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(1.0, GridUnitType.Star)))
        headerGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength.Auto))
        
        let logo = TextBlock(Text = "Store Simulator", FontSize = 22.0, FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center)
        Grid.SetColumn(logo, 0)
        headerGrid.Children.Add(logo) |> ignore
        
        let searchPanel = StackPanel(Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center)
        searchPanel.Children.Add(searchBox) |> ignore
        searchPanel.Children.Add(TextBlock(Text = "  Category: ", VerticalAlignment = VerticalAlignment.Center)) |> ignore
        searchPanel.Children.Add(categoryCombo) |> ignore
        searchPanel.Children.Add(TextBlock(Text = "  Brand: ", VerticalAlignment = VerticalAlignment.Center)) |> ignore
        searchPanel.Children.Add(brandCombo) |> ignore
        
        let searchBtn = Button(Content = "Search", Margin = Thickness(10.0, 0.0, 0.0, 0.0), Padding = Thickness(15.0, 8.0))
        searchBtn.Click.Add(fun _ -> this.ApplyFilters())
        searchPanel.Children.Add(searchBtn) |> ignore
        
        let clearBtn = Button(Content = "Clear", Margin = Thickness(5.0, 0.0, 0.0, 0.0), Padding = Thickness(10.0, 8.0))
        clearBtn.Click.Add(fun _ -> this.ClearFilters())
        searchPanel.Children.Add(clearBtn) |> ignore
        
        Grid.SetColumn(searchPanel, 1)
        headerGrid.Children.Add(searchPanel) |> ignore
        
        let userPanel = StackPanel(Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center)
        let modeStr = if UserManager.isAdmin userSession then "Admin" else "Customer"
        userPanel.Children.Add(TextBlock(Text = sprintf "%s: %s" modeStr userSession.User.Username, FontSize = 12.0, VerticalAlignment = VerticalAlignment.Center)) |> ignore
        
        let logoutBtn = Button(Content = "Logout", Margin = Thickness(10.0, 0.0, 0.0, 0.0), Padding = Thickness(10.0, 5.0))
        logoutBtn.Click.Add(fun _ -> this.Logout())
        userPanel.Children.Add(logoutBtn) |> ignore
        
        Grid.SetColumn(userPanel, 2)
        headerGrid.Children.Add(userPanel) |> ignore
        
        header.Child <- headerGrid
        header
    
    member private this.CreateProductsPanel() =
        let card = Border(Background = SolidColorBrush(Colors.Surface), CornerRadius = CornerRadius(8.0), Padding = Thickness(15.0), Margin = Thickness(0.0, 0.0, 10.0, 0.0), BorderBrush = SolidColorBrush(Colors.Border), BorderThickness = Thickness(1.0))
        let panel = StackPanel()
        panel.Children.Add(TextBlock(Text = "Products", FontSize = 18.0, FontWeight = FontWeight.SemiBold, Margin = Thickness(0.0, 0.0, 0.0, 10.0))) |> ignore
        let scroll = ScrollViewer(MaxHeight = 550.0)
        scroll.Content <- productListBox
        panel.Children.Add(scroll) |> ignore
        card.Child <- panel
        card
    
    member private this.CreateDetailsPanel() =
        let card = Border(Background = SolidColorBrush(Colors.Surface), CornerRadius = CornerRadius(8.0), Padding = Thickness(20.0), Margin = Thickness(5.0, 0.0, 5.0, 0.0), BorderBrush = SolidColorBrush(Colors.Border), BorderThickness = Thickness(1.0))
        let panel = StackPanel()
        panel.Children.Add(TextBlock(Text = "Product Details", FontSize = 18.0, FontWeight = FontWeight.SemiBold, Margin = Thickness(0.0, 0.0, 0.0, 15.0))) |> ignore
        panel.Children.Add(productNameLabel) |> ignore
        panel.Children.Add(productBrandLabel) |> ignore
        productPriceLabel.Margin <- Thickness(0.0, 10.0, 0.0, 10.0)
        panel.Children.Add(productPriceLabel) |> ignore
        panel.Children.Add(productRatingLabel) |> ignore
        panel.Children.Add(productStockLabel) |> ignore
        panel.Children.Add(productCategoryLabel) |> ignore
        productDescLabel.Margin <- Thickness(0.0, 15.0, 0.0, 10.0)
        panel.Children.Add(productDescLabel) |> ignore
        
        let qtyPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 10.0, 0.0, 15.0))
        qtyPanel.Children.Add(TextBlock(Text = "Quantity: ", VerticalAlignment = VerticalAlignment.Center, FontSize = 14.0)) |> ignore
        qtyPanel.Children.Add(quantityBox) |> ignore
        panel.Children.Add(qtyPanel) |> ignore
        
        addToCartButton.Margin <- Thickness(0.0, 5.0, 0.0, 0.0)
        panel.Children.Add(addToCartButton) |> ignore
        card.Child <- panel
        card
    
    member private this.CreateCartPanel() =
        let card = Border(Background = SolidColorBrush(Colors.Surface), CornerRadius = CornerRadius(8.0), Margin = Thickness(10.0, 0.0, 0.0, 0.0), BorderBrush = SolidColorBrush(Colors.Border), BorderThickness = Thickness(1.0))
        let scrollViewer = ScrollViewer(Padding = Thickness(15.0))
        let panel = StackPanel()
        let isAdmin = UserManager.isAdmin userSession
        
        let headerPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 0.0, 0.0, 10.0))
        headerPanel.Children.Add(TextBlock(Text = "Shopping Cart", FontSize = 18.0, FontWeight = FontWeight.SemiBold)) |> ignore
        headerPanel.Children.Add(cartCountLabel) |> ignore
        panel.Children.Add(headerPanel) |> ignore
        
        let cartScroll = ScrollViewer(MaxHeight = 150.0)
        cartScroll.Content <- cartListBox
        panel.Children.Add(cartScroll) |> ignore
        
        let actionsPanel = StackPanel(Orientation = Orientation.Horizontal, Margin = Thickness(0.0, 10.0, 0.0, 0.0))
        let removeBtn = Button(Content = "Remove", Margin = Thickness(0.0, 0.0, 5.0, 0.0), Padding = Thickness(10.0, 5.0))
        removeBtn.Click.Add(fun _ -> this.RemoveSelectedItem())
        actionsPanel.Children.Add(removeBtn) |> ignore
        clearCartButton.Margin <- Thickness(5.0, 0.0, 0.0, 0.0)
        clearCartButton.Click.Add(fun _ -> this.ClearCart())
        actionsPanel.Children.Add(clearCartButton) |> ignore
        panel.Children.Add(actionsPanel) |> ignore
        
        let pricePanel = Border(Background = SolidColorBrush(Colors.Background), CornerRadius = CornerRadius(6.0), Padding = Thickness(12.0), Margin = Thickness(0.0, 10.0, 0.0, 0.0))
        let priceStack = StackPanel()
        priceStack.Children.Add(TextBlock(Text = "Price Summary", FontWeight = FontWeight.SemiBold, Margin = Thickness(0.0, 0.0, 0.0, 5.0))) |> ignore
        priceStack.Children.Add(subtotalLabel) |> ignore
        priceStack.Children.Add(discountLabel) |> ignore
        priceStack.Children.Add(taxLabel) |> ignore
        totalLabel.Margin <- Thickness(0.0, 5.0, 0.0, 0.0)
        totalLabel.Foreground <- SolidColorBrush(Colors.Primary)
        priceStack.Children.Add(totalLabel) |> ignore
        pricePanel.Child <- priceStack
        panel.Children.Add(pricePanel) |> ignore
        
        checkoutButton.Margin <- Thickness(0.0, 10.0, 0.0, 0.0)
        checkoutButton.Click.Add(fun _ -> this.Checkout())
        panel.Children.Add(checkoutButton) |> ignore
        
        let receiptsBtn = Button(Content = "View Receipts", Margin = Thickness(0.0, 8.0, 0.0, 0.0), Padding = Thickness(10.0, 8.0), HorizontalAlignment = HorizontalAlignment.Stretch)
        receiptsBtn.Click.Add(fun _ -> this.ViewReceipts())
        panel.Children.Add(receiptsBtn) |> ignore
        
        if isAdmin then
            let adminSection = Border(Background = SolidColorBrush(Color.FromRgb(238uy, 242uy, 255uy)), CornerRadius = CornerRadius(6.0), Padding = Thickness(12.0), Margin = Thickness(0.0, 12.0, 0.0, 0.0))
            let adminStack = StackPanel()
            adminStack.Children.Add(TextBlock(Text = "Admin Panel", FontWeight = FontWeight.SemiBold, Margin = Thickness(0.0, 0.0, 0.0, 8.0), Foreground = SolidColorBrush(Color.FromRgb(99uy, 102uy, 241uy)))) |> ignore
            
            let addProductBtn = Button(Content = "Add New Product", Margin = Thickness(0.0, 4.0, 0.0, 0.0), Padding = Thickness(10.0, 8.0), HorizontalAlignment = HorizontalAlignment.Stretch, Background = SolidColorBrush(Color.FromRgb(99uy, 102uy, 241uy)), Foreground = Brushes.White)
            addProductBtn.Click.Add(fun _ -> this.ShowAddProductDialog())
            adminStack.Children.Add(addProductBtn) |> ignore
            
            let ordersBtn = Button(Content = "View All Orders", Margin = Thickness(0.0, 4.0, 0.0, 0.0), Padding = Thickness(10.0, 8.0), HorizontalAlignment = HorizontalAlignment.Stretch, Background = SolidColorBrush(Color.FromRgb(34uy, 197uy, 94uy)), Foreground = Brushes.White)
            ordersBtn.Click.Add(fun _ -> this.ShowOrdersDialog())
            adminStack.Children.Add(ordersBtn) |> ignore
            
            let lowStockBtn = Button(Content = "Low Stock Alert", Margin = Thickness(0.0, 4.0, 0.0, 0.0), Padding = Thickness(10.0, 8.0), HorizontalAlignment = HorizontalAlignment.Stretch, Background = SolidColorBrush(Color.FromRgb(251uy, 191uy, 36uy)), Foreground = Brushes.Black)
            lowStockBtn.Click.Add(fun _ -> this.ShowLowStockDialog())
            adminStack.Children.Add(lowStockBtn) |> ignore
            
            adminSection.Child <- adminStack
            panel.Children.Add(adminSection) |> ignore
        
        scrollViewer.Content <- panel
        card.Child <- scrollViewer
        card
    
    member private this.CreateStatusBar() =
        let bar = Border(Background = SolidColorBrush(Colors.Surface), BorderBrush = SolidColorBrush(Colors.Border), BorderThickness = Thickness(0.0, 1.0, 0.0, 0.0))
        let grid = Grid()
        grid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(1.0, GridUnitType.Star)))
        grid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength.Auto))
        Grid.SetColumn(statusBar, 0)
        grid.Children.Add(statusBar) |> ignore
        let timeStr = userSession.LoginTime.ToString("HH:mm")
        userInfoLabel.Text <- sprintf "Logged in as: %s | Session: %s" userSession.User.Username timeStr
        Grid.SetColumn(userInfoLabel, 1)
        grid.Children.Add(userInfoLabel) |> ignore
        bar.Child <- grid
        bar
    
    member private this.LoadProducts() =
        let products = Catalog.getAllProducts catalog
        productListBox.Items.Clear()
        for product in products do
            let stockColor = if product.Stock > 5 then "[OK]" elif product.Stock > 0 then "[LOW]" else "[OUT]"
            let stars = String.replicate (int product.Rating) "*"
            let text = sprintf "%s\n%s | $%.2f | %s Stock: %d | %s" product.Name product.Brand product.Price stockColor product.Stock stars
            let item = TextBlock(Text = text, FontSize = 12.0, Padding = Thickness(5.0), TextWrapping = TextWrapping.Wrap)
            productListBox.Items.Add(item) |> ignore
    
    member private this.LoadFilters() =
        categoryCombo.Items.Clear()
        categoryCombo.Items.Add("All Categories") |> ignore
        for cat in Catalog.getCategories catalog do categoryCombo.Items.Add(cat) |> ignore
        categoryCombo.SelectedIndex <- 0
        brandCombo.Items.Clear()
        brandCombo.Items.Add("All Brands") |> ignore
        for brand in Catalog.getBrands catalog do brandCombo.Items.Add(brand) |> ignore
        brandCombo.SelectedIndex <- 0
    
    member private this.AttachEventHandlers() =
        productListBox.SelectionChanged.Add(fun _ ->
            if productListBox.SelectedIndex >= 0 then
                let products = Catalog.getAllProducts catalog
                if productListBox.SelectedIndex < products.Length then
                    selectedProduct <- Some products.[productListBox.SelectedIndex]
                    this.UpdateProductDetails())
        addToCartButton.Click.Add(fun _ -> this.AddToCart())
        cartListBox.SelectionChanged.Add(fun _ ->
            if cartListBox.SelectedIndex >= 0 && not (Cart.isEmpty cart) then
                selectedCartItem <- Some cart.[cartListBox.SelectedIndex])
        searchBox.KeyDown.Add(fun e -> if e.Key = Avalonia.Input.Key.Enter then this.ApplyFilters())
    
    member private this.ApplyFilters() =
        let query = if String.IsNullOrWhiteSpace(searchBox.Text) then None else Some searchBox.Text
        let category = if categoryCombo.SelectedIndex > 0 then Some (categoryCombo.SelectedItem :?> string) else None
        let brand = if brandCombo.SelectedIndex > 0 then Some (brandCombo.SelectedItem :?> string) else None
        let filtered = SearchFilter.advancedSearch (Catalog.getAllProducts catalog) query category brand None None None true
        productListBox.Items.Clear()
        for product in filtered do
            let stockColor = if product.Stock > 5 then "[OK]" elif product.Stock > 0 then "[LOW]" else "[OUT]"
            let stars = String.replicate (int product.Rating) "*"
            let text = sprintf "%s\n%s | $%.2f | %s Stock: %d | %s" product.Name product.Brand product.Price stockColor product.Stock stars
            productListBox.Items.Add(TextBlock(Text = text, FontSize = 12.0, Padding = Thickness(5.0), TextWrapping = TextWrapping.Wrap)) |> ignore
        this.UpdateStatus (sprintf "Found %d products" filtered.Length)
    
    member private this.ClearFilters() =
        searchBox.Text <- ""
        categoryCombo.SelectedIndex <- 0
        brandCombo.SelectedIndex <- 0
        this.LoadProducts()
        this.UpdateStatus "Filters cleared"
    
    member private this.UpdateProductDetails() =
        match selectedProduct with
        | Some product ->
            productNameLabel.Text <- product.Name
            productBrandLabel.Text <- sprintf "by %s" product.Brand
            productPriceLabel.Text <- sprintf "$%.2f" product.Price
            let stars = String.replicate (int product.Rating) "*"
            productRatingLabel.Text <- sprintf "Rating: %s (%.1f/5)" stars product.Rating
            let stockStatus = if product.Stock > 5 then "In Stock" elif product.Stock > 0 then "Low Stock" else "Out of Stock"
            productStockLabel.Text <- sprintf "%s (%d available)" stockStatus product.Stock
            productCategoryLabel.Text <- sprintf "Category: %s" product.Category
            productDescLabel.Text <- product.Description
            quantityBox.Maximum <- decimal product.Stock
            addToCartButton.IsEnabled <- product.Stock > 0 && product.IsAvailable
        | None ->
            productNameLabel.Text <- "Select a product"
            productBrandLabel.Text <- ""
            productPriceLabel.Text <- ""
            productRatingLabel.Text <- ""
            productStockLabel.Text <- ""
            productCategoryLabel.Text <- ""
            productDescLabel.Text <- "Click on a product from the list to view details"
            addToCartButton.IsEnabled <- false
    
    member private this.AddToCart() =
        match selectedProduct with
        | Some product ->
            let quantity = int quantityBox.Value.Value
            match Cart.addToCart cart product quantity with
            | Success newCart ->
                cart <- newCart
                this.UpdateCartDisplay()
                this.UpdateStatus (sprintf "Added %dx %s to cart" quantity product.Name)
            | Error msg -> this.UpdateStatus (sprintf "Error: %s" msg)
        | None -> this.UpdateStatus "Please select a product first"
    
    member private this.UpdateCartDisplay() =
        cartListBox.Items.Clear()
        if Cart.isEmpty cart then
            cartListBox.Items.Add(TextBlock(Text = "Cart is empty", FontStyle = FontStyle.Italic, Foreground = SolidColorBrush(Colors.TextSecondary))) |> ignore
            cartCountLabel.Text <- ""
            subtotalLabel.Text <- ""
            discountLabel.Text <- ""
            taxLabel.Text <- ""
            totalLabel.Text <- ""
            checkoutButton.IsEnabled <- false
            clearCartButton.IsEnabled <- false
        else
            for item in cart do
                let itemTotal = item.Product.Price * decimal item.Quantity
                let text = sprintf "%s\n%dx $%.2f = $%.2f" item.Product.Name item.Quantity item.Product.Price itemTotal
                cartListBox.Items.Add(TextBlock(Text = text, FontSize = 12.0, Padding = Thickness(3.0))) |> ignore
            cartCountLabel.Text <- sprintf " (%d items)" (Cart.getItemCount cart)
            let subtotal = PriceCalculator.calculateSubtotal cart
            let discount = PriceCalculator.getAutomaticDiscount subtotal
            let breakdown = PriceCalculator.calculateBreakdown cart discount taxRate
            subtotalLabel.Text <- sprintf "Subtotal: $%.2f" breakdown.Subtotal
            discountLabel.Text <- if breakdown.Discount > 0m then sprintf "Discount: -$%.2f" breakdown.Discount else ""
            taxLabel.Text <- sprintf "Tax (8.5%%): $%.2f" breakdown.Tax
            totalLabel.Text <- sprintf "Total: $%.2f" breakdown.Total
            checkoutButton.IsEnabled <- true
            clearCartButton.IsEnabled <- true
    
    member private this.RemoveSelectedItem() =
        match selectedCartItem with
        | Some item ->
            cart <- Cart.removeFromCart cart item.Product.Id
            selectedCartItem <- None
            this.UpdateCartDisplay()
            this.UpdateStatus (sprintf "Removed %s from cart" item.Product.Name)
        | None -> this.UpdateStatus "Select an item to remove"
    
    member private this.ClearCart() =
        cart <- Cart.empty
        selectedCartItem <- None
        this.UpdateCartDisplay()
        this.UpdateStatus "Cart cleared"
    
    member private this.Checkout() =
        if Cart.isEmpty cart then this.UpdateStatus "Cart is empty"
        else
            match Cart.processCheckout cart catalog with
            | Success updatedCatalog ->
                catalog <- updatedCatalog
                let subtotal = PriceCalculator.calculateSubtotal cart
                let discount = PriceCalculator.getAutomaticDiscount subtotal
                let breakdown = PriceCalculator.calculateBreakdown cart discount taxRate
                let receipt = FileManager.createReceiptWithUser cart breakdown userSession.User
                let fileName = FileManager.generateFileName "receipt"
                match FileManager.saveReceipt receipt fileName with
                | Success _ ->
                    cart <- Cart.empty
                    selectedCartItem <- None
                    this.UpdateCartDisplay()
                    this.LoadProducts()
                    this.UpdateStatus (sprintf "Checkout complete - Order %s" receipt.OrderId)
                | Error msg -> this.UpdateStatus (sprintf "Checkout failed: %s" msg)
            | Error msg -> this.UpdateStatus (sprintf "Checkout failed: %s" msg)
    
    member private this.ViewReceipts() =
        let receiptFiles = Directory.GetFiles(".", "receipt_*.json") |> Array.sort |> Array.rev |> Array.toList
        if receiptFiles.IsEmpty then this.UpdateStatus "No receipts found"
        else
            let dialog = Window(Title = "Receipt History", Width = 600.0, Height = 500.0, WindowStartupLocation = WindowStartupLocation.CenterOwner)
            dialog.Background <- SolidColorBrush(Colors.Background)
            let panel = StackPanel(Margin = Thickness(20.0))
            panel.Children.Add(TextBlock(Text = "Receipt History", FontSize = 20.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 15.0))) |> ignore
            let listBox = ListBox(MinHeight = 300.0)
            for file in receiptFiles do listBox.Items.Add(Path.GetFileName(file)) |> ignore
            panel.Children.Add(listBox) |> ignore
            let detailsText = TextBox(IsReadOnly = true, MinHeight = 100.0, TextWrapping = TextWrapping.Wrap, Margin = Thickness(0.0, 10.0, 0.0, 0.0))
            panel.Children.Add(detailsText) |> ignore
            listBox.SelectionChanged.Add(fun _ ->
                if listBox.SelectedIndex >= 0 then
                    match FileManager.loadReceipt receiptFiles.[listBox.SelectedIndex] with
                    | Success receipt ->
                        let itemsList = receipt.Items |> List.map (fun item -> sprintf "  - %s x%d" item.Product.Name item.Quantity) |> String.concat "\n"
                        detailsText.Text <- sprintf "Date: %s\n\nItems:\n%s\n\nSubtotal: $%.2f\nDiscount: -$%.2f\nTax: $%.2f\nTotal: $%.2f" (receipt.Date.ToString("yyyy-MM-dd HH:mm")) itemsList receipt.Subtotal receipt.Discount receipt.Tax receipt.Total
                    | Error msg -> detailsText.Text <- sprintf "Error: %s" msg)
            let closeBtn = Button(Content = "Close", Padding = Thickness(20.0, 10.0), Margin = Thickness(0.0, 10.0, 0.0, 0.0), HorizontalAlignment = HorizontalAlignment.Right)
            closeBtn.Click.Add(fun _ -> dialog.Close())
            panel.Children.Add(closeBtn) |> ignore
            dialog.Content <- panel
            dialog.ShowDialog(this) |> ignore
    
    member private this.Logout() =
        let loginWindow = LoginWindow()
        loginWindow.Show()
        this.Close()
    
    member private this.UpdateStatus(message: string) = statusBar.Text <- message
    
    member private this.ShowAddProductDialog() =
        let dialog = Window(Title = "Add New Product", Width = 500.0, Height = 550.0, WindowStartupLocation = WindowStartupLocation.CenterOwner)
        dialog.Background <- SolidColorBrush(Colors.Background)
        let panel = StackPanel(Margin = Thickness(25.0))
        panel.Children.Add(TextBlock(Text = "Add New Product", FontSize = 20.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 20.0))) |> ignore
        let nameBox = TextBox(Watermark = "Product Name", Margin = Thickness(0.0, 0.0, 0.0, 10.0), Height = 35.0)
        panel.Children.Add(TextBlock(Text = "Name:", FontSize = 12.0)) |> ignore
        panel.Children.Add(nameBox) |> ignore
        let descBox = TextBox(Watermark = "Description", Margin = Thickness(0.0, 0.0, 0.0, 10.0), Height = 60.0, TextWrapping = TextWrapping.Wrap, AcceptsReturn = true)
        panel.Children.Add(TextBlock(Text = "Description:", FontSize = 12.0)) |> ignore
        panel.Children.Add(descBox) |> ignore
        let priceBox = NumericUpDown(Minimum = 0.01m, Maximum = 99999.99m, Value = Nullable<decimal>(9.99m), Margin = Thickness(0.0, 0.0, 0.0, 10.0), Height = 35.0, FormatString = "C2")
        panel.Children.Add(TextBlock(Text = "Price:", FontSize = 12.0)) |> ignore
        panel.Children.Add(priceBox) |> ignore
        let categoryBox = TextBox(Watermark = "Category", Margin = Thickness(0.0, 0.0, 0.0, 10.0), Height = 35.0)
        panel.Children.Add(TextBlock(Text = "Category:", FontSize = 12.0)) |> ignore
        panel.Children.Add(categoryBox) |> ignore
        let brandBox = TextBox(Watermark = "Brand Name", Margin = Thickness(0.0, 0.0, 0.0, 10.0), Height = 35.0)
        panel.Children.Add(TextBlock(Text = "Brand:", FontSize = 12.0)) |> ignore
        panel.Children.Add(brandBox) |> ignore
        let stockBox = NumericUpDown(Minimum = 0m, Maximum = 9999m, Value = Nullable<decimal>(10m), Margin = Thickness(0.0, 0.0, 0.0, 10.0), Height = 35.0)
        panel.Children.Add(TextBlock(Text = "Initial Stock:", FontSize = 12.0)) |> ignore
        panel.Children.Add(stockBox) |> ignore
        let statusLabel = TextBlock(FontSize = 12.0, Margin = Thickness(0.0, 10.0, 0.0, 0.0), TextWrapping = TextWrapping.Wrap)
        let buttonPanel = StackPanel(Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = Thickness(0.0, 20.0, 0.0, 0.0))
        let addBtn = Button(Content = "Add Product", Padding = Thickness(20.0, 10.0), Background = SolidColorBrush(Colors.Success), Foreground = Brushes.White, Margin = Thickness(0.0, 0.0, 10.0, 0.0))
        addBtn.Click.Add(fun _ ->
            let price = if priceBox.Value.HasValue then priceBox.Value.Value else 0m
            let stock = if stockBox.Value.HasValue then int stockBox.Value.Value else 0
            match Catalog.addNewProduct catalog nameBox.Text descBox.Text price categoryBox.Text stock brandBox.Text with
            | Success (newCatalog, newProduct) ->
                catalog <- newCatalog
                this.LoadProducts()
                this.LoadFilters()
                statusLabel.Foreground <- SolidColorBrush(Colors.Success)
                statusLabel.Text <- sprintf "Product '%s' added (ID: %d)" newProduct.Name newProduct.Id
                this.UpdateStatus (sprintf "Admin: Added '%s'" newProduct.Name)
                nameBox.Text <- ""; descBox.Text <- ""; categoryBox.Text <- ""; brandBox.Text <- ""
            | Error msg ->
                statusLabel.Foreground <- SolidColorBrush(Colors.Danger)
                statusLabel.Text <- msg)
        buttonPanel.Children.Add(addBtn) |> ignore
        let cancelBtn = Button(Content = "Close", Padding = Thickness(20.0, 10.0))
        cancelBtn.Click.Add(fun _ -> dialog.Close())
        buttonPanel.Children.Add(cancelBtn) |> ignore
        panel.Children.Add(buttonPanel) |> ignore
        panel.Children.Add(statusLabel) |> ignore
        dialog.Content <- panel
        dialog.ShowDialog(this) |> ignore
    
    member private this.ShowOrdersDialog() =
        let allOrders = FileManager.getAllReceipts()
        let dialog = Window(Title = "All Customer Orders", Width = 800.0, Height = 600.0, WindowStartupLocation = WindowStartupLocation.CenterOwner)
        dialog.Background <- SolidColorBrush(Colors.Background)
        let mainGrid = Grid()
        mainGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(1.0, GridUnitType.Star)))
        mainGrid.ColumnDefinitions.Add(ColumnDefinition(Width = GridLength(1.0, GridUnitType.Star)))
        let leftPanel = StackPanel(Margin = Thickness(20.0))
        leftPanel.Children.Add(TextBlock(Text = "Customer Orders", FontSize = 20.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 15.0))) |> ignore
        leftPanel.Children.Add(TextBlock(Text = sprintf "Total Orders: %d" allOrders.Length, FontSize = 14.0, Margin = Thickness(0.0, 0.0, 0.0, 10.0))) |> ignore
        let totalRevenue = allOrders |> List.sumBy (fun o -> o.Total)
        leftPanel.Children.Add(TextBlock(Text = sprintf "Total Revenue: $%.2f" totalRevenue, FontSize = 14.0, FontWeight = FontWeight.SemiBold, Foreground = SolidColorBrush(Colors.Success), Margin = Thickness(0.0, 0.0, 0.0, 15.0))) |> ignore
        let orderListBox = ListBox(MinHeight = 350.0)
        for order in allOrders do
            let itemCount = order.Items |> List.sumBy (fun i -> i.Quantity)
            orderListBox.Items.Add(TextBlock(Text = sprintf "%s\n%s | %d items | $%.2f" order.OrderId (order.Date.ToString("yyyy-MM-dd HH:mm")) itemCount order.Total, FontSize = 12.0, Padding = Thickness(5.0))) |> ignore
        leftPanel.Children.Add(orderListBox) |> ignore
        Grid.SetColumn(leftPanel, 0)
        mainGrid.Children.Add(leftPanel) |> ignore
        let rightPanel = StackPanel(Margin = Thickness(20.0))
        rightPanel.Children.Add(TextBlock(Text = "Order Details", FontSize = 20.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 15.0))) |> ignore
        let detailsText = TextBox(IsReadOnly = true, MinHeight = 300.0, TextWrapping = TextWrapping.Wrap, FontFamily = FontFamily("Consolas"))
        detailsText.Text <- "Select an order to view details"
        rightPanel.Children.Add(detailsText) |> ignore
        orderListBox.SelectionChanged.Add(fun _ ->
            if orderListBox.SelectedIndex >= 0 && orderListBox.SelectedIndex < allOrders.Length then
                let order = allOrders.[orderListBox.SelectedIndex]
                let itemsList = order.Items |> List.map (fun item -> sprintf "  %s x%d = $%.2f" item.Product.Name item.Quantity (item.Product.Price * decimal item.Quantity)) |> String.concat "\n"
                detailsText.Text <- sprintf "Order ID: %s\nDate: %s\nCustomer: %s\n\n--- Items ---\n%s\n\n--- Totals ---\nSubtotal: $%.2f\nDiscount: -$%.2f\nTax: $%.2f\nTotal: $%.2f" order.OrderId (order.Date.ToString("yyyy-MM-dd HH:mm:ss")) order.CustomerName itemsList order.Subtotal order.Discount order.Tax order.Total)
        let closeBtn = Button(Content = "Close", Padding = Thickness(20.0, 10.0), Margin = Thickness(0.0, 15.0, 0.0, 0.0), HorizontalAlignment = HorizontalAlignment.Right)
        closeBtn.Click.Add(fun _ -> dialog.Close())
        rightPanel.Children.Add(closeBtn) |> ignore
        Grid.SetColumn(rightPanel, 1)
        mainGrid.Children.Add(rightPanel) |> ignore
        dialog.Content <- mainGrid
        dialog.ShowDialog(this) |> ignore
    
    member private this.ShowLowStockDialog() =
        let lowStockProducts = Catalog.getLowStockProducts catalog 10
        let outOfStockProducts = Catalog.getOutOfStockProducts catalog
        let dialog = Window(Title = "Inventory Alerts", Width = 600.0, Height = 500.0, WindowStartupLocation = WindowStartupLocation.CenterOwner)
        dialog.Background <- SolidColorBrush(Colors.Background)
        let panel = StackPanel(Margin = Thickness(20.0))
        panel.Children.Add(TextBlock(Text = "Inventory Alerts", FontSize = 20.0, FontWeight = FontWeight.Bold, Margin = Thickness(0.0, 0.0, 0.0, 20.0))) |> ignore
        let outSection = Border(Background = SolidColorBrush(Color.FromRgb(254uy, 226uy, 226uy)), CornerRadius = CornerRadius(6.0), Padding = Thickness(12.0), Margin = Thickness(0.0, 0.0, 0.0, 15.0))
        let outStack = StackPanel()
        outStack.Children.Add(TextBlock(Text = sprintf "Out of Stock (%d products)" outOfStockProducts.Length, FontWeight = FontWeight.SemiBold, Foreground = SolidColorBrush(Colors.Danger))) |> ignore
        if outOfStockProducts.IsEmpty then outStack.Children.Add(TextBlock(Text = "No products out of stock", FontStyle = FontStyle.Italic, Margin = Thickness(0.0, 5.0, 0.0, 0.0))) |> ignore
        else for product in outOfStockProducts do outStack.Children.Add(TextBlock(Text = sprintf "  - %s (%s)" product.Name product.Brand, Margin = Thickness(0.0, 3.0, 0.0, 0.0))) |> ignore
        outSection.Child <- outStack
        panel.Children.Add(outSection) |> ignore
        let lowSection = Border(Background = SolidColorBrush(Color.FromRgb(254uy, 243uy, 199uy)), CornerRadius = CornerRadius(6.0), Padding = Thickness(12.0), Margin = Thickness(0.0, 0.0, 0.0, 15.0))
        let lowStack = StackPanel()
        lowStack.Children.Add(TextBlock(Text = sprintf "Low Stock (%d products)" lowStockProducts.Length, FontWeight = FontWeight.SemiBold, Foreground = SolidColorBrush(Color.FromRgb(180uy, 83uy, 9uy)))) |> ignore
        if lowStockProducts.IsEmpty then lowStack.Children.Add(TextBlock(Text = "No products with low stock", FontStyle = FontStyle.Italic, Margin = Thickness(0.0, 5.0, 0.0, 0.0))) |> ignore
        else for product in lowStockProducts do lowStack.Children.Add(TextBlock(Text = sprintf "  - %s (%s) - %d left" product.Name product.Brand product.Stock, Margin = Thickness(0.0, 3.0, 0.0, 0.0))) |> ignore
        lowSection.Child <- lowStack
        panel.Children.Add(lowSection) |> ignore
        let totalProducts = Catalog.getAllProducts catalog |> List.length
        panel.Children.Add(TextBlock(Text = sprintf "\nInventory Summary:\n  Total: %d | Healthy: %d | Low: %d | Out: %d" totalProducts (totalProducts - lowStockProducts.Length - outOfStockProducts.Length) lowStockProducts.Length outOfStockProducts.Length, FontSize = 13.0, Margin = Thickness(0.0, 10.0, 0.0, 0.0))) |> ignore
        let closeBtn = Button(Content = "Close", Padding = Thickness(20.0, 10.0), Margin = Thickness(0.0, 20.0, 0.0, 0.0), HorizontalAlignment = HorizontalAlignment.Right)
        closeBtn.Click.Add(fun _ -> dialog.Close())
        panel.Children.Add(closeBtn) |> ignore
        dialog.Content <- panel
        dialog.ShowDialog(this) |> ignore
