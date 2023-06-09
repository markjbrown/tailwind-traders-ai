﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;

namespace Tailwind.Traders.Profile.Api.Tests
{
    public static class TestExtensions
    {
        public static async Task<T> VerifyResponseModelAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(response.IsSuccessStatusCode);
            var model = JsonSerializer.Deserialize<T>(content);
            Assert.IsNotNull(model);
            return model;
        }
    }
}
