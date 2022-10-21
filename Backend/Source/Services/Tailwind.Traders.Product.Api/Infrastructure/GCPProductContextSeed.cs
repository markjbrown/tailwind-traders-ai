using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class GCPProductContextSeed : IContextSeed
    {
        private readonly IHostEnvironment _env;
        private readonly IProcessFile _processFile;
        private readonly CollectionReference _productItemCollection;
        private readonly CollectionReference _brandCollection;
        private readonly CollectionReference _typeCollection;
        private readonly CollectionReference _tagCollection;
        private readonly CollectionReference _featureCollection;

        public GCPProductContextSeed(IProcessFile processFile, IWebHostEnvironment env, IOptions<AppSettings> appSettings)
        {
            _processFile = processFile;
            _env = env;

            FirestoreDb db = GcpHelper.CreateDb(appSettings.Value.FireStoreServiceKey);

            // getting collections
            _productItemCollection = db.Collection(appSettings.Value.PRODUCT_PRODUCTITEM_TABLE);
            _brandCollection = db.Collection(appSettings.Value.PRODUCT_BRAND_TABLE);
            _typeCollection = db.Collection(appSettings.Value.PRODUCT_TYPE_TABLE);
            _tagCollection = db.Collection(appSettings.Value.PRODUCT_TAG_TABLE);
            _featureCollection = db.Collection(appSettings.Value.PRODUCT_FEATURE_TABLE);
        }

        public async Task SeedAsync()
        {
            var brands = _processFile.Process<ProductBrand>(_env.ContentRootPath, "ProductBrands");
            var types = _processFile.Process<ProductType>(_env.ContentRootPath, "ProductTypes");
            var features = _processFile.Process<ProductFeature>(_env.ContentRootPath, "ProductFeatures");
            var products = _processFile.Process<ProductItem>(_env.ContentRootPath, "ProductItems", new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });
            var tags = _processFile.Process<ProductTag>(_env.ContentRootPath, "ProductTags");

            foreach (var prodBrand in brands)
            {
                await AddDocumentIfNeeded(_brandCollection, prodBrand);
            }

            foreach (var prodType in types)
            {
                await AddDocumentIfNeeded(_typeCollection, prodType);
            }

            foreach (var prodFeature in features)
            {
                await AddDocumentIfNeeded(_featureCollection, prodFeature);
            }

            foreach (var prodTag in tags)
            {
                await AddDocumentIfNeeded(_tagCollection, prodTag);
            }

            foreach (var prodItem in products)
            {
                await AddDocumentIfNeeded(_productItemCollection, prodItem);
            }

        }

        private static async Task AddDocumentIfNeeded(CollectionReference collection, IHaveId item)
        {
            var docResult = await collection.Select("Id").WhereEqualTo("Id", item.Id).GetSnapshotAsync();
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
