using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public interface IContextSeed<TContext>
        where TContext : DbContext
    {
        Task SeedAsync(TContext context, IWebHostEnvironment env);
    }
}
