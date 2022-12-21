using System;

namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductItemSeed : IHaveId
    {
        public ProductItemSeed()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string ImageName { get; set; }

        public int BrandId { get; set; }

        public int TypeId { get; set; }

        public ProductType Type { get; set; }

        public int? TagId { get; set; }

        public ProductItem ToProductItem()
        {
            return new ProductItem
            {
                ProductItemId = Id,
                Name = Name,
                ImageName = ImageName,
                Price = Price,
            };
        }
    }
}
