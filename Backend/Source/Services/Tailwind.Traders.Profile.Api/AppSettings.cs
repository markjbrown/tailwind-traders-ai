namespace Tailwind.Traders.Profile.Api
{
    public class AppSettings
    {
        public string ProfilesImageUrl { get; set; }
        public AzureCosmosDbConfig CosmosDb { get; set; }
        public GcpFirestoreConfig Firestore { get; set; }
        public AwsDynamoDbConfig DynamoDb { get; set; }
    }
}
