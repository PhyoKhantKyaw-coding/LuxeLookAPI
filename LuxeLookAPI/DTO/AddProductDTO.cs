namespace LuxeLookAPI.DTO;

public class AddProductDTO
{
    public Guid? CatInstanceId { get; set; }
    public Guid? BrandId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public int? StockQTY { get; set; }
    public decimal? Cost { get; set; }
    public decimal? Price { get; set; }
    public string? ProductImageUrl { get; set; }
    public Guid? SupplierId { get; set; }
}
