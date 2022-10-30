using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public class GcpProfileDatabaseSeeder : ISeedDatabase
    {
        private readonly IHostEnvironment _env;
        private readonly IProcessFile _processFile;
        private readonly CollectionReference _profileCollection;

        public GcpProfileDatabaseSeeder(IProcessFile processFile, IWebHostEnvironment env, IOptions<AppSettings> appSettings)
        {
            _processFile = processFile;
            _env = env;

            FirestoreDb db = GcpHelper.CreateDb(appSettings.Value.Firestore);
            _profileCollection = db.Collection(typeof(Profiles).Name);
        }

        public async Task SeedAsync()
        {
            var profiles = _processFile.Process<Profiles>(_env.ContentRootPath, typeof(Profiles).Name);

            foreach (var profile in profiles)
            {
                await AddDocumentIfNeeded(_profileCollection, profile);
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
