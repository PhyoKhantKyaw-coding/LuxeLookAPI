using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace LuxeLookAPI.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ProductModel> Products { get; set; }
    public DbSet<CategoryModel> Categories { get; set; }
    public DbSet<OrderDetailModel> OrderDetails { get; set; }
    public DbSet<OrderModel> Orders { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<PaymentModel> Payments { get; set; }
    public DbSet<CategoryInstance> CategoryInstances { get; set; }
    public DbSet<BrandModel> Brands { get; set; }
    public DbSet<FavoriteModel> Favorites { get; set; }
    public DbSet<AddToCartModel> AddToCarts { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }


}
