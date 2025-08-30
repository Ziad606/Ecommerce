using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Primary key
            builder.HasKey(oi => oi.Id);

            builder.HasIndex(oi => new { oi.OrderId, oi.ProductId });

            // Properties
            builder.Property(oi => oi.Quantity)
                   .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.Subtotal)
                   .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(oi => oi.UpdatedAt)
                   .IsRequired(false);

            // Relationships
            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems) 
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade); // When order deleted, delete order items

            builder.HasOne(oi => oi.Product)
                   .WithMany(p => p.OrderItems)
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
