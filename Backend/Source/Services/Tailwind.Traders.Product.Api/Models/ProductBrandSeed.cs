using Google.Cloud.Firestore;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class ProductBrandSeed : IHaveId
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
