using Amazon.Runtime;

namespace Tailwind.Traders.Profile.Api.AwsClients
{
	public class AwsCredentials : AWSCredentials
	{
		private readonly AwsDynamoDbConfig _dynamoDbKeys;

		public AwsCredentials(AwsDynamoDbConfig dynamoDbKeys)
		{
			_dynamoDbKeys = dynamoDbKeys;
		}

		public override ImmutableCredentials GetCredentials()
		{
			return new ImmutableCredentials(_dynamoDbKeys.AwsAccessKey, _dynamoDbKeys.AwsSecretKey, null);
		}
	}
}
