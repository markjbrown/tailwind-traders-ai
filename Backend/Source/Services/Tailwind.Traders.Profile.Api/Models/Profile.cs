using Google.Cloud.Firestore;
using Tailwind.Traders.Profile.Api.DTOs;

namespace Tailwind.Traders.Profile.Api.Models
{
    [FirestoreData]
    public class Profile
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
                Name = Name,
                Address = Address,
                Email = Email,
                PhoneNumber = PhoneNumber,
                ImageUrlSmall = $"{settings.ProfilesImageUrl}/{ImageNameSmall}",
                ImageUrlMedium = $"{settings.ProfilesImageUrl}/{ImageNameMedium}"
            };
    }    
}
