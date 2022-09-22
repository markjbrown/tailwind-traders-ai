using Google.Cloud.Firestore;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class PrdTag
    {
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public string Value { get; set; }
    }
}
