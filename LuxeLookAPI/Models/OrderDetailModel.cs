using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models;

[Table("tblOrderDetail")]
public class OrderDetailModel : CommonFields
{
    [Key]
    public Guid? OrderDetailId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public int? Qty { get; set; }
    public decimal? Price { get; set; }
    public decimal? Cost { get; set; }
    public decimal? Profit { get; set; }
}
