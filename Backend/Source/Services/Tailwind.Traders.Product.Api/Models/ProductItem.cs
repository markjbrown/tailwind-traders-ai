using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class ProductItem : IHaveId
    {
        public ProductItem()
        {
            Features = new HashSet<ProductFeature>();
        }

        [BsonId]
        public ObjectId? _id { get; set; }

        [FirestoreProperty]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public float Price { get; set; }

        [FirestoreProperty]
        public string ImageName { get; set; }

        [FirestoreProperty]
        public int BrandId {get; set;}

        [FirestoreProperty]
        public ProductBrand Brand { get; set; }

        [FirestoreProperty]
        public int TypeId {get; set;}

        [FirestoreProperty]
        public ProductType Type { get; set; }

        [FirestoreProperty]
        public int? TagId { get; set; }

        [FirestoreProperty]
        public ProductTag Tag { get; set; }

        [FirestoreProperty]
        public virtual ICollection<ProductFeature> Features { get; set; }
    }
}
