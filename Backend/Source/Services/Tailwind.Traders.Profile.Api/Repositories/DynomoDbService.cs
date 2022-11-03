using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.DTOs;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Repositories
{
    public class DynomoDbService
    {
        public DynomoDbService()
        {
        }

        public async static Task<List<Profiles>> GetProfilesAsync(AmazonDynamoDBClient client, string tableName)
        {
            var items = new List<Profiles>();
            var request = new ScanRequest
            {
                TableName = tableName,
                FilterExpression = "App = :app",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     { ":app", new AttributeValue { S = Profiles.AppName } }
                }
            };
            var result = await client.ScanAsync(request);
            foreach (var item in result.Items)
            {
                items.Add(ExtractProfile(item));
            }
            return items;
        }

        private static Profiles ExtractProfile(Dictionary<string, AttributeValue> item)
        {
            item.TryGetValue("App", out var app);
            item.TryGetValue("Name", out var name);
            item.TryGetValue("Address", out var address);
            item.TryGetValue("PhoneNumber", out var phoneNumber);
            item.TryGetValue("Email", out var email);
            item.TryGetValue("ImageNameSmall", out var imageNameSmall);
            item.TryGetValue("ImageNameMedium", out var imageNameMedium);
            var profile = new Profiles
            {
                App = app?.S ?? string.Empty,
                Name = name?.S ?? string.Empty,
                Address = address?.S ?? string.Empty,
                PhoneNumber = phoneNumber?.S ?? string.Empty,
                Email = email?.S ?? string.Empty,
                ImageNameSmall = imageNameSmall?.S ?? string.Empty,
                ImageNameMedium = imageNameMedium?.S ?? string.Empty,
            };
            return profile;
        }

        public async static Task<List<Profiles>> GetProfileByEmailAsync(AmazonDynamoDBClient client, string tableName, string email)
        {
            var items = new List<Profiles>();
            var request = new ScanRequest
            {
                TableName = tableName,
                FilterExpression = "Email = :email",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                     { ":email", new AttributeValue { N = email} }
                }
            };
            var result = await client.ScanAsync(request);
            foreach (var item in result.Items)
            {
                items.Add(ExtractProfile(item));
            }
            return items.ToList();
        }

        public async static Task AddProfileAsync(AmazonDynamoDBClient client, string tableName, Profiles profile)
        {
            var table = Table.LoadTable(client, tableName);
            var item = new Document();
            item["App"] = profile.App;
            item["Name"] = profile.Name;
            item["Address"] = profile.Address;
            item["PhoneNumber"] = profile.PhoneNumber;
            item["Email"] = profile.Email;
            await table.PutItemAsync(item);
        }
    }
}
