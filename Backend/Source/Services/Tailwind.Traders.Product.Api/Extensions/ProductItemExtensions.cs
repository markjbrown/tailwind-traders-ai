using System.Collections.Generic;
using System.Linq;
using Tailwind.Traders.Product.Api.Mappers;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Extensions
{
    public static class ProductItemExtensions
    {
        public static IEnumerable<ProductItem> Join(this IEnumerable<ProductItemSeed> productItems,
            IEnumerable<ProductBrandSeed> productBrands,
            IEnumerable<ProductTypeSeed> productTypes,
            IEnumerable<ProductFeatureSeed> productFeatures,
            IEnumerable<ProductTagSeed> tags)
        {
            return productItems.Select(item =>
            {
                var productItem = item.ToProductItem();
                productItem.BrandName = productBrands.SingleOrDefault(brand => brand.Id == item.BrandId)?.Name;
                productItem.Type = productTypes.SingleOrDefault(type => type.Id == item.TypeId)?.ToProductType();
                productItem.Features = productFeatures
                    .Where(feature => feature.ProductItemId == item.Id)
                    .Select(seed => seed.ToProductFeature())
                    .ToList();
                if (item.TagId != null)
                {
                    productItem.Tags = new[]
                    {
                        tags.SingleOrDefault(t => t.Id == item.TagId).Value
                    };
                }
                return productItem;
            });
        }
    }
}
