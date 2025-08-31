using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Auth.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations;

public class UserEntityConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table configuration
        builder.ToTable("Users");

        // Primary key (inherited from IdentityUser)
        builder.HasKey(u => u.Id);

        // Personal Information
        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.DateOfBirth)
            .HasColumnType("date");

        builder.Property(u => u.Gender)
            .HasMaxLength(20);

        // Contact & Address
        builder.Property(u => u.DefaultShippingAddress)
            .HasMaxLength(500);

        builder.Property(u => u.DefaultBillingAddress)
            .HasMaxLength(500);

        // Account Settings
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.UpdatedAt)
            .HasColumnType("datetime2");

        builder.Property(u => u.LastLoginDate)
            .HasColumnType("datetime2");

        // Navigation Properties - One-to-One relationships
        builder.HasOne(u => u.Cart)
            .WithOne(c => c.Buyer)
            .HasForeignKey<Cart>(c => c.BuyerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Wishlist)
            .WithOne(w => w.Buyer)
            .HasForeignKey<Wishlist>(w => w.BuyerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation Properties - One-to-Many relationships
        builder.HasMany(u => u.Orders)
            .WithOne(o => o.Buyer)
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion

        builder.HasMany(u => u.Reviews)
            .WithOne(o => o.Buyer)
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.UserName)
            .IsUnique()
            .HasDatabaseName("IX_Users_UserName");

        builder.HasIndex(u => new { u.FirstName, u.LastName })
            .HasDatabaseName("IX_Users_FullName");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        builder.HasIndex(u => u.IsDeleted)
            .HasDatabaseName("IX_Users_IsDeleted");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");

       
        // Additional constraints
        builder.HasCheckConstraint("CK_Users_DateOfBirth", "[DateOfBirth] <= GETDATE()");
        builder.HasCheckConstraint("CK_Users_Gender", "[Gender] IN ('Male', 'Female', 'Other', 'PreferNotToSay')");
    }
}