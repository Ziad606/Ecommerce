using Ecommerce.DataAccess.EntitiesConfigurations;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Models.Auth.UserTokens;
using Ecommerce.Entities.Models.Reviews;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.ApplyConfiguration(new UserEntityConfigurations());
            modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityConfigurations());
            modelBuilder.ApplyConfiguration(new ProductImageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new CartEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WishlistEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WishlistItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewEntityConfiguration());
			// Configure StockStatus enum conversion
			modelBuilder.Entity<Product>()
				.Property(p => p.StockStatus)
				.HasConversion(
					// Convert from Enum to Database string
					v => v == Utilities.Enums.StockStatus.GoodStock ? "InStock" :
						 v == Utilities.Enums.StockStatus.LowStock ? "LowStock" : "OutOfStock",
					// Convert from Database string to Enum
					v => v == "InStock" ? Utilities.Enums.StockStatus.GoodStock :
						 v == "LowStock" ? Utilities.Enums.StockStatus.LowStock : Utilities.Enums.StockStatus.OutOfStock
				);
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
    }
}
