using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models
{
    [Table("tblProduct")]
    public class ProductModel : CommonFields
    {
        [Key]
        public Guid? ProductId { get; set; }
        public Guid? CatInstanceId { get; set; }
        public Guid? BrandId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? StockQTY { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Price { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}
