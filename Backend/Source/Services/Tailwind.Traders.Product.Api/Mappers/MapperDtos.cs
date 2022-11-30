using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Tailwind.Traders.Product.Api.Dtos;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Mappers
{
    public class MapperDtos
    {
        private AppSettings _appSettings;

        public MapperDtos(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public IEnumerable<ProductDto> MapperToProductDto(IEnumerable<ProductItem> productItems)
        {
            return productItems.Select(item => MapperToProductDto(item));
        }

        public ProductDto MapperToProductDto(ProductItem productItem, bool isDetail = false)
        {
            return new ProductDto
            {
                BrandName = productItem.BrandName,
                Features = productItem.Features.Select(feature => MapperToProductFeatureDto(feature)),
                Id = productItem.ProductItemId,
                Name = productItem.Name,
                Price = productItem.Price,
                Type = MapperToProductTypeDto(productItem.Type),
                ImageUrl = isDetail ? 
                    $"{_appSettings.ProductDetailImagesUrl}/{productItem.ImageName}" :
                    $"{_appSettings.ProductImagesUrl}/{productItem.ImageName}"
            };
        }

        public IEnumerable<ProductTypeDto> MapperToProductTypeDto(IEnumerable<ProductType> productTypes)
        {
            var types = new List<ProductTypeDto>();

            foreach (var productType in productTypes)
            {
                types.Add(MapperToProductTypeDto(productType));
            }

            return types;
        }

        public ProductTypeDto MapperToProductTypeDto(ProductType productType)
        {
            return new ProductTypeDto
            {
                Code = productType.Code,
                Name = productType.Name
            };
        }

        public ProductFeatureDto MapperToProductFeatureDto(ProductFeature productFeature)
        {
            return new ProductFeatureDto
            {
                Description = productFeature.Description,
                Title = productFeature.Title
            };
        }
    }
}
