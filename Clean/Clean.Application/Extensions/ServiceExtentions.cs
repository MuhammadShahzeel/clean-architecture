using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Extensions
{
    public static class ServiceExtentions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            //  MediatR register karo
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
