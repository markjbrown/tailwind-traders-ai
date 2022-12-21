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
        private readonly IFileProcessor _processFile;
        private readonly CollectionReference _profileCollection;

        public GcpProfileDatabaseSeeder(
            IFileProcessor processFile,
            IWebHostEnvironment env,
            IOptions<AppSettings> options)
        {
            _processFile = processFile;
            _env = env;
            var appSettings = options.Value;
            FirestoreDb db = GcpHelper.CreateDb(appSettings.Firestore);
            _profileCollection = db.Collection("Profiles");
        }

        public async Task SeedAsync()
        {
            var profiles = _processFile.Process<Models.Profile>(_env.ContentRootPath, "Profiles");

            foreach (var profile in profiles)
            {
                await AddDocumentIfNeeded(_profileCollection, profile);
            }
        }

        private static async Task AddDocumentIfNeeded(CollectionReference collection, Models.Profile profile)
        {
            var docResult = await collection.Select("Email").WhereEqualTo("Email", profile.Email).GetSnapshotAsync();
            if (docResult.Count == 0)
            {
                var doc = await collection.AddAsync(profile);
                var snpShot = await doc.GetSnapshotAsync();
                if (snpShot != null && !snpShot.Exists)
                {
                    await doc.SetAsync(profile);
                }
            }
        }
    }
}
