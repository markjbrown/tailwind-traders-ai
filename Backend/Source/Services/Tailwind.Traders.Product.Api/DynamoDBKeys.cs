namespace Tailwind.Traders.Product.Api
{
    public class DynamoDBKeys
    {
        public string Host { get; set; }
        public string Database { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
        public string AwsRegion { get; set; }
        public string ProductItemTable { get; set; }
        public string ProductBrandTable { get; set; }
        public string ProductFeatureTable { get; set; }
        public string ProductTagTable { get; set; }
        public string ProductTypeTable { get; set; }

    }
}
