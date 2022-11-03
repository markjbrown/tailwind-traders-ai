using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.Csv;
using Tailwind.Traders.Profile.Api.Helpers;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public class AzureProfileDatabaseSeeder : ISeedDatabase
    {
        private readonly CsvReaderHelper _csvHelper;
        private readonly IFileProcessor _processFile;
        private readonly ProfileContext _profileContext;
        private readonly IWebHostEnvironment _env;

        public AzureProfileDatabaseSeeder(
            IFileProcessor processFile, ProfileContext profileContext, IWebHostEnvironment env)
        {
            _processFile = processFile;
            _profileContext = profileContext;
            _env = env;
        }

        public async Task SeedAsync()
        {
            await _profileContext.Database.EnsureCreatedAsync();
            if (!_profileContext.Profiles.ToList().Any())
            {
                var profiles = _processFile.Process<Profiles>(_env.ContentRootPath, "Profiles");
                await _profileContext.Profiles.AddRangeAsync(profiles);
                await _profileContext.SaveChangesAsync();
            }
        }
    }    
}
