using Google.Cloud.Firestore;
using Tailwind.Traders.Profile.Api.DTOs;

namespace Tailwind.Traders.Profile.Api.Models
{
    [FirestoreData]
    public class Profiles
    {
        public const string AppName = "Tailwind";

        [FirestoreProperty]
        public string App { get; set; } = AppName;

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Address { get; set; }

        [FirestoreProperty]
        public string PhoneNumber { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string ImageNameSmall { get; set; }

        [FirestoreProperty]
        public string ImageNameMedium { get; set; }

        public ProfileDto ToProfileDto(AppSettings settings) =>
            new ProfileDto()
            {
                Name = this.Name,
                Address = this.Address,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                ImageUrlSmall = $"{settings.ProfilesImageUrl}/{this.ImageNameSmall}",
                ImageUrlMedium = $"{settings.ProfilesImageUrl}/{this.ImageNameMedium}"
            };
    }    
}
