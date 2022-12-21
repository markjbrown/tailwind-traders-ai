using Microsoft.EntityFrameworkCore;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class ProductContext : DbContext
    {
        public DbSet<ProductItem> ProductItems { get; set; }

        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }
    }
}
