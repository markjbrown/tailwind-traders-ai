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
            const string CloudPlatform = "AZURE";
            Initialize(CloudPlatform);
            string uri = ApiPath($@"/v1/product");
            var response = await ApiClient.GetAsync(uri);
            var model = await response.VerifyResponseModelAsync<IEnumerable<ProductDto>>();
            await VerifyJson(await response.Content.ReadAsStringAsync());

            await TimeMethod(CloudPlatform, "GetAllProducts", 50,
                (index) => uri,
                async (response) => await response.VerifyResponseModelAsync<IEnumerable<ProductDto>>());
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

        [TestMethod]
        public async Task TestGetProductById_AZURE()
        {
            const string CloudPlatform = "AZURE";
            Initialize(CloudPlatform);
            string uri = ApiPath($@"/v1/product/52");
            var response = await ApiClient.GetAsync(uri);
            var model = await response.VerifyResponseModelAsync<ProductDto>();
            await VerifyJson(await response.Content.ReadAsStringAsync());

            await TimeMethod(CloudPlatform, "GetProductById", 70, 
                (index) => ApiPath($@"/v1/product/{index}"),
                async (response) => await response.VerifyResponseModelAsync<ProductDto>());
        }

    }
}
