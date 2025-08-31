using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Entities.Models.Auth.Identity;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class CartEntityConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {

            builder.HasKey(c => c.Id);

            builder.Property(c => c.BuyerId)
                   .IsRequired();
            builder.HasIndex(c => c.BuyerId);

            // Timestamps
            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                   .IsRequired(false);

            // One-to-Many: Cart - CartItems
            builder.HasMany(c => c.CartItems)
                   .WithOne(ci => ci.Cart)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade); // When delete cart delete its cart items

            // One-to-One: Buyer - Cart
            builder.HasOne(c => c.Buyer)
                   .WithOne(b => b.Cart)
                   .HasForeignKey<Cart>(c => c.BuyerId)
                   .OnDelete(DeleteBehavior.Cascade); // When delete a buyer delete its cart

        }
    }
}
