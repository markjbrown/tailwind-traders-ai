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
        const string AZURE_CLOUD = "AZURE";
        const string AWS_CLOUD = "AWS";
        const string GCP_CLOUD = "GCP";
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
            //service.AddTransient<IContextSeed<ProductContext>, ProductContextSeed>()
                service.AddTransient<IProcessFile, ProccessCsv>()
                .AddTransient<ClassMap, ProductBrandMap>()
                .AddTransient<ClassMap, ProductFeatureMap>()
                .AddTransient<ClassMap, ProductItemMap>()
                .AddTransient<ClassMap, ProductTypeMap>()
                .AddTransient<ClassMap, ProductTagMap>()
                .AddTransient<MapperDtos>();

            string env = configuration["CLOUD_PLATFORM"];

            if (env == AZURE_CLOUD)
            {
                service.AddTransient<IContextSeed, AzureProductContextSeed>();
                service.AddScoped<IProductItemRepository, AzureProductItemRepository>();
            }
            else if (env == AWS_CLOUD)
            {
                service.AddTransient<IContextSeed, AWSProductContextSeed>();
                service.AddScoped<IProductItemRepository, AwsDynamoProductItemRepository>();
            }
            else if (env == GCP_CLOUD)
            {
                service.AddTransient<IContextSeed, GCPProductContextSeed>();
                service.AddScoped<IProductItemRepository, GCPProductItemRepository>();
            }

            service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
