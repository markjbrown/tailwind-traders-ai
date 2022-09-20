using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductFeature
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ProductItemId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
