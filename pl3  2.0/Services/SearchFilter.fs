module StoreSimulator.Services.SearchFilter

open StoreSimulator.Core

/// Filter products by name (case-insensitive)
let filterByName (products: Product list) (searchTerm: string) : Product list =
    let lowerSearchTerm = searchTerm.ToLower()
    products |> List.filter (fun p -> p.Name.ToLower().Contains(lowerSearchTerm))

/// Filter products by category
let filterByCategory (products: Product list) (category: string) : Product list =
    products |> List.filter (fun p -> p.Category.Equals(category, System.StringComparison.OrdinalIgnoreCase))

/// Filter products by brand
let filterByBrand (products: Product list) (brand: string) : Product list =
    products |> List.filter (fun p -> p.Brand.Equals(brand, System.StringComparison.OrdinalIgnoreCase))

/// Filter products by price range
let filterByPriceRange (products: Product list) (minPrice: decimal) (maxPrice: decimal) : Product list =
    products |> List.filter (fun p -> p.Price >= minPrice && p.Price <= maxPrice)

/// Filter products by stock availability
let filterInStock (products: Product list) : Product list =
    products |> List.filter (fun p -> p.Stock > 0 && p.IsAvailable)

/// Filter products by minimum rating
let filterByRating (products: Product list) (minRating: decimal) : Product list =
    products |> List.filter (fun p -> p.Rating >= minRating)

/// Filter by tag
let filterByTag (products: Product list) (tag: string) : Product list =
    let lowerTag = tag.ToLower()
    products |> List.filter (fun p -> p.Tags |> List.exists (fun t -> t.ToLower().Contains(lowerTag)))

/// Search in name and description
let searchProducts (products: Product list) (searchTerm: string) : Product list =
    let lowerTerm = searchTerm.ToLower()
    products |> List.filter (fun p -> 
        p.Name.ToLower().Contains(lowerTerm) || 
        p.Description.ToLower().Contains(lowerTerm) ||
        p.Brand.ToLower().Contains(lowerTerm) ||
        p.Tags |> List.exists (fun t -> t.ToLower().Contains(lowerTerm)))

/// Get all unique categories
let getCategories (products: Product list) : string list =
    products |> List.map (fun p -> p.Category) |> List.distinct |> List.sort

/// Get all unique brands
let getBrands (products: Product list) : string list =
    products |> List.map (fun p -> p.Brand) |> List.distinct |> List.sort

/// Get all unique tags
let getTags (products: Product list) : string list =
    products |> List.collect (fun p -> p.Tags) |> List.distinct |> List.sort

/// Sort products by price (ascending)
let sortByPriceAsc (products: Product list) : Product list =
    products |> List.sortBy (fun p -> p.Price)

/// Sort products by price (descending)
let sortByPriceDesc (products: Product list) : Product list =
    products |> List.sortByDescending (fun p -> p.Price)

/// Sort products by name
let sortByName (products: Product list) : Product list =
    products |> List.sortBy (fun p -> p.Name)

/// Sort products by rating (descending)
let sortByRating (products: Product list) : Product list =
    products |> List.sortByDescending (fun p -> p.Rating)

/// Sort products by stock
let sortByStock (products: Product list) : Product list =
    products |> List.sortBy (fun p -> p.Stock)

/// Combined search
let search (products: Product list) 
           (nameQuery: string option) 
           (category: string option) 
           (minPrice: decimal option) 
           (maxPrice: decimal option) : Product list =
    products
    |> fun p -> match nameQuery with Some query -> searchProducts p query | None -> p
    |> fun p -> match category with Some cat -> filterByCategory p cat | None -> p
    |> fun p ->
        match minPrice, maxPrice with
        | Some min, Some max -> filterByPriceRange p min max
        | Some min, None -> List.filter (fun prod -> prod.Price >= min) p
        | None, Some max -> List.filter (fun prod -> prod.Price <= max) p
        | None, None -> p

/// Advanced search with all filters
let advancedSearch 
    (products: Product list) 
    (query: string option)
    (category: string option)
    (brand: string option)
    (minPrice: decimal option)
    (maxPrice: decimal option)
    (minRating: decimal option)
    (inStockOnly: bool) : Product list =
    
    products
    |> fun p -> match query with Some q when not (System.String.IsNullOrWhiteSpace(q)) -> searchProducts p q | _ -> p
    |> fun p -> match category with Some cat when not (System.String.IsNullOrWhiteSpace(cat)) -> filterByCategory p cat | _ -> p
    |> fun p -> match brand with Some b when not (System.String.IsNullOrWhiteSpace(b)) -> filterByBrand p b | _ -> p
    |> fun p ->
        match minPrice, maxPrice with
        | Some min, Some max -> filterByPriceRange p min max
        | Some min, None -> List.filter (fun prod -> prod.Price >= min) p
        | None, Some max -> List.filter (fun prod -> prod.Price <= max) p
        | None, None -> p
    |> fun p -> match minRating with Some rating -> filterByRating p rating | None -> p
    |> fun p -> if inStockOnly then filterInStock p else p
