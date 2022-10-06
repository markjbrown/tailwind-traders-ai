using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class GCPProductContextSeed : IContextSeed
    {

        private readonly IHostEnvironment _env;
        private readonly IProcessFile _processFile;
        FirestoreDb db;

        private readonly CollectionReference _productItemCollection;
        private readonly CollectionReference _brandCollection;
        private readonly CollectionReference _typeCollection;
        private readonly CollectionReference _tagCollection;
        private readonly CollectionReference _featureCollection;
        public GCPProductContextSeed(IProcessFile processFile, IWebHostEnvironment env, IOptions<AppSettings> appSettings)
        {
            _env = env;
            _processFile = processFile;
            // DB Authentication with serviceJson and initialization
            string keyPath = Path.Combine(_env.ContentRootPath, "Key\\serviceKey.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            db = FirestoreDb.Create(appSettings.Value.FireStoreProjectId);

            // getting collections
            _productItemCollection = db.Collection(typeof(ProductItem).Name);
            _brandCollection = db.Collection(typeof(ProductBrand).Name);
            _typeCollection = db.Collection(typeof(ProductType).Name);
            _tagCollection = db.Collection(typeof(ProductTag).Name);
            _featureCollection = db.Collection(typeof(ProductFeature).Name);
        }
        public async Task SeedAsync()
        {
            var brands = _processFile.Process<ProductBrand>(_env.ContentRootPath, "ProductBrands");
            var types = _processFile.Process<ProductType>(_env.ContentRootPath, "ProductTypes");
            var features = _processFile.Process<ProductFeature>(_env.ContentRootPath, "ProductFeatures");
            var products = _processFile.Process<ProductItem>(_env.ContentRootPath, "ProductItems", new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });
            var tags = _processFile.Process<ProductTag>(_env.ContentRootPath, "ProductTags");

            ProductItemExtensions.Join(products, brands, types, features, tags);

            foreach (var prodBrand in brands)
            {
                var docResult = await _brandCollection.Select("Id").WhereEqualTo("Id", prodBrand.Id).GetSnapshotAsync();
                if (docResult.Count == 0)
                {
                    var doc = await _brandCollection.AddAsync(prodBrand);
                    var snpShot = await doc.GetSnapshotAsync();
                    if (snpShot != null && !snpShot.Exists)
                    {
                        await doc.SetAsync(prodBrand);
                    }
                }
            }

            foreach (var prodType in types)
            {
                var docResult = await _typeCollection.Select("Id").WhereEqualTo("Id", prodType.Id).GetSnapshotAsync();
                if (docResult.Count == 0)
                {
                    var doc = await _typeCollection.AddAsync(prodType);
                    var snpShot = await doc.GetSnapshotAsync();
                    if (snpShot != null && !snpShot.Exists)
                    {
                        await doc.SetAsync(prodType);
                    }
                }
            }

            foreach (var prodFeature in features)
            {
                var docResult = await _featureCollection.Select("Id").WhereEqualTo("Id", prodFeature.Id).GetSnapshotAsync();
                if (docResult.Count == 0)
                {
                    var doc = await _featureCollection.AddAsync(prodFeature);
                    var snpShot = await doc.GetSnapshotAsync();
                    if (snpShot != null && !snpShot.Exists)
                    {
                        await doc.SetAsync(prodFeature);
                    }
                }
            }

            foreach (var prodTag in tags)
            {
                var docResult = await _tagCollection.Select("Id").WhereEqualTo("Id", prodTag.Id).GetSnapshotAsync();
                if (docResult.Count == 0)
                {
                    var doc = await _tagCollection.AddAsync(prodTag);
                    var snpShot = await doc.GetSnapshotAsync();
                    if (snpShot != null && !snpShot.Exists)
                    {
                        await doc.SetAsync(prodTag);
                    }
                }
            }

            foreach (var prodItem in products)
            {
                var docResult = await _productItemCollection.Select("Id").WhereEqualTo("Id", prodItem.Id).GetSnapshotAsync();
                if (docResult.Count == 0)
                {
                    var doc = await _productItemCollection.AddAsync(prodItem);
                    var snpShot = await doc.GetSnapshotAsync();
                    if (snpShot != null && !snpShot.Exists)
                    {
                        await doc.SetAsync(prodItem);
                    }
                }
            }

        }
    }
}
