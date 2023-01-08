using System.Threading.Tasks;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public interface ISeedDatabase
    {
        Task ResetAsync();
        Task SeedAsync();
    }
}
