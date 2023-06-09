﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Dtos;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Tests
{
    [TestClass]
    public class BrandControllerIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task TestGetAllBrands_AZURE()
        {
            await TestGetAllBrands("AZURE");
        }

        private async Task TestGetAllBrands(string CloudPlatform)
        {
            Initialize(CloudPlatform);
            string uri = ApiPath($@"/v1/brand");
            var response = await ApiClient.GetAsync(uri);
            var model = await response.VerifyResponseModelAsync<IEnumerable<ProductBrand>>();
            await VerifyJson(await response.Content.ReadAsStringAsync());


            await TimeMethod(CloudPlatform, "GetAllBrands", 50,
                (index) => uri,
                async (response) => await response.VerifyResponseModelAsync<IEnumerable<ProductBrand>>());
        }

        [TestMethod]
        public async Task TestGetAllBrands_AWS()
        {
            await TestGetAllBrands("AWS");
        }

        [TestMethod]
        public async Task TestGetAllBrands_GCP()
        {
            await TestGetAllBrands("GCP");
        }

    }
}
