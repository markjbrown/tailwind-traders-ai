using CsvHelper.Configuration;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Mappers
{
    public class ProductItemMap : ClassMap<ProductItem>
    {
        public ProductItemMap()
        {
            Map(p => p.ProductItemId);
            Map(p => p.Name);
            Map(p => p.Price);
            Map(p => p.ImageName);
            Map(p => p.BrandName);
            Map(p => p.Type);
            Map(p => p.Tags);
        }
    }
}
