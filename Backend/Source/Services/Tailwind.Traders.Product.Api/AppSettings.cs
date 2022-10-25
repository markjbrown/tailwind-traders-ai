namespace Tailwind.Traders.Product.Api
{
    public class AppSettings
    {
        public string ProductImagesUrl { get; set; }
        public string ProductDetailImagesUrl { get; set; }
        public FireStoreKeys FireStoreServiceKey { get; set; }
        public DynamoDBKeys DynamoDBServiceKey { get; set; }
    }
}
