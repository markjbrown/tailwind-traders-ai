using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class PrdItem
    {
        public PrdItem()
        {
            Features = new HashSet<PrdFeature>();
        }
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public float Price { get; set; }
        [FirestoreProperty]
        public string ImageName { get; set; }
        [FirestoreProperty]
        public int BrandId { get; set; }
        [FirestoreProperty]
        public PrdBrand Brand { get; set; }
        [FirestoreProperty]
        public int TypeId { get; set; }
        [FirestoreProperty]
        public PrdType Type { get; set; }
        [FirestoreProperty]
        public int? TagId { get; set; }
        [FirestoreProperty]
        public PrdTag Tag { get; set; }
        public virtual ICollection<PrdFeature> Features { get; set; }
    }
}
