﻿using eCommmerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Inerfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add database connectivity
            // Add authentication scheme
            services.AddSharedServices<ProductDbContext>(config, config["MySerilog:log:FineName"]!);

            // Create Dependency Injection (DI)
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception: handles external errors.
            // Listen to Only Api Gateway: blocks all outsider calls;
            app.UseSharedPolicies();

            return app;
        }
    }
}