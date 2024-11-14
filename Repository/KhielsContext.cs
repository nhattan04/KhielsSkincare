using KhielsSkincare.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KhielsSkincare.Repository
{
    public class KhielsContext : IdentityDbContext<AppUser>
    {
        public KhielsContext(DbContextOptions<KhielsContext> options): base(options) { }

        
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }     
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ProductQuantity> ProductQuantities { get; set; }
        public DbSet<ShippingFee> ShippingFees { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
        public DbSet<Statistical> Statisticals { get; set; }

    }
}
