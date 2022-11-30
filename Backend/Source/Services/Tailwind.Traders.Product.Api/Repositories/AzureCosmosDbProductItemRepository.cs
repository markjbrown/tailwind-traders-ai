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
        public async Task<List<Models.ProductItem>> FindProductsAsync(string[] brands, string[] types)
        {
            var items = await _productContext.ProductItems
                .Where(item => brands.Contains(item.BrandName) || types.Contains(item.Type.Name))
                .ToListAsync();
            return items;
        }

        public async Task<List<Models.ProductItem>> FindProductsByTag(string tag)
        {
            var items = await _productContext.ProductItems.Where(p => p.Tags.Contains(tag))
                .Take(_take).ToListAsync();
            return items;
        }

        public async Task<List<Models.ProductItem>> GetAllProductsAsync()
        {
            var items = await _productContext.ProductItems.AsQueryable().ToListAsync();
            return items;
        }

        public async Task<Models.ProductItem> GetProductById(int productId)
        {
            var items = await _productContext.ProductItems.Where(p => p.ProductItemId == productId).ToListAsync();
            var item = items.SingleOrDefault();
            return item;
        }

        public async Task<List<Models.ProductItem>> RecommendedProductsAsync()
        {
            //TODOL this does not seem right, is this used?
            var items = await _productContext.ProductItems.AsQueryable()
                .ToListAsync();
            items = items.Take(_take).ToList();
            return items;
        }

        public async Task<List<string>> GetAllBrandsAsync()
        {
            var brands = await _productContext.ProductItems.AsQueryable().Select(pi => pi.BrandName).ToListAsync();
            return brands;
        }

        public async Task<List<Models.ProductType>> GetAllTypesAsync()
        {
            var types = await _productContext.ProductItems.AsQueryable().Select(pi => pi.Type).ToListAsync();
            return types;
        }

        public async Task<List<ProductItem>> FindProductsByIdsAsync(int[] ids)
        {
            var items = await _productContext.ProductItems.Where(p => ids.Contains(p.ProductItemId))
                .Take(_take).ToListAsync();
            return items;
        }
    }
}
