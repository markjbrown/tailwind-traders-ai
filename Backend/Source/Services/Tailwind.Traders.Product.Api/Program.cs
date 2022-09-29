using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Infrastructure;

namespace Tailwind.Traders.Product.Api
{
    public class Program
    {
        const string AZURE_CLOUD = "AZURE";
        const string AWS_CLOUD = "AWS";
        const string GCP_CLOUD = "GCP";
        public static void Main(string[] args)
        {
            var webHostBuilder = CreateWebHostBuilder(args).Build();

            var config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            var CLOUD_PLATFORM = config.GetSection("CLOUD_PLATFORM");

            if (CLOUD_PLATFORM.Value == AZURE_CLOUD)
            {
                webHostBuilder.MigrateDbContext<ProductContext, ProductContextSeed>();
            }

            webHostBuilder.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public static class LinqFilterExtension
    {
        public static IAsyncEnumerable<TEntity> AsAsyncEnumerable<TEntity>(this Microsoft.EntityFrameworkCore.DbSet<TEntity> obj) where TEntity : class
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsAsyncEnumerable(obj);
        }
        public static IQueryable<TEntity> Where<TEntity>(this Microsoft.EntityFrameworkCore.DbSet<TEntity> obj, System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return System.Linq.Queryable.Where(obj, predicate);
        }
    }
}
