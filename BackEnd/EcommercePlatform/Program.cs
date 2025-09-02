using Ecommerce.API.Extensions;
using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Extensions;
using Ecommerce.DataAccess.Seeder;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Configurations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
namespace Ecommerce.API;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Host.UseSerilogLogging();

        // Active Model State
        builder.Services.AddControllers().ConfigureApiBehaviorOptions(
            options => options.SuppressModelStateInvalidFilter = true
        );

        // IOptional Pattern
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("Authorization:Google"));

        builder.Services.AddApplicationServices();
        builder.Services.AddScoped<ResponseHandler>();
        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddEmailServices(builder.Configuration);

        builder.Services.AddServicesConfigurations(builder.Configuration);

        builder.Services.AddDataProtection()
            .PersistKeysToDbContext<AuthContext>()
            .SetApplicationName("AuthStarter");

        // For redis 
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis")!) ??
                throw new InvalidOperationException("this 'Redis' is not valid");
            configuration.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(configuration);
        });

        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        // #region Seed User,Role Data
        // using (var scope = app.Services.CreateScope())
        // {
        //     var services = scope.ServiceProvider;
        //     var userManager = services.GetRequiredService<UserManager<User>>();
        //     var roleManager = services.GetRequiredService<RoleManager<Entities.Models.Auth.Identity.Role>>();
        //
        //     await RoleSeeder.SeedAsync(roleManager);
        //     await UserSeeder.SeedAsync(userManager);k
        // }
        // #endregion

        //if (app.Environment.IsDevelopment())
        //{
        app.UseSwagger();
        app.UseSwaggerUI();
        //}

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
