using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using Tailwind.Traders.Product.Api.HealthCheck;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Mappers;
using Tailwind.Traders.Product.Api.Repos;

namespace Tailwind.Traders.Product.Api.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddProductsContext(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<ProductContext>(options =>
            {
                options.UseCosmos(configuration["CosmosDb:Host"], configuration["CosmosDb:Key"], configuration["CosmosDb:Database"])
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });

            return service;
        }

        public static IServiceCollection AddModulesProducts(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddTransient<IContextSeed<ProductContext>, ProductContextSeed>()
                .AddTransient<IProcessFile, ProccessCsv>()
                .AddTransient<ClassMap, ProductBrandMap>()
                .AddTransient<ClassMap, ProductFeatureMap>()
                .AddTransient<ClassMap, ProductItemMap>()
                .AddTransient<ClassMap, ProductTypeMap>()
                .AddTransient<ClassMap, ProductTagMap>()
                .AddTransient<MapperDtos>()
#if AZURE
                .AddScoped<IProductItemRepository, AzureProductItemRepository>()
#elif AWS
                .AddScoped<IProductItemRepository, AWSProductItemRepository>()
#elif GCP
                .AddScoped<IProductItemRepository, GCPProductItemRepository>()            
#endif
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            service.Configure<AppSettings>(configuration);

            return service;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder.Add(new HealthCheckRegistration(
                "ProductsDB-check",
                sp => new CosmosDbHealthCheck(
                    $"AccountEndpoint={configuration["CosmosDb:Host"]};AccountKey={configuration["CosmosDb:Key"]}",
                    configuration["CosmosDb:Database"]),
                HealthStatus.Unhealthy,
                new string[] { "productdb" }
            ));

            return services;
        }
    }
}
