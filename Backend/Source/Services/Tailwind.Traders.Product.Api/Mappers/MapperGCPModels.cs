using System.Collections.Generic;
using System.Linq;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Mappers
{
    public class MapperGCPModels
    {
        public IEnumerable<ProductItem> MapperToProductItem(IEnumerable<PrdItem> prdItems)
        {
            var products = new List<ProductItem>();

            foreach (var prdItem in prdItems)
            {
                products.Add(MapperToProductItem(prdItem));
            }

            return products;
        }

        public ProductItem MapperToProductItem(PrdItem productItem, bool isDetail = false)
        {
            return new ProductItem
            {
                Brand = MapperToProductBrand(productItem.Brand),
                //Features = (ICollection<ProductFeature>)productItem.Features.Select(feature => MapperToProductFeature(feature)),
                Id = productItem.Id,
                Name = productItem.Name,
                Price = productItem.Price,
                Type = productItem.Type is not null ? MapperToProductType(productItem.Type) : null,
                BrandId = productItem.BrandId,
                Tag = productItem.Tag is not null ?MapperToProductTag(productItem.Tag) : null,
                TagId = productItem.TagId,
                ImageName = productItem.ImageName
            };
        }

        public IEnumerable<ProductBrand> MapperToProductBrand(IEnumerable<PrdBrand> productBrands)
        {
            var brands = new List<ProductBrand>();

            foreach (var productBrand in productBrands)
            {
                brands.Add(MapperToProductBrand(productBrand));
            }

            return brands;
        }

        public ProductBrand MapperToProductBrand(PrdBrand productBrand)
        {
            return new ProductBrand
            {
                Id = productBrand.Id,
                Name = productBrand.Name
            };
        }

        public IEnumerable<ProductType> MapperToProductType(IEnumerable<PrdType> productTypes)
        {
            var types = new List<ProductType>();

            foreach (var productType in productTypes)
            {
                types.Add(MapperToProductType(productType));
            }

            return types;
        }

        public ProductType MapperToProductType(PrdType productType)
        {
            return new ProductType
            {
                Id = productType.Id,
                Code = productType.Code,
                Name = productType.Name
            };
        }

        public ProductFeature MapperToProductFeature(PrdFeature productFeature)
        {
            return new ProductFeature
            {
                Id = productFeature.Id,
                Description = productFeature.Description,
                ProductItemId = productFeature.ProductItemId,
                Title = productFeature.Title
            };
        }

        public IEnumerable<ProductTag> MapperToProductTag(IEnumerable<PrdTag> productTags)
        {
            var tag = new List<ProductTag>();

            foreach (var productTag in productTags)
            {
                tag.Add(MapperToProductTag(productTag));
            }

            return tag;
        }

        public ProductTag MapperToProductTag(PrdTag productTag)
        {
            return new ProductTag
            {
                Id = productTag.Id,
                Value = productTag.Value
            };
        }
    }
}
