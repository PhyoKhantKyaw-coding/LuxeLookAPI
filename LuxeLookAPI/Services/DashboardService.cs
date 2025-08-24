using LuxeLookAPI.Models;
using LuxeLookAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace LuxeLookAPI.Services
{
    public class DashboardService
    {
        private readonly DataContext _context;

        public DashboardService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<DashboardDTO>> GetDashboardDataAsync(string status, string? monthName = null)
        {
            DateTime now = DateTime.UtcNow;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = now;

            // filter by status
            switch (status?.ToLower())
            {
                case "day":
                    startDate = now.Date;
                    break;

                case "week":
                    startDate = now.Date.AddDays(-7);
                    break;

                case "month":
                    if (!string.IsNullOrWhiteSpace(monthName))
                    {
                        // filter by given month name
                        if (DateTime.TryParseExact(monthName, "MMMM",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime parsedMonth))
                        {
                            startDate = new DateTime(now.Year, parsedMonth.Month, 1);
                            endDate = startDate.AddMonths(1).AddDays(-1);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid month name.");
                        }
                    }
                    else
                    {
                        // current month
                        startDate = new DateTime(now.Year, now.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);
                    }
                    break;

                case "all":
                default:
                    startDate = DateTime.MinValue;
                    break;
            }

            // query orders in range
            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.ActiveFlag)
                .ToListAsync();

            var dto = new DashboardDTO
            {
                TotalUsers = await _context.Users.CountAsync(u => u.ActiveFlag),
                TotalProducts = await _context.Products.CountAsync(p => p.ActiveFlag),
                TotalOrders = orders.Count,
                TotalCategories = await _context.Categories.CountAsync(c => c.ActiveFlag),
                TotalBrands = await _context.Brands.CountAsync(),
                TotalSales = orders.Sum(o => o.TotalAmount ?? 0),
                TotalCost = orders.Sum(o => o.TotalCost ?? 0),
                TotalProfit = orders.Sum(o => o.TotalProfit ?? 0)
            };

            return new List<DashboardDTO> { dto };
        }
    }
}
