using Ecommerce.Entities.Models.Auth.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.DataAccess.Seeder
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<User> _userManager)
        {
            var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var usersCount = await _userManager.Users.CountAsync();
            if (usersCount <= 0)
            {
                var adminUser = new User()
                {
                    UserName = "admin",
                    Email = "zezomohammed390@gmail.com",
                    PhoneNumber = "01224309198",
                    EmailConfirmed = true,FirstName = "John",
                    LastName = "Administrator",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Gender = "Male",
                    DefaultShippingAddress = "123 Admin Street, Business District, Cairo, Egypt",
                    DefaultBillingAddress = "123 Admin Street, Business District, Cairo, Egypt",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = baseDate,
                    LastLoginDate = DateTime.UtcNow.AddDays(-1),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumberConfirmed = true
                };
                await _userManager.CreateAsync(adminUser, "P@ssw0rd123Pass");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                
                
                var normalUser = new User
                {
                    UserName = "Ziad",
                    Email = "00ziadmohammed606@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Ziad",
                    LastName = "Mohammed",
                    DateOfBirth = new DateTime(1990, 8, 22),
                    Gender = "Male",
                    DefaultShippingAddress = "456 Manager Avenue, New Cairo, Egypt",
                    DefaultBillingAddress = "456 Manager Avenue, New Cairo, Egypt",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = baseDate.AddDays(1),
                    LastLoginDate = DateTime.UtcNow.AddDays(-2),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "+201234567891",
                    PhoneNumberConfirmed = true
                };
                
                await _userManager.CreateAsync(normalUser, "P@ssw0rd123Pass");
                await _userManager.AddToRoleAsync(normalUser, "User");
            }
        }
    }
}
