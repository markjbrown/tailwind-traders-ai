using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class ProductItem
    {
        public ProductItem()
        {
            Features = new List<ProductFeature>();
        }

        [FirestoreProperty]
        public int ProductItemId { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public float Price { get; set; }

        [FirestoreProperty]
        public string ImageName { get; set; }

        [FirestoreProperty]
        public string BrandName {get; set;}

        [FirestoreProperty]
        public ProductType Type { get; set; }

        [FirestoreProperty]
        public IReadOnlyList<string> Tags { get; set; }

        [FirestoreProperty]
        public List<ProductFeature> Features { get; set; }
    }
}
