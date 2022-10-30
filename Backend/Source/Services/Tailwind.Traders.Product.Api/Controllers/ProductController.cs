using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Dtos;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Mappers;
using Tailwind.Traders.Product.Api.Repositories;

namespace Tailwind.Traders.Product.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly MapperDtos _mapperDtos;
        private readonly AppSettings _settings;
        private readonly IProductItemRepository _productItemRepository;

        public ProductController(IProductItemRepository productItemRepository, ILogger<ProductController> logger, MapperDtos mapperDtos, IOptions<AppSettings> options)
        {
            _logger = logger;
            _mapperDtos = mapperDtos;
            _settings = options.Value;
            _productItemRepository = productItemRepository;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AllProductsAsync()
        {
            var items = await _productItemRepository.GetAllProductsAsync();

            if (!items.Any())
            {
                _logger.LogDebug("Products empty");
                return NoContent();
            }

            return Ok(_mapperDtos.MapperToProductDto(items));
        }

        [HttpGet]
        [Route("{productId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ProductByIdAsync(int productId)
        {
            var item = await _productItemRepository.GetProductById(productId);
            if (item == null)
            {
                _logger.LogDebug($"Product with id '{productId}', not found");

                return NotFound();
            }

            return Ok(_mapperDtos.MapperToProductDto(item, isDetail: true));
        }

        [HttpGet("ids")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> FindProductAsync([FromQuery] int[] id)
        {
            var items = await _productItemRepository.FindProductsByIdsAsync(id);

            if (!items.Any())
            {
                _logger.LogDebug("No Products with these IDs");

                return NoContent();
            }

            return Ok(_mapperDtos.MapperToProductDto(items));
        }


        [HttpGet("filter")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> FindProductAsync([FromQuery] int[] brand, [FromQuery] int[] type)
        {
            var items = await _productItemRepository.FindProductsAsync(brand, type);

            if (!items.Any())
            {
                _logger.LogDebug("No Products for this criteria");

                return NoContent();
            }

            return Ok(_mapperDtos.MapperToProductDto(items));
        }

        [HttpGet("tag/{tag}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> FindProductsByTag(string tag)
        {
            var items = await _productItemRepository.FindProductsByTag(tag);
            if (items == null)
            {
                _logger.LogDebug("No tag found: " + tag);
                return NoContent();
            }

            var data = items.Select(p => new ClassifiedProductDto()
            {
                Id = p.Id,
                ImageUrl = $"{_settings.ProductImagesUrl}/{p.ImageName}",
                Name = p.Name,
                Price = p.Price
            });

            if (!data.Any())
            {
                _logger.LogDebug("No products found with the tag: " + tag);
                return NoContent();
            }

            return Ok(data);
        }

        [HttpGet("recommended")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> RecommendedProductsAsync()
        {
            var items = await _productItemRepository.RecommendedProductsAsync();
            if (!items.Any())
            {
                _logger.LogDebug("There are no recommended products");

                return NoContent();
            }
            return Ok(_mapperDtos.MapperToProductDto(items));
        }
    }
}