using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplicationShoppp.Models;

namespace WebApplicationShoppp.Data
{
    public class ProductDbContext : IdentityDbContext<IdentityUser>
    {
    

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
            

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany() 
                .HasForeignKey(o => o.ProductId);

            modelBuilder.Entity<Order>()
        .HasOne(o => o.User)
        .WithMany()  
        .HasForeignKey(o => o.UserId)
        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
