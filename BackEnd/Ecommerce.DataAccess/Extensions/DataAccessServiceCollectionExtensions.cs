using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.Advertisement;
using Ecommerce.DataAccess.Services.Auth;
using Ecommerce.DataAccess.Services.Category;
using Ecommerce.DataAccess.Services.Email;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.DataAccess.Services.OAuth;
using Ecommerce.DataAccess.Services.Order;
using Ecommerce.DataAccess.Services.OTP;
using Ecommerce.DataAccess.Services.Products;
using Ecommerce.DataAccess.Services.Token;
using Ecommerce.DataAccess.Services.Wishlist;
using Ecommerce.Services.Implementations;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;

namespace Ecommerce.DataAccess.Extensions
{
    public static class DataAccessServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DevCS") ??
                    throw new InvalidOperationException("Connection string 'DevCS' not found.");
            services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(connectionString)
            );
            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthGoogleService, AuthGoogleService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAdvertisementService, AdvertisementService>();

            return services;
        }

        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();

            services.AddFluentEmail(emailSettings!.FromEmail)
                .AddSmtpSender(new SmtpClient(emailSettings.SmtpServer)
                {
                    Port = emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
                    EnableSsl = emailSettings.EnableSsl
                });

            return services;
        }
    }
}
