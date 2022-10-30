using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Repositories
{
    public interface IProductItemRepository
    {
        Task<List<ProductItem>> GetAllProductsAsync();
        Task<ProductItem> GetProductById(int productId);
        Task<List<ProductItem>> FindProductsAsync(int[] brand, int[] type);
        Task<List<ProductItem>> FindProductsByTag(string tag);
        Task<List<ProductItem>> FindProductsByIdsAsync(int[] ids);
        Task<List<ProductItem>> RecommendedProductsAsync();
        Task<List<ProductBrand>> GetAllBrandsAsync();
        Task<List<ProductType>> GetAllTypesAsync();
    }
}
