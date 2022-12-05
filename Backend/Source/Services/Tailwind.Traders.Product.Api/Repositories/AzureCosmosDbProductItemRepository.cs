using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Repositories
{
    public class AzureCosmosDbProductItemRepository : IProductItemRepository
    {
        private readonly ProductContext _productContext;
        private const int _take = 3;
        public AzureCosmosDbProductItemRepository(ProductContext productContext)
        {
            _productContext = productContext;
        }
        public async Task<List<Models.ProductItem>> FindProductsAsync(int[] brand, int[] type)
        {
            var items = await _productContext.ProductItems
                .Where(item => brand.Contains(item.BrandId) || type.Contains(item.TypeId))
                .ToListAsync();

            items
                .OrderBy(inc => inc.Id)
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

            var items = await _productContext.ProductItems.Where(p => p.TagId == productTag.Id).Take(_take).ToListAsync();

            items.Join(
                _productContext.ProductBrands,
                _productContext.ProductTypes,
                _productContext.ProductFeatures,
                _productContext.Tags);

            return items;
        }

        public async Task<List<Models.ProductItem>> GetAllProductsAsync()
        {
            var items = await _productContext.ProductItems.AsQueryable().ToListAsync();

            items
                .OrderBy(inc => inc.Id)
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
            var items = await _productContext.ProductItems.AsQueryable().ToListAsync();

            items = items
                .OrderBy(product => new Random().Next()).Take(_take)
                .ToList();

            items.Join(
              _productContext.ProductBrands,
              _productContext.ProductTypes,
              _productContext.ProductFeatures,
              _productContext.Tags);

            return items;
        }

        public async Task<List<Models.ProductBrand>> GetAllBrandsAsync()
        {
            var brands = await _productContext.ProductBrands.AsQueryable()
                .ToListAsync();
            return brands;
        }

        public async Task<List<Models.ProductType>> GetAllTypesAsync()
        {
            var types = await _productContext.ProductTypes.AsQueryable()
                .ToListAsync();
            return types;
        }

        public async Task<List<ProductItem>> FindProductsByIdsAsync(int[] ids)
        {
            var items = await _productContext.ProductItems.Where(p => ids.Contains(p.Id)).Take(_take).ToListAsync();

            items.Join(
                _productContext.ProductBrands,
                _productContext.ProductTypes,
                _productContext.ProductFeatures,
                _productContext.Tags);

            return items;
        }
    }
}
