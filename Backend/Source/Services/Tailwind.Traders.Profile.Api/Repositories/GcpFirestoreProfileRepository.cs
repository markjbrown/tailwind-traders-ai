using Google.Cloud.Firestore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.DTOs;
using Tailwind.Traders.Profile.Api.Infrastructure;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Repositories
{
    public class GcpFirestoreProfileRepository : IProfileRepository
    {
        private readonly CollectionReference _profilesCollection;
        private readonly AppSettings _settings;

        public GcpFirestoreProfileRepository(IOptions<AppSettings> options)
        {
            _settings = options.Value;
            // DB Authentication with serviceJson and initialization
            FirestoreDb db = GcpHelper.CreateDb(_settings.Firestore);

            // getting collections
            _profilesCollection = db.Collection("Profiles");
        }

        public async Task Add(CreateUser user)
        {
            // Alternatively, collection.Document("los-angeles").Create(city);
            DocumentReference document = await _profilesCollection.AddAsync(user.MapUserProfile());
        }

        public async Task<IEnumerable<ProfileDto>> GetAll()
        {
            var profilesSnapshot = await _profilesCollection.GetSnapshotAsync();
            var profiles = profilesSnapshot.Documents.Select(x => x.ConvertTo<Profiles>()).ToList();
            return profiles.Select(p => p.ToProfileDto(_settings));
        }

        public async Task<ProfileDto> GetByEmail(string nameFilter)
        {
            var profileSnapshot = await _profilesCollection.WhereEqualTo("Email", nameFilter).GetSnapshotAsync();
            var profile = profileSnapshot.Documents
                .Select(x => x.ConvertTo<Profiles>())
                .SingleOrDefault()
                .ToProfileDto(_settings);
            return profile;
        }
    }
}
