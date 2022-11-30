using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class AzureProductDatabaseSeeder : ISeedDatabase
    {
        private readonly IProcessFile _processFile;
        private readonly ProductContext _productContext;
        private readonly IWebHostEnvironment _env;

        public AzureProductDatabaseSeeder(
            IProcessFile processFile,
            ProductContext productContext,
            IWebHostEnvironment env)
        {
            _processFile = processFile;
            _productContext = productContext;
            _env = env;
        }

        public async Task ResetAsync()
        {
            await _productContext.Database.EnsureDeletedAsync();
        }

        public async Task SeedAsync()
        {
            await _productContext.Database.EnsureCreatedAsync();

            if (!_productContext.ProductItems.ToList().Any())
            {
                var brands = _processFile.Process<ProductBrandSeed>(_env.ContentRootPath, "ProductBrands");
                var types = _processFile.Process<ProductTypeSeed>(_env.ContentRootPath, "ProductTypes");
                var features = _processFile.Process<ProductFeatureSeed>(_env.ContentRootPath, "ProductFeatures");
                var tags = _processFile.Process<ProductTagSeed>(_env.ContentRootPath, "ProductTags");
                var products = _processFile.Process<ProductItemSeed>(_env.ContentRootPath, "ProductItems",
                    new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });

                var productItems = ProductItemExtensions.Join(products, brands, types, features, tags);

                await _productContext.ProductItems.AddRangeAsync(productItems);

                await _productContext.SaveChangesAsync();
            }
        }
    }
}
