using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.AWSClients;
using Tailwind.Traders.Product.Api.Models;
using Tailwind.Traders.Product.Api.Repos.Services;
using Tailwind.Traders.Product.Api.Extensions;

namespace Tailwind.Traders.Product.Api.Repos
{
    public class AwsDynamoProductItemRepository : IProductItemRepository
    {
        #region DataMember
        private readonly AppSettings _appConfig;
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private readonly DynamoDBContext _productContext;
        private const int _take = 3;
        #endregion

        public AwsDynamoProductItemRepository(IOptions<AppSettings> options)
        {
            _appConfig = options.Value;
            var dynamoDbConfig = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_appConfig.DynamoDBServiceKey.AwsRegion)
            };
            var awsCredentials = new AwsCredentials(_appConfig);
            _amazonDynamoDBClient = new AmazonDynamoDBClient(awsCredentials, dynamoDbConfig);
        }

        public async Task<List<ProductItem>> FindProductsAsync(int[] brand, int[] type)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appConfig.ProductItemCollectionName);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appConfig.ProductBrandCollectionName);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appConfig.ProductTypeCollectionName);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appConfig.ProductFeatureCollectionName);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appConfig.ProductTagCollectionName);

            items = items.Where(item => brand.Contains(item.BrandId) || type.Contains(item.TypeId)).ToList();
            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(brands, types, features, tags);
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appConfig.ProductItemCollectionName);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appConfig.ProductBrandCollectionName);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appConfig.ProductTypeCollectionName);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appConfig.ProductFeatureCollectionName);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appConfig.ProductTagCollectionName);

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

        public async Task<List<ProductBrand>> GetAllBrandsAsync()
        {
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appConfig.ProductBrandCollectionName);
            return brands;
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appConfig.ProductItemCollectionName);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appConfig.ProductBrandCollectionName);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appConfig.ProductTypeCollectionName);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appConfig.ProductFeatureCollectionName);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appConfig.ProductTagCollectionName);

            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(brands, types, features, tags);

            return items;
        }

        public async Task<List<ProductType>> GetAllTypesAsync()
        {
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appConfig.ProductTypeCollectionName);
            return types;
        }

        public async Task<ProductItem> GetProductById(int productId)
        {
            var items = await DynomoDbService.GetProductItemByIdAsync(_amazonDynamoDBClient, _appConfig.ProductItemCollectionName);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appConfig.ProductBrandCollectionName);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appConfig.ProductTypeCollectionName);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appConfig.ProductFeatureCollectionName);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appConfig.ProductTagCollectionName);

            items.Join(brands, types, features, tags);


            var item = items.FirstOrDefault();
            return item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient, _appConfig.ProductItemCollectionName);
            items = items.OrderBy(product => new Random().Next()).Take(_take).ToList();
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient, _appConfig.ProductBrandCollectionName);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient, _appConfig.ProductTypeCollectionName);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient, _appConfig.ProductFeatureCollectionName);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient, _appConfig.ProductTagCollectionName);

            items.Join(brands, types, features, tags);

            return items;
        }
    }
}
