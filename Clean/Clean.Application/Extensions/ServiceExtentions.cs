using FluentValidation;
using MediatR;
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
            var assembly = typeof(ServiceExtentions).Assembly; 
            //  MediatR register 
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });

            //register AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            });

            // fluent validation registeration
            services.AddValidatorsFromAssembly(assembly);


            // register pipeline behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviors<,>));

        }
    }
}
