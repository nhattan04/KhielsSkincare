using KhielsSkincare.Models;
using Microsoft.EntityFrameworkCore;

namespace KhielsSkincare.Repository
{
    public class KhielsContext : DbContext
    {
        public KhielsContext(DbContextOptions<KhielsContext> options)
            : base(options) { }

        
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
           
    }
}
