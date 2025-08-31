namespace LuxeLookAPI.DTO;

public class GetOrderDTO
{
    public Guid? OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? OrderPlace { get; set; }
    public string? OrderStartPoint { get; set; }
    public string? OrderEndPoint { get; set; }
    public decimal? TotalAmount { get; set; }
    public int? TotalQTY { get; set; }
    public decimal? TotalProfit { get; set; }
    public decimal? TotalCost { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? DeliveryName { get; set; }
    public string? PaymentType { get; set; }
    public decimal? PaymentAmount { get; set; }
    public decimal? DeliFee { get; set; }
    public string? PaymentStatus { get; set; }
}
public class OrderWithVoucherDTO
{
    public Guid? OrderId { get; set; }                // The order ID
    public string VoucherCode { get; set; }           // Voucher code
    public decimal? DiscountAmount { get; set; }      // Fixed discount applied to the order
    public decimal? DiscountPercent { get; set; }     // Percentage discount applied to the order
    public string? Description { get; set; }          // Optional description of the voucher

    // Order total information
    public decimal? TotalAmount { get; set; }         // Total order amount before discount
    public decimal? FinalAmount => TotalAmount.HasValue && DiscountAmount.HasValue
                                    ? TotalAmount.Value - DiscountAmount.Value
                                    : TotalAmount; // Final amount after discount

    // List of order details (products) affected by the voucher
    public List<OrderDetailWithVoucherDTO> OrderDetails { get; set; } = new List<OrderDetailWithVoucherDTO>();
}

// DTO representing individual order details (products) in the voucher
public class OrderDetailWithVoucherDTO
{
    public Guid? OrderDetailId { get; set; }          // Order detail ID
    public string ProductName { get; set; }           // Product name
    public int Qty { get; set; }                      // Quantity ordered
    public decimal Price { get; set; }                // Price per product
    public decimal? DiscountAmount { get; set; }      // Discount applied to this product
    public decimal FinalPrice => Price * Qty - (DiscountAmount ?? 0); // Final price after discount
}
