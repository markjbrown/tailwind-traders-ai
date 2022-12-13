using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Dtos;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Tests
{
    [TestClass]
    public class TypeControllerIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task TestGetAllTypes_AZURE()
        {
            await TestGetAllTypes("AZURE");
        }

        private async Task TestGetAllTypes(string CloudPlatform)
        {
            Initialize(CloudPlatform);
            string uri = ApiPath($@"/v1/type");
            var response = await ApiClient.GetAsync(uri);
            var model = await response.VerifyResponseModelAsync<IEnumerable<ProductType>>();
            await VerifyJson(await response.Content.ReadAsStringAsync());

            await TimeMethod(CloudPlatform, "GetAllTypes", 50,
                (index) => uri,
                async (response) => await response.VerifyResponseModelAsync<IEnumerable<ProductType>>());
        }

        [TestMethod]
        public async Task TestGetAllTypes_AWS()
        {
            await TestGetAllTypes("AWS");
        }

        [TestMethod]
        public async Task TestGetAllTypes_GCP()
        {
            await TestGetAllTypes("GCP");
        }

    }
}
