namespace Tailwind.Traders.Product.Api
{
    public class AppSettings
    {
        public string ProductImagesUrl { get; set; }
        public string ProductDetailImagesUrl { get; set; }

        public AzureCosmosDbConfig CosmosDb { get; set; }
        public GcpFirestoreConfig FireStoreServiceKey { get; set; }
        public AwsDynamoDbConfig DynamoDBServiceKey { get; set; }
    }
}
