using System.Collections.Generic;

namespace Tailwind.Traders.WebBff.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string ImageUrl { get; set; }

        public string BrandName { get; set; }

        public ProductType Type { get; set; }

        public IEnumerable<ProductFeature> Features { get; set; }

        public int StockUnits { get; set; }
    }
}
