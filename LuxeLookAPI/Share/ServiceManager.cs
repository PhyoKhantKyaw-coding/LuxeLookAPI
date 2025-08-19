using LuxeLookAPI.Models;
using LuxeLookAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace LuxeLookAPI.Share
{
    public class ServiceManager
    {
        public static void SetServiceInfo(IServiceCollection services, AppSettings appSettings)
        {
            services.AddDbContextPool<DataContext>(options =>
            {
                options.UseSqlServer(appSettings.ConnectionString);
            });
            services.AddScoped<ProductService>();
            services.AddScoped<DeliveryService>();
            services.AddScoped<OrderService>();
            services.AddScoped<UserService>();
            services.AddScoped<CategoryService>();

        }
    }
}
