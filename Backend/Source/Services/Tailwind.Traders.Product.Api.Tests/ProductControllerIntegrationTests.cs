using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Dtos;

namespace Tailwind.Traders.Product.Api.Tests
{
    [TestClass]
    public class ProductControllerIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task TestGetAllProducts_AZURE()
        {
            Initialize("AZURE");
            var response = await ApiClient.GetAsync(ApiPath($@"/v1/product"));
            await response.VerifyResponseModelAsync<IEnumerable<ProductDto>>();
        }

        [TestMethod]
        public async Task TestGetAllProducts_AWS()
        {
            Initialize("AWS");
            var response = await ApiClient.GetAsync(ApiPath($@"/v1/product"));
            await response.VerifyResponseModelAsync<IEnumerable<ProductDto>>();
        }

        [TestMethod]
        public async Task TestGetAllProducts_GCP()
        {
            Initialize("GCP");
            var response = await ApiClient.GetAsync(ApiPath($@"/v1/product"));
            await response.VerifyResponseModelAsync<IEnumerable<ProductDto>>();
        }

    }
}
