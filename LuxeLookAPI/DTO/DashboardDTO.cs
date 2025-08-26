namespace LuxeLookAPI.DTO
{
    public class DashboardDTO
    {
        // Metric Cards
        public decimal? TotalRevenue { get; set; }
        public int? NewCustomers { get; set; }
        public int? ActiveAccounts { get; set; }
        public decimal? GrowthRate { get; set; }

        // Bar Chart: Sales over recent dates
        public List<ChartDataPoint> BarChartData { get; set; } = new List<ChartDataPoint>();

        // Area Chart: Total and completed orders over months
        public List<AreaChartData> AreaChartData { get; set; } = new List<AreaChartData>();

        // Donut Chart: Order status distribution
        public List<DonutChartData> DonutChartData { get; set; } = new List<DonutChartData>();

        // Recent Sales
        public List<RecentSale> RecentSales { get; set; } = new List<RecentSale>();
    }

    public class ChartDataPoint
    {
        public string? Label { get; set; } // e.g., "Apr 8"
        public decimal? Value { get; set; } // Sales amount
    }

    public class AreaChartData
    {
        public string? Month { get; set; } // e.g., "Jan"
        public int? TotalOrders { get; set; }
        public int? CompletedOrders { get; set; }
    }

    public class DonutChartData
    {
        public string? Label { get; set; } // Status like "ordered", "completed"
        public int? Value { get; set; } // Count of orders
    }

    public class RecentSale
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Amount { get; set; } // Formatted as currency
        public string? Avatar { get; set; } // Profile image URL
    }
}