using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasIndex(ci => new { ci.CartId, ci.ProductId });

            builder.Property(ci => ci.Quantity)
                   .IsRequired();

            builder.Property(ci => ci.CreatedAt)
                   .IsRequired();

            builder.HasOne(ci => ci.Cart)
                   .WithMany(c => c.CartItems)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.Product)
                   .WithMany(p => p.CartItems)
                   .HasForeignKey(ci => ci.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
