using Google.Cloud.Firestore;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class PrdBrand
    {
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
    }
}
