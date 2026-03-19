using Clean.Application.Interfaces;
using Clean.Application.Settings;
using Clean.Persistence.Context;
using Clean.Persistence.IdentityModels;
using Clean.Persistence.Seeds;
using Clean.Persistence.SharedServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Persistence.Extensions
{
    public static class ServiceExtentions
    {
        public static void AddPersistance(this IServiceCollection services, IConfiguration configuration)
        {
            //
            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");


            // Register the IApplicationDbContext interface with the ApplicationDbContext implementation
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();


            //register identity
            services.AddDataProtection();
            services.AddIdentityCore<ApplicationUser>()
           .AddRoles<ApplicationRole>()
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders()
           ;
            // configure token lifespan
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(30);
            });
            // you can usee identityuser and identityrole as well instead of applicationuser and applicationrole


            //always register here

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));


            // register shared services
            services.AddScoped<IAccountService, AccountService>();

            // register token service
            services.AddScoped<ITokenService, JwtTokenService>();

            // to get from app settings
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));






        }
    }
}
