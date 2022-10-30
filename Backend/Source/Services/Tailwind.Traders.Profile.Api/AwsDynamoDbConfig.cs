namespace Tailwind.Traders.Profile.Api
{
    public class AwsDynamoDbConfig
    {
        public static string ConfigKey = "DynamoDb";

        public string Host { get; set; }
        public string Database { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
        public string AwsRegion { get; set; }
        public string ProfileTable { get; set; }

    }
}
