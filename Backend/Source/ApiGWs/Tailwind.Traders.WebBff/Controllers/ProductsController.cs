using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.WebBff.Infrastructure;
using Tailwind.Traders.WebBff.Models;

namespace Tailwind.Traders.WebBff.Controllers
{

    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _settings;
        private readonly ILogger _logger;
        private const string VERSION_API = "v1";
        private const string BearerScheme = "Bearer";

        public ProductsController(
            ILogger<ProductsController> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _settings = options.Value;
            _logger = logger;
        }

        // GET: v1/products/landing
        [HttpGet("landing")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            string username = null;
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authValue))
            {
                var token = authValue.First().Substring(BearerScheme.Length + 1);
                username = RetrieveUserFromToken(token);
            }

            var recommendedProducts = await GetRecommendedProductsAsync(username);
            var aggresponse = new
            {
                PopularProducts = recommendedProducts
            };
            return Ok(aggresponse);
        }

        private async Task<IEnumerable<PopularProduct>> GetRecommendedProductsAsync(string username)
        {
            var mlClient = _httpClientFactory.CreateClient();
            string apiKey = _settings.RecommenderService.ApiKey; // Replace this with the API key for the web service
            mlClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            mlClient.BaseAddress = new Uri(_settings.RecommenderService.Url);

            string jsonBody = $"{{\"data\": {{\"user_id\": \"{username}\", \"product_type\": -1}} }}";
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            // This header will force the request to go to a specific deployment.
            // Remove this line to have the request observe the endpoint traffic rules
            //content.Headers.Add("azureml-model-deployment", "default");
            var response = await mlClient.PostAsync("", content);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            var recommendation = JsonConvert.DeserializeObject<Recommendation>(responseContent);

            var productClient = _httpClientFactory.CreateClient(HttpClients.ApiGW);
            var result = await productClient.GetStringAsync(API.Products.GetByIds(_settings.ProductsApiUrl, VERSION_API, recommendation.Products));
            var products = JsonConvert.DeserializeObject<Product[]>(result);
            return products.Select(p => new PopularProduct
            {
                Id = p.Id,
                Name = p.Name,
                ImageUrl = p.ImageUrl,
                Price = (decimal)p.Price,
            });
        }

        private async Task<IEnumerable<PopularProduct>> GetPopularProductsAsync(string token)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);
            string requestUri = API.PopularProducts.GetProducts(_settings.PopularProductsApiUrl, VERSION_API);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(BearerScheme, token);
            }
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var popularProducts = JsonConvert.DeserializeObject<IEnumerable<PopularProduct>>(result);
            return popularProducts;
        }

        // GET: v1/products/1
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductDetails([FromRoute] int id)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);
            var result = await client.GetStringAsync(API.Products.GetProduct(_settings.ProductsApiUrl, VERSION_API, id));
            var product = JsonConvert.DeserializeObject<Product>(result);

            // We need to call the stock API to retrieve the stock of the product
            var stockUrl = API.Stock.GetStockProduct(_settings.StockApiUrl, VERSION_API, id);
            try
            {
                var stockResponse = await client.GetAsync(stockUrl);
                stockResponse.EnsureSuccessStatusCode();
                var stockJson = await stockResponse.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse(stockJson);
                product.StockUnits = (int)data.productStock;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HttpRequestException calling Stock API using {stockUrl}. Message is {ex.Message}");
                _logger.LogInformation($"Error won't be forwarded to client, instead stock is set to 0.");
                product.StockUnits = 0;
            }

            return Ok(product);
        }

        // GET: v1/products
        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProducts([FromQuery] string[] brand = null, [FromQuery] string[] type = null)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var result = await client.GetStringAsync(API.Products.GetTypes(_settings.ProductsApiUrl, VERSION_API));
            var types = JsonConvert.DeserializeObject<IEnumerable<ProductType>>(result);
            var productsUrl = brand?.Count() > 0 || type?.Count() > 0 ?
                API.Products.GetProductsByFilter(_settings.ProductsApiUrl, VERSION_API, brand, type) :
                API.Products.GetProducts(_settings.ProductsApiUrl, VERSION_API);

            var resultProducts = await client.GetAsync(productsUrl);
            result = await resultProducts.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);

            result = await client.GetStringAsync(API.Products.GetBrands(_settings.ProductsApiUrl, VERSION_API));
            var brands = JsonConvert.DeserializeObject<ProductBrand[]>(result);

            var aggresponse = new
            {
                Products = products,
                Brands = brands,
                Types = types
            };
            return Ok(aggresponse);
        }


        private async Task<ClassificationResult> DoMlNetClassifierAction(IFormFile file)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var fileContent = new StreamContent(file.OpenReadStream())
            {
                Headers =
                {
                    ContentLength = file.Length,
                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                }
            };

            var formDataContent = new MultipartFormDataContent
            {
                { fileContent, "file", file.FileName }
            };

            var response = await client.PostAsync(API.Products.ImageClassifier.PostImage(_settings.ImageClassifierApiUrl, VERSION_API), formDataContent);

            if (response.IsSuccessStatusCode)
            {

                var result = await response.Content.ReadAsStringAsync();

                var scoredProduct = JsonConvert.DeserializeObject<ClassificationResult>(result);

                return scoredProduct;
            }
            else
            {
                return ClassificationResult.InvalidResult(response.StatusCode);
            }
        }

        // POST: v1/products/imageclassifier
        [HttpPost("imageclassifier")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostImage(IFormFile file)
        {
            ClassificationResult result = null;
            if (_settings.UseMlNetClassifier)
            {
                _logger.LogInformation($"Beginning a ML.NET based classification");
                result = await DoMlNetClassifierAction(file);
            }
     
            if (result == null || !result.IsOk)
            {
                var resultCode = (int)HttpStatusCode.NotImplemented;
                if (result != null) resultCode = (int)result.Code;
                _logger.LogInformation($"Classification failed due to HTTP CODE {resultCode}");
                return StatusCode(resultCode, "Request to inner container returned HTTP " + resultCode);
            }

            _logger.LogInformation($"Classification ended up with tag {result.Label} with a prob (0-1) of {result.Probability}");

            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);
            // Need to query products API for tag
            var ptagsResponse = await client.GetAsync(API.Products.GetByTag(_settings.ProductsApiUrl, VERSION_API, result.Label));

            if (ptagsResponse.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Tag {result.Label} do not have any object associated");
                return Ok(Enumerable.Empty<ClassifiedProductItem>());
            }

            ptagsResponse.EnsureSuccessStatusCode();

            var suggestedProductsJson = await ptagsResponse.Content.ReadAsStringAsync();
            var suggestedProducts = JsonConvert.DeserializeObject<IEnumerable<ClassifiedProductItem>>(suggestedProductsJson);

            return Ok(suggestedProducts);
        }

        private string RetrieveUserFromToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var isReadableToken = jwtHandler.CanReadToken(token);
            if (!isReadableToken)
            {
                return null;
            }

            var claims = jwtHandler.ReadJwtToken(token).Claims;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
    }
}
