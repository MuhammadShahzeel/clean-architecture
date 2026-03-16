using Clean.Application.Interfaces;
using Clean.Application.Settings;
using Clean.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Infrastructure.Extensions
{
    public static class ServiceExtentions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            

            var emailSection = configuration.GetSection("MailSettings");
            services.Configure<EmailSettings>(emailSection); // to make EmailSettings available for injection via IOptions<EmailSettings>
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
