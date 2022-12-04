using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using VerifyMSTest;

namespace Tailwind.Traders.Product.Api.Tests
{
    public class IntegrationTestBase : VerifyBase
    {
        protected ApiHost ApiHost = new ApiHost();
        protected HttpClient ApiClient => ApiHost.Client;

        protected string ApiPath(string path) => ApiHost.Url(path);

        public IntegrationTestBase()
        {
        }

        protected void Initialize(string cloudPlatform, Action<IServiceCollection> overrideRegistration = null)
        {
            ApiHost.Initialize(cloudPlatform, overrideRegistration);
        }

        protected async Task TimeMethod(string cloud, string MethodName, int NumberOfIterations, Func<int, string> getProductIndexUri, Func<HttpResponseMessage, Task> verifyResponseForProductById)
        {
            var stopwatch = Stopwatch.StartNew();
            for (int index = 1; index <= NumberOfIterations; index++)
            {
                string newUri = getProductIndexUri(index);
                var newResponse = await ApiClient.GetAsync(newUri);
                await verifyResponseForProductById(newResponse);
            }
            stopwatch.Stop();
            Console.WriteLine($"{cloud} {MethodName} took {stopwatch.Elapsed}");
        }
    }
}