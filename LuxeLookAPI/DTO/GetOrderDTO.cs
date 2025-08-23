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
