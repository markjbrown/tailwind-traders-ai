using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tailwind.Traders.Product.Api.Models
{
    [FirestoreData]
    public class PrdType
    {
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public string Code { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
    }
}
