using Amazon.Runtime;

namespace Tailwind.Traders.Product.Api.AWSClients
{
	public class AwsCredentials : AWSCredentials
	{
		private readonly DynamoDBKeys _dynamoDbKeys;

		public AwsCredentials(DynamoDBKeys dynamoDbKeys)
		{
			_dynamoDbKeys = dynamoDbKeys;
		}

		public override ImmutableCredentials GetCredentials()
		{
			return new ImmutableCredentials(_dynamoDbKeys.AwsAccessKey, _dynamoDbKeys.AwsSecretKey, null);
		}
	}
}
