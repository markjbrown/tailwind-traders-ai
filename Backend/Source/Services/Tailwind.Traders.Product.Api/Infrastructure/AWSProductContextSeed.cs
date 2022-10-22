﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.AWSClients;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class AWSProductContextSeed : IContextSeed
    {
        private readonly IProcessFile _processFile;
        private readonly AppSettings _appConfig;
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private readonly IWebHostEnvironment _env;
       

        public AWSProductContextSeed(IProcessFile processFile, IOptions<AppSettings> options, IWebHostEnvironment env)
        {
            _processFile = processFile;
            _appConfig = options.Value;
            _env = env;
            _amazonDynamoDBClient = new AmazonDynamoDBClient();
        }

        public async Task SeedAsync()
        {
            var brands = _processFile.Process<ProductBrand>(_env.ContentRootPath, "ProductBrands");
            var types = _processFile.Process<ProductType>(_env.ContentRootPath, "ProductTypes");
            var features = _processFile.Process<ProductFeature>(_env.ContentRootPath, "ProductFeatures");
            var products = _processFile.Process<ProductItem>(_env.ContentRootPath, "ProductItems", new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });
            var tags = _processFile.Process<ProductTag>(_env.ContentRootPath, "ProductTags");


            
            Table productItemTable = Table.LoadTable(_amazonDynamoDBClient, _appConfig.AWS_PRODUCT_PRODUCTITEM_TABLE);
            foreach (var item in products)
            {
                var productItem = new Document();
                productItem["Id"] = item.Id;
                productItem["Name"] = item.Name;
                productItem["BrandId"] = item.BrandId;
                productItem["TypeId"] = item.TypeId;
                productItem["TagId"] = item.TagId;
                productItem["Price"] = item.Price;
                productItem["ImageName"] = item.ImageName;
                await productItemTable.PutItemAsync(productItem);
                                
            }


            
            Table brandTable = Table.LoadTable(_amazonDynamoDBClient, _appConfig.AWS_PRODUCT_BRAND_TABLE);
            foreach (var item in brands)
            {
                var brandItem = new Document();
                brandItem["Id"] = item.Id;
                brandItem["Name"] = item.Name;
                await brandTable.PutItemAsync(brandItem);
            }

            Table featureTable = Table.LoadTable(_amazonDynamoDBClient, _appConfig.AWS_PRODUCT_FEATURE_TABLE);
            foreach (var item in features)
            {
                var featureItem = new Document();
                featureItem["Id"] = item.Id;
                featureItem["ProductItemId"] = item.ProductItemId;
                featureItem["Title"] = item.Title;
                featureItem["Description"] = item.Description;
                await featureTable.PutItemAsync(featureItem);
            }

            Table productTypeTable = Table.LoadTable(_amazonDynamoDBClient, _appConfig.AWS_PRODUCT_TYPE_TABLE);
            foreach (var item in types)
            {
                var prodcutTypeItem = new Document();
                prodcutTypeItem["Id"] = item.Id;
                prodcutTypeItem["Code"] = item.Code;
                prodcutTypeItem["Name"] = item.Name;
                await productTypeTable.PutItemAsync(prodcutTypeItem);
            }

            Table productTagTable = Table.LoadTable(_amazonDynamoDBClient, _appConfig.AWS_PRODUCT_TAG_TABLE);
            foreach (var item in tags)
            {
                var prodcutTagItem = new Document();
                prodcutTagItem["Id"] = item.Id;
                prodcutTagItem["Value"] = item.Value;
                await productTagTable.PutItemAsync(prodcutTagItem);
            }
        }
    }
}
