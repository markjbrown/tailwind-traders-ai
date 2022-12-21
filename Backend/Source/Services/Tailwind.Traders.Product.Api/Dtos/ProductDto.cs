using System.Collections.Generic;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string ImageUrl { get; set; }

        public string BrandName { get; set; }

        public ProductType Type { get; set; }

        public IEnumerable<ProductFeature> Features { get; set; }
    }
}
