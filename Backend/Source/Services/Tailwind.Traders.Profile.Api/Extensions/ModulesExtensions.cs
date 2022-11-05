using Google.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tailwind.Traders.Profile.Api.AwsClients;
using Tailwind.Traders.Profile.Api.Helpers;
using Tailwind.Traders.Profile.Api.Infrastructure;
using Tailwind.Traders.Profile.Api.Repositories;

namespace Tailwind.Traders.Profile.Api.Extensions
{
    public static class ModulesExtensions
    {
        public static IServiceCollection AddModulesProfile(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<CsvReaderHelper>();
            services.AddScoped<IFileProcessor, CsvFileProcessor>();

            string env = configuration["CLOUD_PLATFORM"];

            if (env == CloudConstants.AZURE_CLOUD)
            {
                services.AddTransient<ISeedDatabase, AzureProfileDatabaseSeeder>();
                services.AddScoped<IProfileRepository, AzureProfileRepository>();
                services.AddApplicationInsightsTelemetry(configuration);
                services.AddProfileContext(configuration);

                var appInsightsIK = configuration["ApplicationInsights:InstrumentationKey"];
                if (!string.IsNullOrEmpty(appInsightsIK))
                {
                    services.AddApplicationInsightsTelemetry(appInsightsIK);
                }
            }
            else if (env == CloudConstants.AWS_CLOUD)
            {
                services.AddTransient<AmazonDynamoDbClientFactory>();
                services.AddTransient<ISeedDatabase, AwsProfileDatabaseSeeder>();
                services.AddScoped<IProfileRepository, AwsDynamoDbProfileRepository>();
            }
            else if (env == CloudConstants.GCP_CLOUD)
            {
                services.AddTransient<ISeedDatabase, GcpProfileDatabaseSeeder>();
                services.AddScoped<IProfileRepository, GcpFirestoreProfileRepository>();
            }
            return services;
        }
    }
}
