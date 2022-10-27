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

        public MapperDtos(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public IEnumerable<ProductDto> MapperToProductDto(IEnumerable<ProductItem> productItems)
        {
            var products = new List<ProductDto>();

            foreach(var productItem in productItems)
            {
                products.Add(MapperToProductDto(productItem));
            }

            return products;
        }

        public ProductDto MapperToProductDto(ProductItem productItem, bool isDetail = false)
        {
            return new ProductDto
            {
                Brand = MapperToProductBrandDto(productItem.Brand),
                Features = productItem.Features.Select(feature => MapperToProductFeatureDto(feature)),
                Id = productItem.Id,
                Name = productItem.Name,
                Price = productItem.Price,
                Type = MapperToProductTypeDto(productItem.Type),
                ImageUrl = isDetail ? 
                    $"{_appSettings.ProductDetailImagesUrl}/{productItem.ImageName}" :
                    $"{_appSettings.ProductImagesUrl}/{productItem.ImageName}"
            };
        }

        public IEnumerable<ProductBrandDto> MapperToProductBrandDto(IEnumerable<ProductBrand> productBrands)
        {
            var brands = new List<ProductBrandDto>();

            foreach (var productBrand in productBrands)
            {
                brands.Add(MapperToProductBrandDto(productBrand));
            }

            return brands;
        }

        public ProductBrandDto MapperToProductBrandDto(ProductBrand productBrand)
        {
            return new ProductBrandDto
            {
                Id = productBrand.Id,
                Name = productBrand.Name
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
                Id = productType.Id,
                Code = productType.Code,
                Name = productType.Name
            };
        }

        public ProductFeatureDto MapperToProductFeatureDto(ProductFeature productFeature)
        {
            return new ProductFeatureDto
            {
                Id = productFeature.Id,
                Description = productFeature.Description,
                Title = productFeature.Title
            };
        }
    }
}
