using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class ProductEntityConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Description)
                   .IsRequired();

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.SKU)
                   .HasMaxLength(100);

            // builder.Property(p => p.Status)
            //        .HasConversion<string>()
            //        .IsRequired();

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
            //
            // builder.HasOne(p => p.Seller)
            //        .WithMany(s => s.Products)
            //        .HasForeignKey(p => p.SellerId)
            //        .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Images)
                   .WithOne(i => i.Product)
                   .HasForeignKey(i => i.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reviews)
                   .WithOne(r => r.Product)
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.StockStatus)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();
        }
    }
}
