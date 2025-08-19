namespace LuxeLookAPI.DTO
{
    public class UpdateProductDTO
    {
        public Guid? ProductId { get; set; }
        public Guid? CatId { get; set; }
        public string? ProductName { get; set; }
        public int? StockQTY { get; set; }
        public string? BrandName { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Price { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}
