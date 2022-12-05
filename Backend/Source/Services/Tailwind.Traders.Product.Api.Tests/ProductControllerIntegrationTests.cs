using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Controllers;
using Tailwind.Traders.Product.Api.Dtos;

namespace Tailwind.Traders.Product.Api.Tests
{
    [TestClass]
    public class ProductControllerIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task TestGetAllProducts_AZURE()
        {
            await TestGetAllProducts("AZURE");
        }

        private async Task TestGetAllProducts(string cloudPlatform)
        {
            Initialize(cloudPlatform);
            string uri = ApiPath($@"/v1/product");
            var response = await ApiClient.GetAsync(uri);
            var model = await response.VerifyResponseModelAsync<IEnumerable<ProductDto>>();
            await VerifyJson(await response.Content.ReadAsStringAsync());

            await TimeMethod(cloudPlatform, "GetAllProducts", 50,
                (index) => uri,
                async (response) => await response.VerifyResponseModelAsync<IEnumerable<ProductDto>>());
        }

        [TestMethod]
        public async Task TestGetAllProducts_AWS()
        {
            await TestGetAllProducts("AWS");
        }

        [TestMethod]
        public async Task TestGetAllProducts_GCP()
        {
            Initialize("GCP");
            var response = await ApiClient.GetAsync(ApiPath($@"/v1/product"));
            await response.VerifyResponseModelAsync<IEnumerable<ProductDto>>();
        }

        [TestMethod]
        public async Task TestGetProductById_AZURE()
        {
            await TestGetProductById("AZURE", 52);
        }

        [TestMethod]
        public async Task TestGetProductById_1_AZURE()
        {
            await TestGetProductById("AZURE", 1);
        }

        private async Task TestGetProductById(string cloudPlatform, int productId)
        {
            Initialize(cloudPlatform);

            string uri = ApiPath($@"/v1/product/{productId}");
            var response = await ApiClient.GetAsync(uri);
            var model = await response.VerifyResponseModelAsync<ProductDto>();
            await VerifyJson(await response.Content.ReadAsStringAsync());

            await TimeMethod(cloudPlatform, "GetProductById", 70,
                (index) => ApiPath($@"/v1/product/{index}"),
                async (response) => await response.VerifyResponseModelAsync<ProductDto>());
        }

        [TestMethod]
        public async Task TestGetProductById_AWS()
        {
            await TestGetProductById("AWS", 52);
        }

        [TestMethod]
        public async Task TestGetProductById_1_AWS()
        {
            await TestGetProductById("AWS", 1);
        }
    }
}
