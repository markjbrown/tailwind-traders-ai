using Google.Cloud.Firestore;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class PrdFeature
    {
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public int ProductItemId { get; set; }
        [FirestoreProperty]
        public string Title { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
    }
}
