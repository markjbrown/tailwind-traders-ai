using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class GcpProductDatabaseSeeder : ISeedDatabase
    {
        private readonly IHostEnvironment _env;
        private readonly IProcessFile _processFile;
        private readonly CollectionReference _productItemCollection;
        private readonly CollectionReference _brandCollection;
        private readonly CollectionReference _typeCollection;
        private readonly CollectionReference _tagCollection;
        private readonly CollectionReference _featureCollection;

        public GcpProductDatabaseSeeder(IProcessFile processFile, IWebHostEnvironment env, IOptions<AppSettings> appSettings)
        {
            _processFile = processFile;
            _env = env;

            FirestoreDb db = GcpHelper.CreateDb(appSettings.Value.FireStoreServiceKey);

            // getting collections
            _productItemCollection = db.Collection(typeof(ProductItem).Name);
            _typeCollection = db.Collection(typeof(ProductType).Name);
            _featureCollection = db.Collection(typeof(ProductFeature).Name);
        }

        public async Task ResetAsync()
        {
            var products = _processFile.Process<ProductItemSeed>(_env.ContentRootPath, "ProductItems",
                new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });

            foreach (var product in products)
            {
                DocumentReference docRef = _productItemCollection.Document(product.Id.ToString());
                await docRef.DeleteAsync();
            }
        }

        public async Task SeedAsync()
        {
            var brands = _processFile.Process<ProductBrandSeed>(_env.ContentRootPath, "ProductBrands");
            var types = _processFile.Process<ProductTypeSeed>(_env.ContentRootPath, "ProductTypes");
            var features = _processFile.Process<ProductFeatureSeed>(_env.ContentRootPath, "ProductFeatures");
            var tags = _processFile.Process<ProductTagSeed>(_env.ContentRootPath, "ProductTags");
            var products = _processFile.Process<ProductItemSeed>(_env.ContentRootPath, "ProductItems",
                new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });

            var productItems = ProductItemExtensions.Join(products, brands, types, features, tags);
            foreach (var productItem in productItems)
            {
                await AddDocumentIfNeeded(_productItemCollection, productItem);
            }
        }

        private static async Task AddDocumentIfNeeded(CollectionReference collection, ProductItem item)
        {
            var docResult = await collection.Select(nameof(ProductItem.ProductItemId))
                .WhereEqualTo(nameof(ProductItem.ProductItemId), item.ProductItemId).GetSnapshotAsync();
            if (docResult.Count == 0)
            {
                var doc = await collection.AddAsync(item);
                var snpShot = await doc.GetSnapshotAsync();
                if (snpShot != null && !snpShot.Exists)
                {
                    await doc.SetAsync(item);
                }
            }
        }
    }
}
