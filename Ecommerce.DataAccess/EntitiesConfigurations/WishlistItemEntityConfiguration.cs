using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class WishlistItemEntityConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.HasKey(wi => wi.Id);
            builder.Property(wi => wi.WishlistId)
            .IsRequired();

            builder.Property(wi => wi.ProductId)
                .IsRequired();

            builder.HasOne(wi => wi.Wishlist)
                .WithMany(w => w.WishlistItems)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wi => wi.Product)
                .WithMany(p => p.WishlistItems)
                .HasForeignKey(wi => wi.ProductId);

            builder.Property(wi => wi.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
