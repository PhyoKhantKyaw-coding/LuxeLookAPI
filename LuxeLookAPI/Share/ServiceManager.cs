using LuxeLookAPI.Models;
using LuxeLookAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace LuxeLookAPI.Share;

public class ServiceManager
{
    public static void SetServiceInfo(IServiceCollection services, AppSettings appSettings)
    {
        services.AddDbContextPool<DataContext>(options =>
        {
            options.UseSqlServer(appSettings.ConnectionString);
        });
        //services.AddDbContextPool<DataContext>(options =>
        //{
        //    options.UseMySql(appSettings.ConnectionString,
        //                     ServerVersion.AutoDetect(appSettings.ConnectionString));
        //});
        services.AddScoped<ProductService>();
        services.AddScoped<OrderService>();
        services.AddScoped<UserService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<CommonTokenReader>();
        services.AddScoped<DashboardService>();
        services.AddScoped<BookingService>();

    }
}
