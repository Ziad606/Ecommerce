using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class WishlistEntityConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.BuyerId)
                   .IsRequired();

            builder.HasOne(w => w.Buyer)
               .WithOne(b => b.Wishlist) 
               .HasForeignKey<Wishlist>(w => w.BuyerId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Property(w => w.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(w => w.UpdatedAt)
                   .IsRequired(false);
        }
    }
}
