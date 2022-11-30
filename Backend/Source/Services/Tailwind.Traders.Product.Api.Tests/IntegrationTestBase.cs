﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
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
    }
}