using System.Collections.Generic;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.DTOs;

namespace Tailwind.Traders.Profile.Api.Repositories
{
    public interface IProfileRepository
    {
        Task Add(CreateUser user);
        Task<IEnumerable<ProfileDto>> GetAll();
        Task<ProfileDto> GetByEmail(string nameFilter);
    }
}