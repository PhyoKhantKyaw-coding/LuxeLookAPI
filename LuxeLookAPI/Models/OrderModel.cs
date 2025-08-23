using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models;

[Table("tblOrder")]
public class OrderModel : CommonFields
{
    [Key]
    public Guid? OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? OrderPlace { get; set; }
    public string? OrderStartPoint { get; set; }
    public string? OrderEndPoint { get; set; }
    public decimal? TotalAmount { get; set; }
    public int? TotalQTY { get; set; }
    public decimal? TotalProfit { get; set; }
    public decimal? TotalCost { get; set; }
    public string? Status { get; set; } // ordered, delivering, completed, rejected, accepted
    public Guid? UserId { get; set; }
    public Guid? DeliveryId { get; set; }
    public Guid? PaymentId { get; set; }
}
