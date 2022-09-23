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
    public class ProductFeature
    {
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
        public int ProductItemId { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public string Title { get; set; }
#if GCP
        [FirestoreProperty]
#endif
        public string Description { get; set; }
    }
}
