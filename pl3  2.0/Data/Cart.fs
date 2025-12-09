module StoreSimulator.Data.Cart

open StoreSimulator.Core
open StoreSimulator.Data.Catalog

/// Create an empty cart
let empty : CartItem list = []

/// Add a product to cart (immutable operation)
let addToCart (cart: CartItem list) (product: Product) (quantity: int) : StoreResult<CartItem list> =
    if quantity <= 0 then
        Error "Quantity must be greater than 0"
    elif not product.IsAvailable then
        Error (sprintf "%s is currently unavailable" product.Name)
    elif quantity > product.Stock then
        Error (sprintf "Not enough stock. Available: %d" product.Stock)
    else
        match List.tryFind (fun item -> item.Product.Id = product.Id) cart with
        | Some existingItem ->
            let newQuantity = existingItem.Quantity + quantity
            if newQuantity > product.Stock then
                Error (sprintf "Total quantity (%d) exceeds available stock (%d)" newQuantity product.Stock)
            else
                let updatedCart = 
                    cart |> List.map (fun item -> 
                        if item.Product.Id = product.Id then { item with Quantity = newQuantity }
                        else item)
                Success updatedCart
        | None ->
            let newItem = { Product = product; Quantity = quantity }
            Success (newItem :: cart)

/// Remove a product from cart completely
let removeFromCart (cart: CartItem list) (productId: int) : CartItem list =
    cart |> List.filter (fun item -> item.Product.Id <> productId)

/// Update quantity of a product in cart
let updateQuantity (cart: CartItem list) (productId: int) (newQuantity: int) : StoreResult<CartItem list> =
    if newQuantity <= 0 then
        Success (removeFromCart cart productId)
    else
        match List.tryFind (fun item -> item.Product.Id = productId) cart with
        | Some item ->
            if newQuantity > item.Product.Stock then
                Error (sprintf "Quantity (%d) exceeds available stock (%d)" newQuantity item.Product.Stock)
            else
                let updatedCart = 
                    cart |> List.map (fun i -> 
                        if i.Product.Id = productId then { i with Quantity = newQuantity }
                        else i)
                Success updatedCart
        | None ->
            Error "Product not found in cart"

/// Get total number of items in cart
let getItemCount (cart: CartItem list) : int =
    cart |> List.sumBy (fun item -> item.Quantity)

/// Check if cart is empty
let isEmpty (cart: CartItem list) : bool =
    List.isEmpty cart

/// Process checkout - decrease stock for all items in cart
let processCheckout (cart: CartItem list) (catalog: Map<int, Product>) : StoreResult<Map<int, Product>> =
    let validationResult = 
        cart |> List.fold (fun acc item ->
            match acc with
            | Error msg -> Error msg
            | Success _ ->
                match Map.tryFind item.Product.Id catalog with
                | Some product ->
                    if product.Stock >= item.Quantity then Success ()
                    else Error (sprintf "Not enough stock for %s. Available: %d, In cart: %d" product.Name product.Stock item.Quantity)
                | None ->
                    Error (sprintf "Product %s no longer exists" item.Product.Name)
        ) (Success ())
    
    match validationResult with
    | Error msg -> Error msg
    | Success _ ->
        let updatedCatalog = 
            cart |> List.fold (fun cat item ->
                match Catalog.decreaseStock cat item.Product.Id item.Quantity with
                | Success newCat -> newCat
                | Error _ -> cat
            ) catalog
        Success updatedCatalog

/// Get cart total value
let getCartTotal (cart: CartItem list) : decimal =
    cart |> List.sumBy (fun item -> item.Product.Price * decimal item.Quantity)

/// Validate cart against current catalog
let validateCart (cart: CartItem list) (catalog: Map<int, Product>) : StoreResult<CartItem list> =
    let errors = 
        cart |> List.choose (fun item ->
            match Map.tryFind item.Product.Id catalog with
            | Some product ->
                if not product.IsAvailable then
                    Some (sprintf "%s is no longer available" product.Name)
                elif product.Stock < item.Quantity then
                    Some (sprintf "Only %d of %s available (you have %d in cart)" product.Stock product.Name item.Quantity)
                else None
            | None ->
                Some (sprintf "%s no longer exists in catalog" item.Product.Name)
        )
    
    if errors.IsEmpty then Success cart
    else Error (String.concat "\n" errors)
