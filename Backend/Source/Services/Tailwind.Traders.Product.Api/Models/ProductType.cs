using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductType
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
