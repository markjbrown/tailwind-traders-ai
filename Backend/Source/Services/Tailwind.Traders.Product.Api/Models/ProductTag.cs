using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tailwind.Traders.Product.Api.Models
{
#if GCP
    [FirestoreData]
#endif
    public class ProductTag
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
        public string Value { get; set; }
    }
}
