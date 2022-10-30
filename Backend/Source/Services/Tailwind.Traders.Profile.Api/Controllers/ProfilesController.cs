using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.DTOs;
using Tailwind.Traders.Profile.Api.Infrastructure;
using Tailwind.Traders.Profile.Api.Models;
using Tailwind.Traders.Profile.Api.Repositories;

namespace Tailwind.Traders.Profile.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppSettings _settings;
        private readonly IProfileRepository _profileRepository;

        public ProfileController(IProfileRepository profileRepository, IOptions<AppSettings> options)
        {
            _settings = options.Value;
            _profileRepository = profileRepository;
        }


        // GET v1/profile
        [HttpGet]
        [ProducesResponseType(typeof(List<Profiles>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetAllProfiles()
        {
            List<ProfileDto> result = await _profileRepository.GetAll();

            if (!result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }

        // GET v1/profile/me
        [HttpGet("me")]
        [ProducesResponseType(typeof(List<Profiles>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetProfile()
        {
            var nameFilter = User.Identity.Name ?? string.Empty;
            ProfileDto result = await _profileRepository.GetByEmail(nameFilter);

            if (result == null)
            {
                var defaultUser = GetDefaultUserProfile(nameFilter);
                return Ok(defaultUser);
            }

            return Ok(result);
        }

        // POST v1/profile
        [HttpPost]
        [ProducesResponseType(typeof(List<Profiles>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // TODO: Auto generated value for int not implemented with CosmosDb EF yet.
            await _profileRepository.Add(user);

            return Ok();
        }

        private ProfileDto GetDefaultUserProfile(string nameFilter)
        {
            return new ProfileDto
            {
                Id = 0,
                Email = nameFilter,
                Address = "7711 W. Pawnee Ave. Beachwood, OH 44122",
                Name = nameFilter,
                PhoneNumber = "+1-202-555-0155",
                ImageUrlMedium = "defaultImage-m.jpg",
                ImageUrlSmall = "defaultImage-s.jpg"
            };
        }
    }
}
