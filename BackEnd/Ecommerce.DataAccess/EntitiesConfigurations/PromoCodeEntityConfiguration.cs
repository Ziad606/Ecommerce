using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations;
public class PromoCodeEntityConfiguration : IEntityTypeConfiguration<PromoCode>
{

    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        // Table name
        builder.ToTable("PromoCodes");

        // Primary key
        builder.HasKey(p => p.Code);

        // Code property configuration
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false); // For better performance with alphanumeric codes

        // Date properties
        builder.Property(p => p.StartAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(p => p.EndAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        // Discount percentage with precision
        builder.Property(p => p.DiscountPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");


        // IsDeleted for soft delete pattern
        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // IsActive is computed, so we ignore it for EF mapping
        builder.Ignore(p => p.IsActive);

        // Indexes for better query performance
        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_PromoCodes_IsDeleted");

        builder.HasIndex(p => new { p.StartAt, p.EndAt })
            .HasDatabaseName("IX_PromoCodes_DateRange");

        builder.HasIndex(p => new { p.IsDeleted, p.StartAt, p.EndAt })
            .HasDatabaseName("IX_PromoCodes_Active_DateRange");


        builder.HasCheckConstraint("CK_PromoCode_DiscountPercentage",
               "DiscountPercentage >= 0 AND DiscountPercentage <= 100");


        // Add constraint to ensure EndAt is after StartAt
        builder.HasCheckConstraint("CK_PromoCode_DateRange", "EndAt > StartAt");

        // Global query filter for soft delete (optional)
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
