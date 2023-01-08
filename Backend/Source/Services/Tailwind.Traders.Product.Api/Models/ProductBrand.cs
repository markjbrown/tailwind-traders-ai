using Google.Cloud.Firestore;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class ProductBrand
    {
        [FirestoreProperty]
        public string Code { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }
    }
}
