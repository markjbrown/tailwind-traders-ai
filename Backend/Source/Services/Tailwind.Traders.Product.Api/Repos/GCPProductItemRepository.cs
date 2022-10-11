using Google.Cloud.Firestore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Mappers;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Repos
{
    public class GCPProductItemRepository : IProductItemRepository
    {
        #region DataMembers
        private readonly IHostEnvironment _env;
        FirestoreDb db;

        private readonly CollectionReference _productItemCollection;
        private readonly CollectionReference _brandCollection;
        private readonly CollectionReference _typeCollection;
        private readonly CollectionReference _tagCollection;
        private readonly CollectionReference _featureCollection;
        #endregion

        public GCPProductItemRepository(IOptions<AppSettings> appSettings)
        {
            // DB Authentication with serviceJson and initialization
            FirestoreDb db = GcpHelper.CreateDb(appSettings.Value.FireStoreServiceKey);

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
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();

            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<ProductBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<ProductType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<ProductFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<ProductTag>()).AsQueryable()
                    );
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var tagSnapshot = await _tagCollection.WhereEqualTo("Value", tag).GetSnapshotAsync();
            var productTag = tagSnapshot.Documents.Select(x => x.ConvertTo<ProductTag>()).SingleOrDefault();
            if (productTag == null)
            {
                return null;
            }

            var productItemSnapshot = await _productItemCollection.WhereEqualTo("TagId", productTag.Id).GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();

            items = items.Take(3).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();

            items
                .Join(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<ProductBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<ProductType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<ProductFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<ProductTag>()).AsQueryable()
                    );

            return items;
        }

        public async Task<List<ProductBrand>> GetAllBrandsAsync()
        {
            var branhSnapshot = await _brandCollection.GetSnapshotAsync();
            var brands = branhSnapshot.Documents.Select(x => x.ConvertTo<ProductBrand>()).ToList();
            return brands;
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var productItemSnapshot = await _productItemCollection.GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();
            
            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<ProductBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<ProductType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<ProductFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<ProductTag>()).AsQueryable()
                    );


            return items;
        }

        public async Task<List<ProductType>> GetAllTypesAsync()
        {
            var typeSnapshot = await _typeCollection.GetSnapshotAsync();
            var types = typeSnapshot.Documents.Select(x => x.ConvertTo<ProductType>()).ToList();
            return types;
        }

        public async Task<ProductItem> GetProductById(int productId)
        {
            var productItemSnapshot = await _productItemCollection.WhereEqualTo("Id", productId).GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();

            items.Join(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<ProductBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<ProductType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<ProductFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<ProductTag>()).AsQueryable()
                    );

            var item = items.FirstOrDefault();
            return item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var productItemSnapshot = await _productItemCollection.GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();
            items = items.OrderBy(product => new Random().Next()).Take(3).ToList();

            var prdBrandSnapshot = await _brandCollection.GetSnapshotAsync();
            var prdTypeSnapshot = await _typeCollection.GetSnapshotAsync();
            var prdFeatureSnapshot = await _featureCollection.GetSnapshotAsync();
            var prdTagSnapshot = await _tagCollection.GetSnapshotAsync();
            items
                .OrderByDescending(inc => inc.Name.Contains("gnome"))
                .Join(
                    prdBrandSnapshot.Documents.Select(x => x.ConvertTo<ProductBrand>()).AsQueryable(),
                    prdTypeSnapshot.Documents.Select(x => x.ConvertTo<ProductType>()).AsQueryable(),
                    prdFeatureSnapshot.Documents.Select(x => x.ConvertTo<ProductFeature>()).AsQueryable(),
                    prdTagSnapshot.Documents.Select(x => x.ConvertTo<ProductTag>()).AsQueryable()
                    );

            return null;
        }
    }
}
