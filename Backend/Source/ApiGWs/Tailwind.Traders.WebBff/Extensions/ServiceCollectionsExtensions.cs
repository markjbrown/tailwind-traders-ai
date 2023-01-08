using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Tailwind.Traders.WebBff.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            Uri productsUri = new Uri($"{configuration["ProductsApiUrl"]}/liveness");
            Uri profileUri = new Uri($"{configuration["ProfileApiUrl"]}/liveness");
            Uri loginUri = new Uri($"{configuration["LoginApiUrl"]}/liveness");
            // Uri couponsUri = new Uri($"{configuration["CouponsApiUrl"]}/liveness");
            Uri imageClassifierUri = new Uri($"{configuration["ImageClassifierApiUrl"]}/liveness");
            // Uri popularProductsUri = new Uri($"{configuration["PopularProductsApiUrl"]}/liveness");
            Uri stockUri = new Uri($"{configuration["StockApiUrl"]}/liveness");
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddUrlGroup(productsUri, name: "productapi-check", tags: new string[] { "productapi" })
                .AddUrlGroup(profileUri, name: "profileapi-check", tags: new string[] { "profileapi" })
                .AddUrlGroup(loginUri, name: "loginapi-check", tags: new string[] { "loginapi" })
                //.AddUrlGroup(couponsUri, name: "couponsapi-check", tags: new string[] { "couponsapi" })
                .AddUrlGroup(imageClassifierUri, name: "image-classifier-api-check", tags: new string[] { "imageclassifierapi" })
                //.AddUrlGroup(popularProductsUri, name: "popular-products-api-check", tags: new string[] { "popularproductsapi" })
                .AddUrlGroup(stockUri, name: "stockapi-check", tags: new string[] { "stockapi" });

            return services;
        }
    }
}