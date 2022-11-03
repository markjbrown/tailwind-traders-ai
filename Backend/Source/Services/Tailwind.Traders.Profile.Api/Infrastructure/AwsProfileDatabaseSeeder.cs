using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.AwsClients;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public class AwsProfileDatabaseSeeder : ISeedDatabase
    {
        private readonly IFileProcessor _processFile;
        private readonly AppSettings _appSettings;
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private readonly IWebHostEnvironment _env;
       

        public AwsProfileDatabaseSeeder(
            IFileProcessor processFile, 
            IOptions<AppSettings> options, 
            IWebHostEnvironment env,
            AmazonDynamoDbClientFactory factory)
        {
            _processFile = processFile;
            _appSettings = options.Value;
            _env = env;
            _amazonDynamoDBClient = factory.Create();
        }

        public async Task SeedAsync()
        {
            var profiles = _processFile.Process<Profiles>(_env.ContentRootPath, "Profiles");

            Table profileTable = Table.LoadTable(_amazonDynamoDBClient, _appSettings.DynamoDb.ProfileTable);
            foreach (var item in profiles)
            {
                var productItem = new Document();
                productItem["Name"] = item.Name;
                productItem["Address"] = item.Address;
                productItem["Email"] = item.Email;
                productItem["PhoneNumber"] = item.PhoneNumber;
                productItem["ImageNameSmall"] = item.ImageNameSmall;
                productItem["ImageNameMedium"] = item.ImageNameMedium;
                await profileTable.PutItemAsync(productItem);
            }
        }
/*
        private async Task CreateTable(string tableName)
        {
            var response = await _amazonDynamoDBClient.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>()
                              {
                                  new AttributeDefinition
                                  {
                                      AttributeName = "Id",
                                      AttributeType = "N"
                                  }
                              },
                KeySchema = new List<KeySchemaElement>()
                              {
                                  new KeySchemaElement
                                  {
                                      AttributeName = "Id",
                                      KeyType = "HASH"
                                  }
                              },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                }
            });
        }
*/
    }
}
