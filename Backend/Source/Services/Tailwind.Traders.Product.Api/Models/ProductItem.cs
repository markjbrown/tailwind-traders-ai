using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tailwind.Traders.Product.Api.Models
{
#if GCP
    [FirestoreData]
#endif
    public class ProductItem
    {
        public ProductItem()
        {
            Features = new HashSet<ProductFeature>();
        }
#if AWS
        [BsonId]
        public ObjectId _id { get; set; }
#endif
#if GCP
        [FirestoreProperty]
#endif
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public string Name { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public float Price { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public string ImageName { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public int BrandId {get; set;}
#if GCP
        [FirestoreProperty]
#endif
        public ProductBrand Brand { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public int TypeId {get; set;}
#if GCP
        [FirestoreProperty]
#endif
        public ProductType Type { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public int? TagId { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public ProductTag Tag { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public virtual ICollection<ProductFeature> Features { get; set; }
    }
}
