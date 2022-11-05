using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.DTOs;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Tests
{
    [TestClass]
    public class ProfileControllerIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task TestGetAllProfiles_AZURE()
        {
            Initialize("AZURE");
            var response = await ApiClient.GetAsync(ApiPath($@"/v1/profile"));
            await response.VerifyResponseModelAsync<IEnumerable<Profiles>>();
        }

        [TestMethod]
        public async Task TestAddProfile_AWS()
        {
            Initialize("AWS");
            var createUser = new CreateUser
            {
                Name = "Scott Reed",
                Email = "scott@me.com",
                Address = "123 Fake St. San Diego, CA, 92101",
                PhoneNumber = "619-555-1212",
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(createUser), Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await ApiClient.PostAsync(ApiPath($@"/v1/profile"), stringContent);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task TestGetAllProfiles_AWS()
        {
            Initialize("AWS");
            var response = await ApiClient.GetAsync(ApiPath($@"/v1/profile"));
            await response.VerifyResponseModelAsync<IEnumerable<Profiles>>();
        }

        [TestMethod]
        public async Task TestGetAllProfiles_GCP()
        {
            Initialize("GCP");
            var response = await ApiClient.GetAsync(ApiPath($@"/v1/profile"));
            await response.VerifyResponseModelAsync<IEnumerable<Profiles>>();
        }

    }
}
