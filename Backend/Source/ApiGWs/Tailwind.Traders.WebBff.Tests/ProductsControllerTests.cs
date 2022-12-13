using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tailwind.Traders.WebBff.Controllers;
using Tailwind.Traders.WebBff.Infrastructure;
using Tailwind.Traders.WebBff.Models;

namespace Tailwind.Traders.WebBff.Tests
{
    [TestClass]
    public class ProductsControllerTests
    {
        [TestMethod]
        public async Task TestGetProductsWithNoBrand_ReturnsFilteredByType()
        {
            AppSettings appSettings = new AppSettings { ProductsApiUrl = "http://localhost:5000" };
            var baseUri = appSettings.ProductsApiUrl; 

            var typesUrl = "/type";
            var productTypes = new ProductType[]
            {
                new ProductType() { Id = 23, Code = "home", Name = "White Door"},
                new ProductType() { Id = 27, Code = "gardening", Name = "Metal Watering Can"},
            };
            var productTypesJson = JsonConvert.SerializeObject(productTypes);

            var brandsUrl = "/brand";
            var brands = new ProductBrand[]
            {
                new ProductBrand() { Id = 1, Name = "ElectroDrill"},
                new ProductBrand() { Id = 3, Name = "ProSaws"},
            };
            var brandsJson = JsonConvert.SerializeObject(brands);

            var filterUrl = "/product/filter";
            var filteredProducts = productTypes.Where(c => c.Code == "home");
            var filteredProductsJson = JsonConvert.SerializeObject(filteredProducts);

            var responses = new Dictionary<string, string>() {
                [typesUrl] = productTypesJson,
                [brandsUrl] = brandsJson,
                [filterUrl] = filteredProductsJson,
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpClientFactory.Setup(f => f.CreateClient(HttpClients.ApiGW))
                .Returns(new HttpClient(mockHttpMessageHandler.Object));

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            MockHttpCalls(mockHttpMessageHandler, responses);

            var productsController = new ProductsController(
                new NullLogger<ProductsController>(),
                mockHttpClientFactory.Object,
                Options.Create(appSettings),
                mockHttpContextAccessor.Object);

            await productsController.GetProducts(null, new[] { "home" });
        }

        private static void MockHttpCalls(Mock<HttpMessageHandler> mockHttpMessageHandler, 
            Dictionary<string, string> responses)
        {
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    var correctResponseJson = responses.Single(r => request.RequestUri.PathAndQuery.Contains(r.Key));
                    response.Content = new StringContent(correctResponseJson.Value);
                    return response;
                });
        }
    }
}
