using LuxeLookAPI.Models;
using LuxeLookAPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LuxeLookAPI.Services
{
    public class DashboardService
    {
        private readonly DataContext _context;

        public DashboardService(DataContext context)
        {
            _context = context;
        }

        public async Task<DashboardDTO> GetDashboardDataAsync()
        {
            DateTime now = DateTime.UtcNow;

            // Fetch orders and users once to reduce multiple queries
            var activeOrders = await _context.Orders
                .Where(o => o.ActiveFlag)
                .ToListAsync();

            var activeUsers = await _context.Users
                .Where(u => u.ActiveFlag)
                .ToListAsync();

            // Metric Cards
            var totalRevenue = activeOrders.Sum(o => o.TotalAmount ?? 0);
            var newCustomers = activeUsers.Count(u => u.CreatedAt >= now.AddDays(-30));
            var activeAccounts = activeUsers.Count;

            var previousRevenue = activeOrders
                .Where(o => o.OrderDate < now.AddDays(-30))
                .Sum(o => o.TotalAmount ?? 0);

            var growthRate = previousRevenue > 0
                ? ((totalRevenue - previousRevenue) / previousRevenue * 100)
                : 0;

            // Bar Chart: Sales over last 30 days (limit 10 points)
            var barChartData = activeOrders
                .Where(o => o.OrderDate >= now.AddDays(-30))
                .GroupBy(o => o.OrderDate!.Value.Date)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key.ToString("MMM d", CultureInfo.InvariantCulture),
                    Value = g.Sum(o => o.TotalAmount ?? 0)
                })
                .OrderBy(g => g.Label)
                .Take(10)
                .ToList();

            // Area Chart: Orders over last 6 months
            var areaChartData = new List<AreaChartData>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var monthOrders = activeOrders
                    .Where(o => o.OrderDate >= monthStart && o.OrderDate <= monthEnd)
                    .ToList();

                areaChartData.Add(new AreaChartData
                {
                    Month = monthStart.ToString("MMM", CultureInfo.InvariantCulture),
                    TotalOrders = monthOrders.Count,
                    CompletedOrders = monthOrders.Count(o => o.Status?.ToLower() == "completed")
                });
            }

            // Donut Chart: Order status distribution
            var donutChartData = activeOrders
                .GroupBy(o => o.Status ?? "Unknown")
                .Select(g => new DonutChartData
                {
                    Label = g.Key,
                    Value = g.Count()
                })
                .ToList();

            // Recent Sales: Last 5 orders
            var recentSales = activeOrders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Join(
                    activeUsers,
                    order => order.UserId,
                    user => user.UserId,
                    (order, user) => new RecentSale
                    {
                        Name = user.UserName ?? "Unknown",
                        Email = user.Email ?? "N/A",
                        Amount = (order.TotalAmount ?? 0).ToString("C"), // formatted as currency
                        Avatar = user.ProfileImageUrl ?? "https://i.pravatar.cc/150?img=1"
                    })
                .ToList();

            return new DashboardDTO
            {
                TotalRevenue = totalRevenue,
                NewCustomers = newCustomers,
                ActiveAccounts = activeAccounts,
                GrowthRate = Math.Round(growthRate, 1),
                BarChartData = barChartData,
                AreaChartData = areaChartData,
                DonutChartData = donutChartData,
                RecentSales = recentSales
            };
        }
    }
}
