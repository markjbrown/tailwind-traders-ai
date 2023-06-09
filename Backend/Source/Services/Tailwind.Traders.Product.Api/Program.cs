﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Tailwind.Traders.Product.Api.Infrastructure;

namespace Tailwind.Traders.Product.Api
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            using (var scope = webHost.Services.CreateScope())
            {
                var seedData = scope.ServiceProvider.GetRequiredService<ISeedDatabase>();
                seedData.SeedAsync().Wait();
            }

            webHost.Run();
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
