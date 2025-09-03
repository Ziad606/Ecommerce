using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations;
public class AdvertisementEntityConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ImageLink).IsRequired().HasMaxLength(500);
        builder.Property(e => e.ImageOrientation).HasConversion<string>();
        builder.HasOne(e => e.Product)
              .WithMany()
              .HasForeignKey(e => e.ProductId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}
