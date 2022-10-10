namespace Tailwind.Traders.Product.Api
{
    public class AppSettings
    {
        public string ProductImagesUrl { get; set; }                 
        public string FireStoreServiceKeyPath { get; set; }
        public FireStoreKeys FireStoreServiceKey { get; set; }
        public DynamoDBKeys DynamoDBServiceKey { get; set; }
       
    }
   
}
