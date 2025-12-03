module Cart

open Types

// Cart Logic Developer - Add/remove operations with immutable lists

// Create an empty cart
let empty : CartItem list = []

// Add a product to cart (immutable operation)
let addToCart (cart: CartItem list) (product: Product) (quantity: int) : StoreResult<CartItem list> =
    if quantity <= 0 then
        Error "Quantity must be greater than 0"
    elif quantity > product.Stock then
        Error $"Not enough stock. Available: {product.Stock}"
    else
        // Check if product already exists in cart
        match List.tryFind (fun item -> item.Product.Id = product.Id) cart with
        | Some existingItem ->
            // Update quantity of existing item
            let newQuantity = existingItem.Quantity + quantity
            if newQuantity > product.Stock then
                Error $"Total quantity ({newQuantity}) exceeds available stock ({product.Stock})"
            else
                let updatedCart = 
                    cart 
                    |> List.map (fun item -> 
                        if item.Product.Id = product.Id then 
                            { item with Quantity = newQuantity }
                        else 
                            item)
                Success updatedCart
        | None ->
            // Add new item to cart
            let newItem = { Product = product; Quantity = quantity }
            Success (newItem :: cart)

// Remove a product from cart completely
let removeFromCart (cart: CartItem list) (productId: int) : CartItem list =
    cart |> List.filter (fun item -> item.Product.Id <> productId)

// Update quantity of a product in cart
let updateQuantity (cart: CartItem list) (productId: int) (newQuantity: int) : StoreResult<CartItem list> =
    if newQuantity <= 0 then
        // Remove item if quantity is 0 or negative
        Success (removeFromCart cart productId)
    else
        match List.tryFind (fun item -> item.Product.Id = productId) cart with
        | Some item ->
            if newQuantity > item.Product.Stock then
                Error $"Quantity ({newQuantity}) exceeds available stock ({item.Product.Stock})"
            else
                let updatedCart = 
                    cart 
                    |> List.map (fun i -> 
                        if i.Product.Id = productId then 
                            { i with Quantity = newQuantity }
                        else 
                            i)
                Success updatedCart
        | None ->
            Error "Product not found in cart"

// Get total number of items in cart
let getItemCount (cart: CartItem list) : int =
    cart |> List.sumBy (fun item -> item.Quantity)

// Check if cart is empty
let isEmpty (cart: CartItem list) : bool =
    List.isEmpty cart
