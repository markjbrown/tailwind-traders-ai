using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public interface IContextSeed
    {
        Task SeedAsync();
    }
}
