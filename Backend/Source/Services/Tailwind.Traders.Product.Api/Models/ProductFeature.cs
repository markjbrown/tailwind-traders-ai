using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tailwind.Traders.Product.Api.Models
{

    [FirestoreData]
    public class ProductFeature
    {

      [BsonId]
       public ObjectId? _id { get; set; }


        [FirestoreProperty]

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [FirestoreProperty]

        public int ProductItemId { get; set; }

        [FirestoreProperty]

        public string Title { get; set; }

        [FirestoreProperty]

        public string Description { get; set; }
    }
}
