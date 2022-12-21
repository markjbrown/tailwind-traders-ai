using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Repositories.Services
{
    public class DynomoDbService
    {
        public DynomoDbService()
        {
        }

        public async static Task<List<ProductItem>> GetProductItemsAsync(AmazonDynamoDBClient _amazonDynamoDBClient, string _tableName)
        {
            var items = new List<ProductItem>();
            var request = new ScanRequest
            {
                TableName = _tableName,
                FilterExpression = "Id >= :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     { ":id", new AttributeValue { N = "1" } }
                }
            };
            var result = await _amazonDynamoDBClient.ScanAsync(request);
            foreach (var item in result.Items)
            {
                item.TryGetValue(nameof(ProductItem.ProductItemId), out var productId);
                item.TryGetValue(nameof(ProductItem.Name), out var name);
                item.TryGetValue(nameof(ProductItem.Price), out var price);
                item.TryGetValue(nameof(ProductItem.ImageName), out var imageName);
                item.TryGetValue(nameof(ProductItem.Tags), out var tags);
                item.TryGetValue(nameof(ProductItem.Type), out var type);
                item.TryGetValue(nameof(ProductItem.BrandName), out var brandName);
                items.Add(new ProductItem
                {
                    ProductItemId = Convert.ToInt32(productId.N ?? "0"),
                    Name = name?.S ?? string.Empty,
                    Price = Convert.ToSingle(price?.N ?? "0"),
                    ImageName = imageName?.S ?? string.Empty,
                    BrandName = brandName?.S ?? string.Empty,
                    Type = ConvertToType(type?.M), //TODO: Need to map the dictionary to a type object
                    Tags = tags.SS.AsReadOnly()
                });
            }
            return items.ToList();
        }

        private static ProductType ConvertToType(Dictionary<string, AttributeValue> m)
        {
            string name = string.Empty;
            if (m.TryGetValue(nameof(ProductType.Name), out var nameAttribute))
            {
                name = nameAttribute.S;
            }
            string code = string.Empty;
            if (m.TryGetValue(nameof(ProductType.Code), out var codeAttribute))
            {
                code = nameAttribute.S;
            }
            return new ProductType { Name = name, Code = code };
        }

        public async static Task<ProductItem> GetProductItemByIdAsync(AmazonDynamoDBClient _amazonDynamoDBClient, string _tableName, int productId)
        {
            var items = new List<ProductItem>();
            var request = new ScanRequest
            {
                TableName = _tableName,
                FilterExpression = "Id = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     { ":id", new AttributeValue { N = productId.ToString() } }
                }
            };
            var result = await _amazonDynamoDBClient.ScanAsync(request);
            var item = result.Items.SingleOrDefault();
            item.TryGetValue(nameof(ProductItem.ProductItemId), out var id);
            item.TryGetValue(nameof(ProductItem.Name), out var name);
            item.TryGetValue(nameof(ProductItem.Price), out var price);
            item.TryGetValue(nameof(ProductItem.ImageName), out var imageName);
            item.TryGetValue(nameof(ProductItem.Tags), out var tags);
            item.TryGetValue(nameof(ProductItem.Type), out var type);
            item.TryGetValue(nameof(ProductItem.BrandName), out var brandName);
            var productItem = new ProductItem
            {
                ProductItemId = Convert.ToInt32(id?.N ?? "0"),
                Name = name?.S ?? string.Empty,
                Price = Convert.ToSingle(price?.N ?? "0"),
                ImageName = imageName?.S ?? string.Empty,
                BrandName = brandName?.S ?? string.Empty,
                Type = ConvertToType(type?.M),
                Tags = tags.SS.AsReadOnly()
            };
            return productItem;
        }
    }
}
