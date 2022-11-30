using Microsoft.EntityFrameworkCore;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public class ProfileContext : DbContext
    {
        public DbSet<Models.Profile> Profiles { get; set; }

        public ProfileContext(DbContextOptions<ProfileContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Profile>() 
                .HasPartitionKey(c => c.App)
                .HasKey(c => c.Email);
        }
    }
}
