using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.DTOs;
using Tailwind.Traders.Profile.Api.Infrastructure;

namespace Tailwind.Traders.Profile.Api.Repositories
{
    public class AzureProfileRepository : IProfileRepository
    {
        private readonly ProfileContext _ctx;
        private readonly AppSettings _settings;

        public AzureProfileRepository(ProfileContext ctx, IOptions<AppSettings> options)
        {
            _ctx = ctx;
            _settings = options.Value;
        }
        public async Task<List<ProfileDto>> GetAll()
        {
            return await _ctx.Profiles.AsQueryable()
                .Select(p => p.ToProfileDto(_settings))
                .ToListAsync();
        }
        public async Task<ProfileDto> GetByEmail(string nameFilter)
        {
            return await _ctx.Profiles.AsQueryable()
                            .Where(p => p.Email == nameFilter)
                            .Select(p => p.ToProfileDto(_settings))
                            .SingleOrDefaultAsync();
        }

        public async Task Add(CreateUser user)
        {
            var newId = await _ctx.Profiles.AsQueryable().CountAsync();
            var profile = user.MapUserProfile(newId);
            await _ctx.Profiles.AddAsync(profile);
            await _ctx.SaveChangesAsync();
        }
    }
}
