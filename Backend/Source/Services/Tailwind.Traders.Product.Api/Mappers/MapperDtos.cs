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
                Features = productItem.Features,
                Id = productItem.ProductItemId,
                Name = productItem.Name,
                Price = productItem.Price,
                Type = productItem.Type,
                ImageUrl = isDetail ? 
                    $"{_appSettings.ProductDetailImagesUrl}/{productItem.ImageName}" :
                    $"{_appSettings.ProductImagesUrl}/{productItem.ImageName}"
            };
        }
    }
}
