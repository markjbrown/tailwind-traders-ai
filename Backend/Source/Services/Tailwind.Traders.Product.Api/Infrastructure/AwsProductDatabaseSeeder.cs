using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.AwsClients;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class AwsProductDatabaseSeeder : ISeedDatabase
    {
        private readonly IProcessFile _processFile;
        private readonly AppSettings _appConfig;
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private readonly IWebHostEnvironment _env;


        public AwsProductDatabaseSeeder(IProcessFile processFile,
            IOptions<AppSettings> options,
            IWebHostEnvironment env,
            AmazonDynamoDbClientFactory factory)
        {
            _processFile = processFile;
            _appConfig = options.Value;
            _env = env;
            _amazonDynamoDBClient = factory.Create();
        }

        public async Task SeedAsync()
        {
            var brands = _processFile.Process<ProductBrandSeed>(_env.ContentRootPath, "ProductBrands");
            var types = _processFile.Process<ProductTypeSeed>(_env.ContentRootPath, "ProductTypes");
            var features = _processFile.Process<ProductFeatureSeed>(_env.ContentRootPath, "ProductFeatures");
            var tags = _processFile.Process<ProductTagSeed>(_env.ContentRootPath, "ProductTags");
            var products = _processFile.Process<ProductItemSeed>(_env.ContentRootPath, "ProductItems",
                new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });

            var productItems = ProductItemExtensions.Join(products, brands, types, features, tags);

            foreach (var item in productItems)
            {
                var request = new PutItemRequest
                {
                    TableName = _appConfig.DynamoDBServiceKey.ProductItemTable,
                    Item = new System.Collections.Generic.Dictionary<string, AttributeValue>
                    {
                        [nameof(ProductItem.ProductItemId)] = new AttributeValue { N = item.ProductItemId.ToString() },
                        [nameof(ProductItem.Name)] = new AttributeValue(item.Name),
                        [nameof(ProductItem.BrandName)] = new AttributeValue(item.BrandName),
                        [nameof(ProductItem.Type)] = ConvertToAttributeValue(item.Type),
                        [nameof(ProductItem.Tags)] = new AttributeValue { SS = item.Tags.ToList() },
                        [nameof(ProductItem.ProductItemId)] = new AttributeValue { N = item.Price.ToString() },
                        [nameof(ProductItem.ProductItemId)] = new AttributeValue(item.ImageName),
                    }
                };
                await _amazonDynamoDBClient.PutItemAsync(request);
            }

        }

        private AttributeValue ConvertToAttributeValue(ProductType type)
        {
            var value = new AttributeValue();
            value.M = new System.Collections.Generic.Dictionary<string, AttributeValue>
            {
                [nameof(ProductType.Name)] = new AttributeValue(type.Name),
                [nameof(ProductType.Code)] = new AttributeValue(type.Code),
            };
            return value;
        }
    }
}
