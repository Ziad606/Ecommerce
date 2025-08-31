using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.ImageUrl)
                   .IsRequired();

            builder.Property(pi => pi.ProductId)
                   .IsRequired();
            builder.HasIndex(pi => pi.ProductId);

            builder.Property(pi => pi.IsPrimary)
                   .HasDefaultValue(false);


            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.Images)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade); // When product deleted, delete its images

            builder.Property(pi => pi.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pi => pi.UpdatedAt)
                   .IsRequired(false);
        }
    }
}
