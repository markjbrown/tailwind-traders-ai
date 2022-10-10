using CsvHelper.Configuration;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Mappers
{
    public class ProductBrandMap : ClassMap<ProductBrand>
    {
        public ProductBrandMap()
        {
            Map(p => p.Id);
            Map(p => p.Name);
        }
    }
}
