module PriceCalculator

open Types

// Price Calculator - Compute totals and discounts

// Calculate subtotal for a single cart item
let calculateItemTotal (item: CartItem) : decimal =
    item.Product.Price * decimal item.Quantity

// Calculate cart subtotal (before discounts)
let calculateSubtotal (cart: CartItem list) : decimal =
    cart 
    |> List.sumBy calculateItemTotal

// Apply discount to a total
let applyDiscount (subtotal: decimal) (discount: DiscountType) : decimal =
    match discount with
    | NoDiscount -> subtotal
    | PercentageOff percentage ->
        subtotal - (subtotal * percentage / 100m)
    | BuyXGetYFree (buy, free) ->
        // This is a simplified implementation
        // In real scenario, this would need item-level logic
        subtotal

// Calculate tax (simple percentage)
let calculateTax (subtotal: decimal) (taxRate: decimal) : decimal =
    subtotal * taxRate / 100m

// Calculate final total
let calculateTotal (cart: CartItem list) (discount: DiscountType) (taxRate: decimal) : decimal =
    let subtotal = calculateSubtotal cart
    let afterDiscount = applyDiscount subtotal discount
    let tax = calculateTax afterDiscount taxRate
    afterDiscount + tax

// Create a detailed price breakdown
type PriceBreakdown = {
    Subtotal: decimal
    Discount: decimal
    TaxRate: decimal
    Tax: decimal
    Total: decimal
}

let calculateBreakdown (cart: CartItem list) (discount: DiscountType) (taxRate: decimal) : PriceBreakdown =
    let subtotal = calculateSubtotal cart
    let afterDiscount = applyDiscount subtotal discount
    let discountAmount = subtotal - afterDiscount
    let tax = calculateTax afterDiscount taxRate
    let total = afterDiscount + tax
    
    {
        Subtotal = subtotal
        Discount = discountAmount
        TaxRate = taxRate
        Tax = tax
        Total = total
    }

// Simple discount for orders over certain amount
let getAutomaticDiscount (subtotal: decimal) : DiscountType =
    if subtotal >= 500m then
        PercentageOff 10m // 10% off for orders over $500
    elif subtotal >= 200m then
        PercentageOff 5m  // 5% off for orders over $200
    else
        NoDiscount
