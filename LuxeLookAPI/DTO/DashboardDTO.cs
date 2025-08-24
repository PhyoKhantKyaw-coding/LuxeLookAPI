namespace LuxeLookAPI.DTO
{
    public class DashboardDTO
    {
        public int? TotalUsers { get; set; }
        public int? TotalProducts { get; set; }
        public int? TotalOrders { get; set; }
        public int? TotalCategories { get; set; }
        public int? TotalBrands { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? TotalProfit { get; set; }
    }
}
