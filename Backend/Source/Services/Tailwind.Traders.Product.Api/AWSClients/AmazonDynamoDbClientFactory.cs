using Amazon;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;

namespace Tailwind.Traders.Product.Api.AWSClients
{
    public class AmazonDynamoDbClientFactory
    {
        AppSettings _appSettings;
        public AmazonDynamoDbClientFactory(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }
        public AmazonDynamoDBClient Create()
        {
            var dynamoSettings = _appSettings.DynamoDBServiceKey;
            if (!string.IsNullOrEmpty(dynamoSettings.AwsAccessKey)
                && !string.IsNullOrEmpty(dynamoSettings.AwsSecretKey))
            {
                var dynamoDbConfig = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(dynamoSettings.AwsRegion)
                };
                var awsCredentials = new AwsCredentials(dynamoSettings);
                return new AmazonDynamoDBClient(awsCredentials, dynamoDbConfig);
            }

            return new AmazonDynamoDBClient();
        }
    }
}
