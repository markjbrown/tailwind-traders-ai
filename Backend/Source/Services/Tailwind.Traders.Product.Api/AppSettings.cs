namespace Tailwind.Traders.Product.Api
{
    public class AppSettings
    {
        public string ProductImagesUrl { get; set; }
        public FireStoreKeys FireStoreServiceKey { get; set; }
        public DynamoDBKeys DynamoDBServiceKey { get; set; }
        public string ProductItemCollectionName { get; set; }
        public string ProductBrandCollectionName { get; set; }
        public string ProductFeatureCollectionName { get; set; }
        public string ProductTagCollectionName { get; set; }
        public string ProductTypeCollectionName { get; set; }       
    }
}
