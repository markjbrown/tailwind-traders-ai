using System.Threading.Tasks;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public interface ISeedDatabase
    {
        Task SeedAsync();
    }
}
