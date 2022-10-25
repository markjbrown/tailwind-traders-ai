using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Reflection;
using System;
using Tailwind.Traders.Product.Api.Controllers;
using System.Configuration;

namespace Tailwind.Traders.Product.Api.Tests
{
    public class ApiHost
    {
        public const string BaseAddress = "https://apihost";
        private Startup _startup;

        public string Url(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;
            return BaseAddress + path;
        }

        public TestServer Server { get; private set; }
        public HttpMessageHandler Handler { get; private set; }
        public HttpClient Client { get; set; }

        public void Initialize(string cloudPlatform)
        {
            var hostBuilder = new HostBuilder()
               .ConfigureWebHost(builder =>
               {
                   builder.UseTestServer();

                   builder.ConfigureAppConfiguration((context, b) =>
                   {
                       // this is needed to pull in the API assembly into dynamic controller resolution
                       context.HostingEnvironment.ApplicationName = typeof(ProductController).Assembly.GetName().Name;
                   });

                   builder.UseSetting("Environment", "Development");

                   var configuration = new ConfigurationBuilder()
                        .AddJsonFile($"appsettings.{cloudPlatform}.json")
                        .AddUserSecrets(typeof(Startup).GetTypeInfo().Assembly)
                        .Build();

                   _startup = new Startup(configuration);
                   builder.ConfigureServices(ConfigureServices);
                   builder.Configure((ctx, bldr) => Configure(bldr, ctx.HostingEnvironment, bldr.ApplicationServices));
               });

            var host = hostBuilder.Start();

            Server = host.GetTestServer();
            Handler = Server.CreateHandler();

            Client = new HttpClient(Handler);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _startup.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            _startup.Configure(app, env, serviceProvider);
        }

        public HttpClient CreateClient()
        {
            return new HttpClient(Handler);
        }
    }
}