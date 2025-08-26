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

            // Query all active orders
            var orders = await _context.Orders
                .Where(o => o.ActiveFlag)
                .ToListAsync();

            // Calculate metrics
            var totalRevenue = orders.Sum(o => o.TotalAmount ?? 0);
            var newCustomers = await _context.Users
                .CountAsync(u => u.ActiveFlag && u.CreatedAt >= now.AddDays(-30)); // Last 30 days for new customers
            var activeAccounts = await _context.Users.CountAsync(u => u.ActiveFlag);
            var previousOrders = await _context.Orders
                .Where(o => o.OrderDate < now.AddDays(-30) && o.ActiveFlag)
                .SumAsync(o => o.TotalAmount ?? 0);
            var growthRate = previousOrders > 0 ? ((totalRevenue - previousOrders) / previousOrders * 100) : 0;

            // Bar Chart: Sales over recent dates (last 30 days, limited to 10 points)
            var barChartData = await _context.Orders
                .Where(o => o.OrderDate >= now.Date.AddDays(-30) && o.OrderDate <= now && o.ActiveFlag)
                .GroupBy(o => o.OrderDate!.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Value = g.Sum(o => o.TotalAmount ?? 0)
                })
                .OrderBy(g => g.Date)
                .Take(10)
                .ToListAsync();

            // Format the date on the client side
            var formattedBarChartData = barChartData.Select(g => new ChartDataPoint
            {
                Label = g.Date.ToString("MMM d", CultureInfo.InvariantCulture),
                Value = g.Value
            }).ToList();

            // Area Chart: Total and completed orders over last 6 months
            var areaChartData = new List<AreaChartData>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var monthOrders = await _context.Orders
                    .Where(o => o.OrderDate >= monthStart && o.OrderDate <= monthEnd && o.ActiveFlag)
                    .ToListAsync();
                areaChartData.Add(new AreaChartData
                {
                    Month = monthStart.ToString("MMM", CultureInfo.InvariantCulture),
                    TotalOrders = monthOrders.Count,
                    CompletedOrders = monthOrders.Count(o => o.Status?.ToLower() == "completed")
                });
            }

            // Donut Chart: Order status distribution (all orders)
            var donutChartData = await _context.Orders
                .Where(o => o.ActiveFlag)
                .GroupBy(o => o.Status)
                .Select(g => new DonutChartData
                {
                    Label = g.Key ?? "Unknown",
                    Value = g.Count()
                })
                .ToListAsync();

            // Recent Sales: Last 5 orders with user details using manual join
            var recentSales = await _context.Orders
                .Where(o => o.ActiveFlag)
                .Join(
                    _context.Users,
                    order => order.UserId,
                    user => user.UserId,
                    (order, user) => new { Order = order, User = user }
                )
                .OrderByDescending(o => o.Order.OrderDate)
                .Take(5)
                .Select(o => new RecentSale
                {
                    Name = o.User.UserName ?? "Unknown",
                    Email = o.User.Email ?? "N/A",
                    Amount = o.Order.TotalAmount.ToString(),
                    Avatar = o.User.ProfileImageUrl ?? "https://i.pravatar.cc/150?img=1"
                })
                .ToListAsync();

            // Construct DTO
            var dto = new DashboardDTO
            {
                TotalRevenue = totalRevenue,
                NewCustomers = newCustomers,
                ActiveAccounts = activeAccounts,
                GrowthRate = Math.Round(growthRate, 1),
                BarChartData = formattedBarChartData,
                AreaChartData = areaChartData,
                DonutChartData = donutChartData,
                RecentSales = recentSales
            };

            return dto;
        }
    }
}