namespace Tailwind.Traders.Product.Api
{
    public class AppSettings
    {
        public string ProductImagesUrl { get; set; }
        public FireStoreKeys FireStoreServiceKey { get; set; }
        public DynamoDBKeys DynamoDBServiceKey { get; set; }
        public string PRODUCT_PRODUCTITEM_TABLE { get; set; }
        public string PRODUCT_BRAND_TABLE { get; set; }
        public string PRODUCT_FEATURE_TABLE { get; set; }
        public string PRODUCT_TAG_TABLE { get; set; }
        public string PRODUCT_TYPE_TABLE { get; set; }       
    }
}
