namespace LuxeLookAPI.DTO;

public class GetProductDTO
{
    public Guid? ProductId { get; set; }
    public string? CatInstanceName { get; set; }
    public string? BrandName { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public int? StockQTY { get; set; }
    public decimal? Cost { get; set; }
    public decimal? Price { get; set; }
    public string? CurrencySymbol { get; set; }
    public string? ProductImageUrl { get; set; }
    public string? SupplierName { get; set; }
}
public class GetProductByCatIdDTO
{
    public Guid? CatId { get; set; }
    public List<GetProductDTO>? Products { get; set; }
}
public class GetProductWithCatInstanceByCatIDDTO 
{
    public Guid? CatInstanceId { get; set; }
    public string? CatInstanceName { get; set; }
    public List<GetProductDTO2>? Products { get; set; }
}
public class GetProductDTO2
{
    public Guid? productID { get; set; }
    public String? productName { get; set; }
}
