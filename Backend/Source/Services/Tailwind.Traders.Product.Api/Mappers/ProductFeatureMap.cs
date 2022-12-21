﻿using CsvHelper.Configuration;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Mappers
{
    public sealed class ProductFeatureMap : ClassMap<ProductFeature>
    {
        public ProductFeatureMap()
        {
            Map(p => p.Title);
            Map(p => p.Description);
        }
    }
}
