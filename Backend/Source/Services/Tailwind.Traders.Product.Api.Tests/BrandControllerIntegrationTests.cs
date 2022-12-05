using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Dtos;

namespace Tailwind.Traders.Product.Api.Tests
{
    [TestClass]
    public class BrandControllerIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task TestGetAllBrands_AZURE()
        {
            const string CloudPlatform = "AZURE";
            Initialize(CloudPlatform);
            string uri = ApiPath($@"/v1/brand");
            var response = await ApiClient.GetAsync(uri);
            var model = await response.VerifyResponseModelAsync<IEnumerable<ProductBrandDto>>();
            await VerifyJson(await response.Content.ReadAsStringAsync());


            await TimeMethod(CloudPlatform, "GetAllBrands", 50,
                (index) => uri,
                async (response) => await response.VerifyResponseModelAsync<IEnumerable<ProductBrandDto>>());
        }

        [TestMethod]
        public async Task TestGetAllBrands_AWS()
        {
            const string CloudPlatform = "AWS";
            Initialize(CloudPlatform);
            string uri = ApiPath($@"/v1/brand");
            var response = await ApiClient.GetAsync(uri);
            var model = await response.VerifyResponseModelAsync<IEnumerable<ProductBrandDto>>();
            await VerifyJson(await response.Content.ReadAsStringAsync());


            await TimeMethod(CloudPlatform, "GetAllBrands", 50,
                (index) => uri,
                async (response) => await response.VerifyResponseModelAsync<IEnumerable<ProductBrandDto>>());
        }

    }
}
