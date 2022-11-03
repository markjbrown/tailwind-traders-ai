using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.AwsClients;
using Tailwind.Traders.Profile.Api.DTOs;

namespace Tailwind.Traders.Profile.Api.Repositories
{
    public class AwsDynamoDbProfileRepository : IProfileRepository
    {
        private readonly AppSettings _appSettings;
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private const int _take = 3;

        public AwsDynamoDbProfileRepository(IOptions<AppSettings> options,
            AmazonDynamoDbClientFactory factory)
        {
            _appSettings = options.Value;
            _amazonDynamoDBClient = factory.Create();
        }

        public async Task Add(CreateUser user)
        {
            await DynomoDbService.AddProfileAsync(_amazonDynamoDBClient, _appSettings.DynamoDb.ProfileTable, user.MapUserProfile());
        }

        public async Task<IEnumerable<ProfileDto>> GetAll()
        {
            var profiles = await DynomoDbService.GetProfilesAsync(_amazonDynamoDBClient, _appSettings.DynamoDb.ProfileTable);
            return profiles.Select(p => p.ToProfileDto(_appSettings));
        }

        public async Task<ProfileDto> GetByEmail(string nameFilter)
        {
            var profiles = await DynomoDbService.GetProfileByEmailAsync(_amazonDynamoDBClient, _appSettings.DynamoDb.ProfileTable, nameFilter);
            return profiles.SingleOrDefault().ToProfileDto(_appSettings);
        }
    }
}
