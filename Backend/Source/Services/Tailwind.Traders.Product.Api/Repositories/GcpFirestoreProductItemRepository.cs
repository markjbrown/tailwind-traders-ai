using Google.Cloud.Firestore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Repositories
{
    public class GcpFirestoreProductItemRepository : IProductItemRepository
    {
        private readonly CollectionReference _productItemCollection;
        private readonly CollectionReference _brandCollection;
        private readonly CollectionReference _typeCollection;
        private readonly CollectionReference _tagCollection;
        private readonly CollectionReference _featureCollection;

        public GcpFirestoreProductItemRepository(IOptions<AppSettings> options)
        {
            // DB Authentication with serviceJson and initialization
            FirestoreDb db = GcpHelper.CreateDb(options.Value.FireStoreServiceKey);

            // getting collections
            _productItemCollection = db.Collection(typeof(ProductItem).Name);
            _typeCollection = db.Collection(typeof(ProductType).Name);
            _featureCollection = db.Collection(typeof(ProductFeature).Name);
        }

        public async Task<List<ProductItem>> FindProductsAsync(string[] brands, string[] types)
        {
            var products = new List<ProductItem>();
            if (brands.Any())
            {
                var productItemsFromBrands = await _productItemCollection.WhereIn("BrandName", brands).GetSnapshotAsync();
                products.AddRange(productItemsFromBrands.Documents.Select(x => x.ConvertTo<ProductItem>()));
            }
            if (types.Any())
            {
                var productItemsFromTypes = await _productItemCollection.WhereIn("Type", types).GetSnapshotAsync();
                products.AddRange(productItemsFromTypes.Documents.Select(x => x.ConvertTo<ProductItem>()));
            }
            return products;
        }

        public async Task<List<ProductItem>> FindProductsByIdsAsync(int[] ids)
        {
            var productItemSnapshot = await _productItemCollection.WhereIn("Id", ids).GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();
            return items;
        }

        public async Task<List<ProductItem>> FindProductsByTag(string tag)
        {
            var productItemSnapshot = await _productItemCollection.WhereArrayContains("Tags", tag).GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();
            return items;
        }

        public async Task<List<string>> GetAllBrandsAsync()
        {
            var productsSnapshot = await _productItemCollection.GetSnapshotAsync();
            var brands = productsSnapshot.Documents.Select(x => x.GetValue<string>(nameof(ProductItem.BrandName))).ToList();
            return brands;
        }

        public async Task<List<ProductItem>> GetAllProductsAsync()
        {
            var productItemSnapshot = await _productItemCollection.GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();
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
            var item = items.SingleOrDefault();
            return item;
        }

        public async Task<List<ProductItem>> RecommendedProductsAsync()
        {
            var productItemSnapshot = await _productItemCollection.GetSnapshotAsync();
            var items = productItemSnapshot.Documents.Select(x => x.ConvertTo<ProductItem>()).ToList();
            items = items.OrderBy(product => new Random().Next()).Take(3).ToList();
            return items;
        }
    }
}
