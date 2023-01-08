using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.AwsClients;
using Tailwind.Traders.Product.Api.Models;
using Tailwind.Traders.Product.Api.Repositories.Services;
using Tailwind.Traders.Product.Api.Extensions;

namespace Tailwind.Traders.Product.Api.Repositories
{
    public class AwsDynamoDbProductItemRepository : IProductItemRepository
    {
        private readonly AppSettings _appSettings;
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private const int _take = 3;

        public AwsDynamoDbProductItemRepository(IOptions<AppSettings> options,
            AmazonDynamoDbClientFactory factory)
        {
            _appSettings = options.Value;
            _amazonDynamoDBClient = factory.Create();
        }

        public async Task<List<ProductItem>> FindProductsAsync(string[] brands, string[] types)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            items = items.Where(item => brands.Contains(item.BrandName) || types.Contains(item.Type.Name)).ToList();
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            items = items.Where(p => p.Tags.Contains(tag)).ToList();
            items = items.Take(_take).ToList();
            return items;
        }


        public async Task<List<ProductItem>> FindProductsByIdsAsync(int[] ids)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            items = items.Where(p => ids.Contains(p.ProductItemId)).ToList();
            items = items.Take(_take).ToList();
            return items;
        }
        public async Task<List<ProductBrand>> GetAllBrandsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductBrandTable);
            return items.Select(item => new ProductBrand { Name = item.BrandName })
                .Distinct().ToList();
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            return items;
        }

        public async Task<List<ProductType>> GetAllTypesAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            return items.Select(item => item.Type).DistinctBy(x => x.Code).ToList();
        }

        public async Task<ProductItem> GetProductById(int productId)
        {
            var item = await DynomoDbService.GetProductItemByIdAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable, productId);
            return item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            items = items.OrderBy(product => new Random().Next()).Take(_take).ToList();
            return items;
        }
    }
}
