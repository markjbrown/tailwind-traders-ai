using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Mappers;
using Tailwind.Traders.Profile.Api.Helpers;
using Tailwind.Traders.Profile.Api.Infrastructure;
//using Tailwind.Traders.Product.Api.Infrastructure;
//using Tailwind.Traders.Product.Api.Mappers;

namespace InitializeData2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .CreateLogger();

            var host = Host.CreateDefaultBuilder(args)
                            .ConfigureServices((ctx, service) =>
                            {
                                RegisterProductStuff(ctx, service);
                                RegisterProfileStuff(ctx, service);
                            })
                            .Build();

            try
            {
                using var scope = host.Services.CreateScope();
                var serviceProvider = scope.ServiceProvider;
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var profileContext = serviceProvider.GetRequiredService<ProfileContext>();
                var productContext = serviceProvider.GetRequiredService<ProductContext>();

                Log.Information("Seeding database");
                var profileSeed = serviceProvider.GetRequiredService<ProfileContextSeed>();
                profileSeed.SeedInternalAsync(profileContext, @"D:\Projects\Microsoft\Code\TailwindTraders\Backend\Services\Tailwind.Traders.Profile.Api").Wait();

                var productSeed = serviceProvider.GetRequiredService<ProductContextSeed>();
                productSeed.SeedInternalAsync(productContext,
                    @"D:\Projects\Microsoft\Code\TailwindTraders\Backend\Services\Tailwind.Traders.Product.Api").Wait();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Something went wrong");
            }
        }

        private static void RegisterProfileStuff(HostBuilderContext ctx, IServiceCollection service)
        {
            service.AddDbContext<ProfileContext>(options =>
            {
                options.UseCosmos(
                    ctx.Configuration["CosmosDb:Host"],
                    ctx.Configuration["CosmosDb:Key"],
                    ctx.Configuration["CosmosDb:Database"])
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            service
                .AddTransient<ProfileContextSeed>()
                .AddScoped<CsvReaderHelper>();
        }

        private static void RegisterProductStuff(HostBuilderContext ctx, IServiceCollection service)
        {
            service.AddDbContext<ProductContext>(options =>
            {
                options.UseCosmos(
                    ctx.Configuration["CosmosDb:Host"],
                    ctx.Configuration["CosmosDb:Key"],
                    ctx.Configuration["CosmosDb:Database"])
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            service.AddTransient<ProductContextSeed>()
                .AddTransient<IProcessFile, ProccessCsv>()
                .AddTransient<ClassMap, ProductBrandMap>()
                .AddTransient<ClassMap, ProductFeatureMap>()
                .AddTransient<ClassMap, ProductItemMap>()
                .AddTransient<ClassMap, ProductTypeMap>()
                .AddTransient<ClassMap, ProductTagMap>()
                .AddTransient<MapperDtos>();
        }
    }
}
