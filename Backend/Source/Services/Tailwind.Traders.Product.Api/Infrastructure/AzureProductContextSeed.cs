using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.AWSClients;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class AzureProductContextSeed : IContextSeed
    {

        private readonly IProcessFile _processFile;
        private readonly AppSettings _appConfig;
        private readonly ProductContext _productContext;
         private readonly IWebHostEnvironment _env;
        public AzureProductContextSeed(IProcessFile processFile, ProductContext productContext, IOptions<AppSettings> options, IWebHostEnvironment env)
        {
            _processFile = processFile;
            _appConfig = options.Value;
            _productContext = productContext;
              _env = env;
        }

        public async Task SeedAsync()
        {
            
            await _productContext.Database.EnsureCreatedAsync();

            if (!_productContext.ProductItems.ToList().Any())
            {
                var brands = _processFile.Process<ProductBrand>(_env.ContentRootPath, "ProductBrands");
                var types = _processFile.Process<ProductType>(_env.ContentRootPath, "ProductTypes");
                var features = _processFile.Process<ProductFeature>(_env.ContentRootPath, "ProductFeatures");
                var products = _processFile.Process<ProductItem>(_env.ContentRootPath, "ProductItems", new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });
                var tags = _processFile.Process<ProductTag>(_env.ContentRootPath, "ProductTags");
                               

                await _productContext.ProductItems.AddRangeAsync(products);
                await _productContext.ProductBrands.AddRangeAsync(brands);
                await _productContext.ProductTypes.AddRangeAsync(types);
                await _productContext.Tags.AddRangeAsync(tags);
                await _productContext.ProductFeatures.AddRangeAsync(features);

                await _productContext.SaveChangesAsync();
            }
        }
    }
}
