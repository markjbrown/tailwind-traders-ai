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
        private readonly string _brandCollection = "ProductBrand";
        private readonly string _typeCollection = "ProductType";
        private readonly string _tagCollection = "ProductTag";
        private readonly string _featureCollection = "ProductFeature";

        private readonly IMongoCollection<ProductItem> _productItem;
        private readonly IMongoCollection<ProductBrand> _productBrand;
        private readonly IMongoCollection<ProductType> _productType;
        private readonly IMongoCollection<ProductTag> _productTag;
        private readonly IMongoCollection<ProductFeature> _productFeature;
        private const int _take = 3;
        #endregion

        public AwsDocumentDbProductItemRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["DocumentDb:Host"]);
            var db = client.GetDatabase(configuration["DocumentDb:Database"]);
            _productItem = db.GetCollection<ProductItem>(_productCollection);
            _productBrand = db.GetCollection<ProductBrand>(_brandCollection);
            _productType = db.GetCollection<ProductType>(_typeCollection);
            _productTag = db.GetCollection<ProductTag>(_tagCollection);
            _productFeature = db.GetCollection<ProductFeature>(_featureCollection);
        }

        public async Task<List<Models.ProductItem>> FindProductsAsync(int[] brand, int[] type)
        {
            var items = await _productItem.FindAsync(item => brand.Contains(item.BrandId) || type.Contains(item.TypeId))?.Result?.ToListAsync();

            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(
                    _productBrand.AsQueryable(),
                    _productType.AsQueryable(),
                    _productFeature.AsQueryable(),
                    _productTag.AsQueryable());
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByIdsAsync(int[] ids)
        {
            var items = await _productItem.FindAsync(p => ids.Contains(p.Id))?.Result?.ToListAsync();
            items = items.Take(_take).ToList();
            items
                .Join(
                    _productBrand.AsQueryable(),
                    _productType.AsQueryable(),
                    _productFeature.AsQueryable(),
                    _productTag.AsQueryable());

            return items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var productTag = _productTag.FindAsync(t => t.Value == tag)?.Result?.SingleOrDefault();

            if (productTag == null)
            {
                return null;
            }

            var items = await _productItem.FindAsync(p => p.TagId == productTag.Id)?.Result?.ToListAsync();
            items = items.Take(_take).ToList();
            items
                .Join(
                    _productBrand.AsQueryable(),
                    _productType.AsQueryable(),
                    _productFeature.AsQueryable(),
                    _productTag.AsQueryable());

            return items;
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var items = await _productItem.FindAsync(_ => true)?.Result?.ToListAsync();

            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(
                    _productBrand.AsQueryable(),
                    _productType.AsQueryable(),
                    _productFeature.AsQueryable(),
                    _productTag.AsQueryable());
            return items;
        }

        public async Task<ProductItem> GetProductById(int productId)
        {
            var items = await _productItem.FindAsync(p => p.Id == productId)?.Result?.ToListAsync();

            items.Join(
                    _productBrand.AsQueryable(),
                    _productType.AsQueryable(),
                    _productFeature.AsQueryable(),
                    _productTag.AsQueryable());

            var item = items.FirstOrDefault();
            return item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var items = await _productItem.FindAsync(_ => true)?.Result?.ToListAsync();

            items = items.OrderBy(product => new Random().Next()).Take(_take).ToList();

            items.Join(
                    _productBrand.AsQueryable(),
                    _productType.AsQueryable(),
                    _productFeature.AsQueryable(),
                    _productTag.AsQueryable());

            return items;
        }

        public async Task<List<Models.ProductBrand>> GetAllBrandsAsync()
        {
            var brands = await _productBrand.FindAsync(_ => true)?.Result?.ToListAsync();
            return brands;
        }

        public async Task<List<Models.ProductType>> GetAllTypesAsync()
        {
            var types = await _productType.FindAsync(_ => true)?.Result?.ToListAsync();
            return types;
        }
    }
}
