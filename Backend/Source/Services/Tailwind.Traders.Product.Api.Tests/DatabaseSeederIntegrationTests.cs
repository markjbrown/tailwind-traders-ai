using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Controllers;
using Tailwind.Traders.Product.Api.Infrastructure;

namespace Tailwind.Traders.Product.Api.Tests
{
    [TestClass]
    public class DatabaseSeederIntegrationTests : IntegrationTestBase
    {
        private Mock<IWebHostEnvironment> _mockWebHostEnv;

        public DatabaseSeederIntegrationTests()
        {
            _mockWebHostEnv = new Mock<IWebHostEnvironment>();
            _mockWebHostEnv.Setup(env => env.ContentRootPath).Returns(GetRootDirectory());
        }

        [TestMethod]
        public async Task TestAzureSeeder()
        {
            Initialize("AZURE", services =>
            {
                services.AddScoped<IWebHostEnvironment>(w => _mockWebHostEnv.Object);
            });
            var seeder = ApiHost.Server.Services.GetService<ISeedDatabase>();
            await seeder.SeedAsync();
        }

        [TestMethod]
        public async Task TestAwsSeeder()
        {
            Initialize("AWS", services =>
            {
                services.AddScoped<IWebHostEnvironment>(w => _mockWebHostEnv.Object);
            });
            var seeder = ApiHost.Server.Services.GetService<ISeedDatabase>();
            await seeder.SeedAsync();
        }

        [TestMethod]
        public async Task TestGcpSeeder()
        {
            Initialize("GCP", services =>
            {
                services.AddScoped<IWebHostEnvironment>(w => _mockWebHostEnv.Object);
            });
            var seeder = ApiHost.Server.Services.GetService<ISeedDatabase>();
            await seeder.SeedAsync();
        }

        public string GetRootDirectory(
                [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            var projectDirectoryName = typeof(ProductController).Assembly.GetName().Name;
            var fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFilePath), "..", projectDirectoryName));
            return fullPath;
        }
    }
}
