using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Repos.Services
{
    public class DynomoDbService
    {
        public DynomoDbService()
        {

        }

        public async static Task<List<ProductItem>> GetProductItemsAsync(AmazonDynamoDBClient _amazonDynamoDBClient)
        {
            var items = new List<ProductItem>();
            var request = new ScanRequest
            {
                TableName = typeof(ProductItem).Name,
                FilterExpression = "Id >= :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     {":id", new AttributeValue {N =  "1"}}
                    }
            };
            var result = await _amazonDynamoDBClient.ScanAsync(request);
            foreach (var item in result.Items)
            {
                item.TryGetValue("Id", out var id);
                item.TryGetValue("Name", out var name);
                item.TryGetValue("Price", out var price);
                item.TryGetValue("ImageName", out var imageName);
                item.TryGetValue("TagId", out var tagId);
                item.TryGetValue("TypeId", out var typeId);
                item.TryGetValue("BrandId", out var brandId);
                items.Add(new ProductItem
                {
                    Id = Convert.ToInt32(id?.N ?? "0"),
                    Name = name?.S ?? string.Empty,
                    Price = Convert.ToSingle(price?.N ?? "0"),
                    ImageName = imageName?.S ?? string.Empty,
                    BrandId = Convert.ToInt32(brandId?.N ?? "0"),
                    TypeId = Convert.ToInt32(typeId?.N ?? "0"),
                    TagId = Convert.ToInt32(tagId?.N ?? "0")                    
                });
            }
            return items.ToList();
        }

        public async static Task<List<ProductItem>> GetProductItemByIdAsync(AmazonDynamoDBClient _amazonDynamoDBClient)
        {
            var items = new List<ProductItem>();
            var request = new ScanRequest
            {
                TableName = typeof(ProductItem).Name,
                FilterExpression = "Id = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     {":id", new AttributeValue {N =  "1"}}
                    }
            };
            var result = await _amazonDynamoDBClient.ScanAsync(request);
            foreach (var item in result.Items)
            {
                item.TryGetValue("Id", out var id);
                item.TryGetValue("Name", out var name);
                item.TryGetValue("Price", out var price);
                item.TryGetValue("ImageName", out var imageName);
                item.TryGetValue("TagId", out var tagId);
                item.TryGetValue("TypeId", out var typeId);
                item.TryGetValue("BrandId", out var brandId);
                items.Add(new ProductItem
                {
                    Id = Convert.ToInt32(id?.N ?? "0"),
                    Name = name?.S ?? string.Empty,
                    Price = Convert.ToSingle(price?.N ?? "0"),
                    ImageName = imageName?.S ?? string.Empty,
                    BrandId = Convert.ToInt32(brandId?.N ?? "0"),
                    TypeId = Convert.ToInt32(typeId?.N ?? "0"),
                    TagId = Convert.ToInt32(tagId?.N ?? "0")
                });
            }
            return items.ToList();
        }

        public async static Task<List<ProductBrand>> GetProductBrandsAsync(AmazonDynamoDBClient _amazonDynamoDBClient)
        {
            var brands = new List<ProductBrand>();
            var request = new ScanRequest
            {
                TableName = typeof(ProductBrand).Name,
                FilterExpression = "Id >= :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     {":id", new AttributeValue {N =  "1"}}
                    }
            };
            var result = await _amazonDynamoDBClient.ScanAsync(request);
            foreach (var item in result.Items)
            {
                item.TryGetValue("Id", out var id);
                item.TryGetValue("Name", out var name);
                brands.Add(new ProductBrand
                {
                    Id = Convert.ToInt32(id?.N ?? "0"),
                    Name = name?.S ?? string.Empty
                });
            }
            return brands.ToList();
        }

        public async static Task<List<ProductType>> GetProductTypesAsync(AmazonDynamoDBClient _amazonDynamoDBClient)
        {
            var types = new List<ProductType>();
            var request = new ScanRequest
            {
                TableName = typeof(ProductType).Name,
                FilterExpression = "Id >= :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     {":id", new AttributeValue {N =  "1"}}
                    }
            };
            var result = await _amazonDynamoDBClient.ScanAsync(request);
            foreach (var item in result.Items)
            {
                item.TryGetValue("Id", out var id);
                item.TryGetValue("Name", out var name);
                item.TryGetValue("Code", out var code);
                types.Add(new ProductType
                {
                    Id = Convert.ToInt32(id?.N ?? "0"),
                    Code = code?.S ?? string.Empty,
                    Name = name?.S ?? string.Empty
                });
            }
            return types.ToList();
        }

        public async static Task<List<ProductTag>> GetProductTagsAsync(AmazonDynamoDBClient _amazonDynamoDBClient)
        {
            var tag = new List<ProductTag>();
            var request = new ScanRequest
            {
                TableName = typeof(ProductTag).Name,
                FilterExpression = "Id >= :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     {":id", new AttributeValue {N =  "1"}}
                    }
            };
            var result = await _amazonDynamoDBClient.ScanAsync(request);
            foreach (var item in result.Items)
            {
                item.TryGetValue("Id", out var id);
                item.TryGetValue("Value", out var value);
                tag.Add(new ProductTag
                {
                    Id = Convert.ToInt32(id?.N ?? "0"),
                    Value = value?.S ?? string.Empty
                });
            }
            return tag.ToList();
        }

        public async static Task<List<ProductFeature>> GetProductFeaturesAsync(AmazonDynamoDBClient _amazonDynamoDBClient)
        {
            var features = new List<ProductFeature>();
            var request = new ScanRequest
            {
                TableName = typeof(ProductFeature).Name,
                FilterExpression = "Id >= :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     {":id", new AttributeValue {N =  "1"}}
                    }
            };
            var result = await _amazonDynamoDBClient.ScanAsync(request);
            foreach (var item in result.Items)
            {
                item.TryGetValue("Id", out var id);
                item.TryGetValue("Title", out var title);
                item.TryGetValue("Description", out var description);
                item.TryGetValue("ProductItemId", out var productItemId);

                features.Add(new ProductFeature
                {
                    Id = Convert.ToInt32(id?.N ?? "0"),
                    Title= title?.S ?? string.Empty,
                    Description = description?.S ?? string.Empty,
                    ProductItemId = Convert.ToInt32(productItemId?.N ?? "0")
                });
            }
            return features.ToList();
        }
    }
}
