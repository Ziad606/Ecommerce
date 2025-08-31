using Ecommerce.Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);


            builder.Property(o => o.TotalPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.ShippingPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(o => o.OrderDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(o => o.Buyer)
                   .WithMany(b => b.Orders)
                   .HasForeignKey(o => o.BuyerId)
                   .OnDelete(DeleteBehavior.Restrict); // If buyer deleted orders

            // builder.HasOne(o => o.Seller)
            //        .WithMany(s => s.Orders)
            //        .HasForeignKey(o => o.SellerId)
            //        .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId);
            
            
            builder.HasIndex(o => new { o.BuyerId });
            
        }
    }
}
