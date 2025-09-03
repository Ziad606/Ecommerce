using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Models.Auth.UserTokens;
using Ecommerce.Entities.Models.Reviews;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ecommerce.DataAccess.ApplicationContext
{
    public class AuthContext : IdentityDbContext<User, Role, string>, IDataProtectionKeyContext
    {
        public AuthContext(DbContextOptions<AuthContext> options)
             : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Advertisement> Advertisements { get; set; }
    }
}
