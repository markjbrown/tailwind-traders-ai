using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public class AzureProfileDatabaseSeeder : ISeedDatabase
    {
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

        public async Task ResetAsync()
        {
            await _profileContext.Database.EnsureDeletedAsync();
        }

        public async Task SeedAsync()
        {
            await _profileContext.Database.EnsureCreatedAsync();
            if (!_profileContext.Profiles.ToList().Any())
            {
                var profiles = _processFile.Process<Models.Profile>(_env.ContentRootPath, "Profiles");
                await _profileContext.Profiles.AddRangeAsync(profiles);
                await _profileContext.SaveChangesAsync();
            }
        }
    }    
}
