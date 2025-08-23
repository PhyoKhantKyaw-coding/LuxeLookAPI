using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models;

[Table("tblAddToCart")]
public class AddToCartModel : CommonFields
{
    [Key]
    public Guid? ATCId { get; set; }
    public Guid? ProductId { get; set; }
    public int? Qty { get; set; }
    public decimal? Amount { get; set; }
    public Guid? UserId { get; set; }
}
