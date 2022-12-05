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

        public async Task<List<ProductItem>> FindProductsAsync(int[] brand, int[] type)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductBrandTable);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTypeTable);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductFeatureTable);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTagTable);

            items = items.Where(item => brand.Contains(item.BrandId) || type.Contains(item.TypeId)).ToList();
            items
                .OrderBy(inc => inc.Id)
                .Join(brands, types, features, tags);
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductBrandTable);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTypeTable);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductFeatureTable);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTagTable);

            var productTag = tags.Where(t => t.Value == tag).SingleOrDefault();
            if (productTag == null)
            {
                return null;
            }

            items = items.Where(p => p.TagId == productTag.Id).ToList();
            items = items.Take(_take).ToList();
            items.Join(brands, types, features, tags);
            return items;
        }


        public async Task<List<ProductItem>> FindProductsByIdsAsync(int[] ids)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductBrandTable);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTypeTable);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductFeatureTable);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTagTable);
            items = items.Where(p => ids.Contains(p.Id)).ToList();
            items = items.Take(_take).ToList();
            items.Join(brands, types, features, tags);
            return items;
        }
        public async Task<List<ProductBrand>> GetAllBrandsAsync()
        {
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductBrandTable);
            return brands.OrderBy(x => x.Id).ToList();
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductBrandTable);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTypeTable);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductFeatureTable);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTagTable);

            items.Join(brands, types, features, tags);

            return items.OrderBy(x => x.Id).ToList();
        }

        public async Task<List<ProductType>> GetAllTypesAsync()
        {
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTypeTable);
            return types.OrderBy(x => x.Id).ToList();
        }

        public async Task<ProductItem> GetProductById(int productId)
        {
            var items = await DynomoDbService.GetProductItemByIdAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable, productId);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductBrandTable);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTypeTable);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductFeatureTable);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTagTable);

            items.Join(brands, types, features, tags);

            var item = items.SingleOrDefault();
            return item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductItemTable);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductBrandTable);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTypeTable);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductFeatureTable);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appSettings.DynamoDBServiceKey.ProductTagTable);

            items = items
                .OrderBy(product => new Random().Next()).Take(_take)
                .ToList();
            items.Join(brands, types, features, tags);

            return items;
        }
    }
}
