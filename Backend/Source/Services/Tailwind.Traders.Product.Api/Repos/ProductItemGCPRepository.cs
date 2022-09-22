using Google.Cloud.Firestore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Mappers;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Repos
{
    public class ProductItemGCPRepository : IProductItemRepository
    {
        #region DataMembers
        private readonly IHostEnvironment _env;
        FirestoreDb db;

        private readonly CollectionReference _productItemCollection;
        private readonly CollectionReference _brandCollection;
        private readonly CollectionReference _typeCollection;
        private readonly CollectionReference _tagCollection;
        private readonly CollectionReference _featureCollection;
        private readonly MapperGCPModels _mapperGCPModels;
        #endregion

        public ProductItemGCPRepository(IHostEnvironment env, MapperGCPModels mapperGCPModels)
        {
            _env = env;
            _mapperGCPModels = mapperGCPModels;
            // DB Authentication with serviceJson and initialization
            string keyPath = Path.Combine(_env.ContentRootPath, "Key\\serviceKey.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            db = FirestoreDb.Create("taliwind");
            
            // getting collections
            _productItemCollection = db.Collection(typeof(ProductItem).Name);
            _brandCollection = db.Collection(typeof(ProductBrand).Name);
            _typeCollection = db.Collection(typeof(ProductType).Name);
            _tagCollection = db.Collection(typeof(ProductTag).Name);
            _featureCollection = db.Collection(typeof(ProductFeature).Name);
        }

        public async Task<List<ProductItem>> FindProductsAsync(int[] brand, int[] type)
        {
            //var items = await _productItem.FindAsync(item => brand.Contains(item.BrandId) || type.Contains(item.TypeId))?.Result?.ToListAsync();
            var productItemSnapshot = await _productItemCollection.WhereArrayContains("BrandId", brand).GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<PrdItem>()).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();

            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Joins(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<PrdBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<PrdType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<PrdFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<PrdTag>()).AsQueryable()
                    );
            return null;//items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var tagSnapshot = await _tagCollection.WhereEqualTo("Value", tag).GetSnapshotAsync();
            var productTag = tagSnapshot.Documents.Select(x => x.ConvertTo<PrdTag>()).SingleOrDefault();


            if (productTag == null)
            {
                return null;
            }

            var productItemSnapshot = await _productItemCollection.WhereEqualTo("TagId", productTag.Id).GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<PrdItem>()).ToList();

            items = items.Take(3).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();

            items
                .Joins(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<PrdBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<PrdType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<PrdFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<PrdTag>()).AsQueryable()
                    );

            return null;//items;
        }

        public async Task<List<ProductBrand>> GetAllBrandsAsync()
        {
            var branhSnapshot = await _brandCollection.GetSnapshotAsync();
            var brands = branhSnapshot.Documents.Select(x => x.ConvertTo<PrdType>()).ToList();
            return null;//types;
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var productItemSnapshot = await _productItemCollection.GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<PrdItem>()).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();
            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Joins(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<PrdBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<PrdType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<PrdFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<PrdTag>()).AsQueryable()
                    );

            var d = _mapperGCPModels.MapperToProductItem(items);


            return _mapperGCPModels.MapperToProductItem(items).ToList();
        }

        public async Task<List<ProductType>> GetAllTypesAsync()
        {
            var typeSnapshot = await _typeCollection.GetSnapshotAsync();
            var types = typeSnapshot.Documents.Select(x => x.ConvertTo<PrdType>()).ToList();
            return null;//types;
        }

        public async Task<ProductItem> GetProductById(int productId)
        {
            var productItemSnapshot = await _productItemCollection.WhereEqualTo("Id", productId).GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<PrdItem>()).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();

            items.Joins(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<PrdBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<PrdType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<PrdFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<PrdTag>()).AsQueryable()
                    );

            var item = items.FirstOrDefault();
            return null;//item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var productItemSnapshot = await _productItemCollection.GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<PrdItem>()).ToList();
            items = items.OrderBy(product => new Random().Next()).Take(3).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();
            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Joins(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<PrdBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<PrdType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<PrdFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<PrdTag>()).AsQueryable()
                    );

            return null;
        }
    }
}
