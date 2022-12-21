namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductTypeSeed
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public ProductType ToProductType()
        {
            return new ProductType { Code = Code, Name = Name };
        }
    }
}
