using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Models;
using Tailwind.Traders.Product.Api.Extensions;
using Microsoft.Extensions.Configuration;

namespace Tailwind.Traders.Product.Api.Repositories
{
    public class AwsDocumentDbProductItemRepository : IProductItemRepository
    {
        #region DataMember
        private readonly string _productCollection = "ProductItem";
        private readonly string _typeCollection = "ProductType";
        private readonly string _featureCollection = "ProductFeature";

        private readonly IMongoCollection<ProductItem> _productItem;
        private readonly IMongoCollection<ProductType> _productType;
        private readonly IMongoCollection<ProductFeature> _productFeature;
        private const int _take = 3;
        #endregion

        public AwsDocumentDbProductItemRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["DocumentDb:Host"]);
            var db = client.GetDatabase(configuration["DocumentDb:Database"]);
            _productItem = db.GetCollection<ProductItem>(_productCollection);
            _productType = db.GetCollection<ProductType>(_typeCollection);
            _productFeature = db.GetCollection<ProductFeature>(_featureCollection);
        }

        public async Task<List<ProductItem>> FindProductsAsync(string[] brands, string[] types)
        {
            var items = await _productItem.FindAsync(item => brands.Contains(item.BrandName) || types.Contains(item.Type.Name))?.Result?.ToListAsync();
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByIdsAsync(int[] ids)
        {
            var items = await _productItem.FindAsync(p => ids.Contains(p.ProductItemId))?.Result?.ToListAsync();
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var items = await _productItem.FindAsync(p => p.Tags.Contains(tag))?.Result?.ToListAsync();
            items = items.Take(_take).ToList();
            return items;
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var items = await _productItem.FindAsync(_ => true)?.Result?.ToListAsync();
            return items;
        }

        public async Task<ProductItem> GetProductById(int productId)
        {
            var items = await _productItem.FindAsync(p => p.ProductItemId == productId)?.Result?.ToListAsync();
            var item = items.SingleOrDefault();
            return item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var items = await _productItem.FindAsync(_ => true)?.Result?.ToListAsync();
            items = items.OrderBy(product => new Random().Next()).Take(_take).ToList();
            return items;
        }

        public async Task<List<ProductBrand>> GetAllBrandsAsync()
        {
            var brands = await _productItem.FindAsync(_ => true)?.Result?.ToListAsync();
            return brands.Select(p => new ProductBrand { Name = p.BrandName })
                .Distinct().ToList();
        }

        public async Task<List<ProductType>> GetAllTypesAsync()
        {
            var types = await _productType.FindAsync(_ => true)?.Result?.ToListAsync();
            return types.DistinctBy(x => x.Code).ToList();
        }
    }
}
