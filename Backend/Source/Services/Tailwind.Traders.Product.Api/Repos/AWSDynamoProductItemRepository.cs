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
    public class AWSDynamoProductItemRepository : IProductItemRepository
    {
        #region DataMember
        private readonly AppSettings _appConfig;
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private readonly DynamoDBContext _productContext;
        private const int _take = 3;
        #endregion

        public AWSDynamoProductItemRepository(IOptions<AppSettings> options)
        {
            _appConfig = options.Value;
            var dynamoDbConfig = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_appConfig.AwsRegion)
            };
            var awsCredentials = new AwsCredentials(_appConfig);
            _amazonDynamoDBClient = new AmazonDynamoDBClient(awsCredentials, dynamoDbConfig);
            _productContext = new DynamoDBContext(_amazonDynamoDBClient, new DynamoDBContextConfig
            {
                ConsistentRead = true
            });
        }

        public async Task<List<ProductItem>> FindProductsAsync(int[] brand, int[] type)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient);

            items = items.Where(item => brand.Contains(item.BrandId) || type.Contains(item.TypeId)).ToList();
            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(
                    brands,
                    types,
                    features,
                    tags);
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient);

            var productTag = tags.Where(t => t.Value == tag).SingleOrDefault();

            if (productTag == null)
            {
                return null;
            }

            items =  items.Where(p => p.TagId == productTag.Id).ToList();
            items = items.Take(_take).ToList();
            items
                .Join(
                    brands,
                    types,
                    features,
                    tags);

            return items;
        }

        public async Task<List<ProductBrand>> GetAllBrandsAsync()
        {
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient);
            return brands;
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient);


            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join
                (
                    brands,
                    types,
                    features,
                    tags
                );

            return items;
        }

        public async Task<List<ProductType>> GetAllTypesAsync()
        {
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient);
            return types;
        }

        public async Task<ProductItem> GetProductById(int productId)
        {
            var items = await DynomoDbService.GetProductItemByIdAsync(_amazonDynamoDBClient);
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient);

            items.Join(
                    brands,
                    types,
                    features,
                    tags);

            var item = items.FirstOrDefault();
            return item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var items = await DynomoDbService.GetProductItemsAsync(_amazonDynamoDBClient);
            items = items.OrderBy(product => new Random().Next()).Take(_take).ToList();
            var brands = await DynomoDbService.GetProductBrandsAsync(_amazonDynamoDBClient);
            var types = await DynomoDbService.GetProductTypesAsync(_amazonDynamoDBClient);
            var features = await DynomoDbService.GetProductFeaturesAsync(_amazonDynamoDBClient);
            var tags = await DynomoDbService.GetProductTagsAsync(_amazonDynamoDBClient);

            items.Join(
                     brands,
                    types,
                    features,
                    tags);

            return items;
        }
    }
}
