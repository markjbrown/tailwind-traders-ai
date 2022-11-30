using Google.Cloud.Firestore;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class ProductFeature
    {
        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }
    }
}
