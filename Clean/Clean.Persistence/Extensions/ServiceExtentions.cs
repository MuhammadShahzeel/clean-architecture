using Clean.Application.Interfaces;
using Clean.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register the IApplicationDbContext interface with the ApplicationDbContext implementation
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        }
    }
}
