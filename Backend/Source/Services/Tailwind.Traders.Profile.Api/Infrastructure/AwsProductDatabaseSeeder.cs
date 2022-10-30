using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.AwsClients;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public class AwsProfileDatabaseSeeder : ISeedDatabase
    {
        private readonly IProcessFile _processFile;
        private readonly AppSettings _appConfig;
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private readonly IWebHostEnvironment _env;
       

        public AwsProfileDatabaseSeeder(IProcessFile processFile, 
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
            var profiles = _processFile.Process<Profiles>(_env.ContentRootPath, "Profiles");

            Table profileTable = Table.LoadTable(_amazonDynamoDBClient, _appConfig.DynamoDb.ProfileTable);
            foreach (var item in profiles)
            {
                var productItem = new Document();
                productItem["Id"] = item.Id;
                productItem["Name"] = item.Name;
                productItem["Address"] = item.Address;
                productItem["Email"] = item.Email;
                productItem["PhoneNumber"] = item.PhoneNumber;
                productItem["ImageNameSmall"] = item.ImageNameSmall;
                productItem["ImageNameMedium"] = item.ImageNameMedium;
                await profileTable.PutItemAsync(productItem);
            }
        }
    }
}
