using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.Login.Api.Models;
using Tailwind.Traders.Profile.Api.Infrastructure;

namespace Tailwind.Traders.Profile.Api.Tests
{
    public class IntegrationTestBase
    {
        protected ApiHost ApiHost = new ApiHost();
        protected HttpClient ApiClient => ApiHost.Client;
        protected string ApiPath(string path) => ApiHost.Url(path);

#if AUTH
        protected AuthHost AuthHost = new AuthHost();
        protected HttpClient AuthClient => AuthHost.Client;
        protected string AuthPath(string path) => AuthHost.Url(path);
#endif

        public IntegrationTestBase()
        {
        }

        protected void Initialize(string cloudPlatform, Action<IServiceCollection> overrideRegistration = null)
        {
#if AUTH
            AuthHost.Initialize(cloudPlatform);
#endif
            ApiHost.Initialize(cloudPlatform, overrideRegistration);
        }

#if AUTH
        protected async Task AuthenticateAsync()
        {
            var tokenRequest = new TokenRequestModel
            {
                Username = "scott@me.com",
                Password = "WhateverIWant",
                GrantType = "password"
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await AuthClient.PostAsync(AuthPath("/v1/oauth2/token"), stringContent);
            var content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var model = JsonConvert.DeserializeObject<TokenResponseModel>(content);
        }
#endif
    }
}