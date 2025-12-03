module SearchFilter

open Types

// Search & Filter Developer - Product filtering features

// Filter products by name (case-insensitive)
let filterByName (products: Product list) (searchTerm: string) : Product list =
    let lowerSearchTerm = searchTerm.ToLower()
    products 
    |> List.filter (fun p -> p.Name.ToLower().Contains(lowerSearchTerm))

// Filter products by category
let filterByCategory (products: Product list) (category: string) : Product list =
    products 
    |> List.filter (fun p -> p.Category.Equals(category, System.StringComparison.OrdinalIgnoreCase))

// Filter products by price range
let filterByPriceRange (products: Product list) (minPrice: decimal) (maxPrice: decimal) : Product list =
    products 
    |> List.filter (fun p -> p.Price >= minPrice && p.Price <= maxPrice)

// Filter products by stock availability
let filterInStock (products: Product list) : Product list =
    products 
    |> List.filter (fun p -> p.Stock > 0)

// Get all unique categories
let getCategories (products: Product list) : string list =
    products 
    |> List.map (fun p -> p.Category)
    |> List.distinct
    |> List.sort

// Sort products by price (ascending)
let sortByPriceAsc (products: Product list) : Product list =
    products |> List.sortBy (fun p -> p.Price)

// Sort products by price (descending)
let sortByPriceDesc (products: Product list) : Product list =
    products |> List.sortByDescending (fun p -> p.Price)

// Sort products by name
let sortByName (products: Product list) : Product list =
    products |> List.sortBy (fun p -> p.Name)

// Combined search: name, category, and price range
let search (products: Product list) 
           (nameQuery: string option) 
           (category: string option) 
           (minPrice: decimal option) 
           (maxPrice: decimal option) : Product list =
    products
    |> fun p -> 
        match nameQuery with
        | Some query -> filterByName p query
        | None -> p
    |> fun p ->
        match category with
        | Some cat -> filterByCategory p cat
        | None -> p
    |> fun p ->
        match minPrice, maxPrice with
        | Some min, Some max -> filterByPriceRange p min max
        | Some min, None -> List.filter (fun prod -> prod.Price >= min) p
        | None, Some max -> List.filter (fun prod -> prod.Price <= max) p
        | None, None -> p
