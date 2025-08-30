using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Description)
                   .HasMaxLength(500); 

            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()"); 

            builder.Property(c => c.UpdatedAt)
                   .IsRequired(false);

            // Relationships
            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // Dont delete products if we delete categories
        }
    }
}
