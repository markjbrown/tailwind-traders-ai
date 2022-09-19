using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Extensions;

namespace Tailwind.Traders.Product.Api.Repos
{
    public class ProductItemAzureRepository : IProductItemRepository
    {
        private readonly ProductContext _productContext;
        public ProductItemAzureRepository(ProductContext productContext)
        {
            _productContext = productContext;
        }
        public async Task<List<Models.ProductItem>> FindProductsAsync(int[] brand, int[] type)
        {
            var items = await _productContext.ProductItems
                .Where(item => brand.Contains(item.BrandId) || type.Contains(item.TypeId))
                .ToListAsync();

            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(
                _productContext.ProductBrands,
                _productContext.ProductTypes,
                _productContext.ProductFeatures,
                _productContext.Tags);

            return items;
        }

        public async Task<List<Models.ProductItem>> FindProductsByTag(string tag)
        {
            var productTag = _productContext.Tags.SingleOrDefault(t => t.Value == tag);

            if (productTag == null)
            {
                return null;
            }

            var items = await _productContext.ProductItems.Where(p => p.TagId == productTag.Id).Take(3).ToListAsync();

            items.Join(
                _productContext.ProductBrands,
                _productContext.ProductTypes,
                _productContext.ProductFeatures,
                _productContext.Tags);

            return items;
        }

        public async Task<List<Models.ProductItem>> GetAllProductsAsync()
        {
            var items = await _productContext.ProductItems.ToListAsync();

            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(
                    _productContext.ProductBrands,
                    _productContext.ProductTypes,
                    _productContext.ProductFeatures,
                    _productContext.Tags);
            return items;
        }

        public async Task<Models.ProductItem> GetProductById(int productId)
        {
            var items = await _productContext.ProductItems.Where(p => p.Id == productId).ToListAsync();

            items.Join(
                _productContext.ProductBrands,
                _productContext.ProductTypes,
                _productContext.ProductFeatures,
                _productContext.Tags);

            var item = items.FirstOrDefault();
            return item;
        }

        public async Task<List<Models.ProductItem>> RecommendedProductsAsync()
        {
            var items = await _productContext.ProductItems.ToListAsync();

            items = items.OrderBy(product => new Random().Next()).Take(3).ToList();

            items.Join(
              _productContext.ProductBrands,
              _productContext.ProductTypes,
              _productContext.ProductFeatures,
              _productContext.Tags);

            return items;
        }

        public async Task<List<Models.ProductBrand>> GetAllBrandsAsync()
        {
            var brands = await _productContext.ProductBrands.ToListAsync();
            return brands;
        }

        public async Task<List<Models.ProductType>> GetAllTypesAsync()
        {
            var types = await _productContext.ProductTypes.ToListAsync();
            return types;
        }
    }
}
