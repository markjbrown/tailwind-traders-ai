namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductFeatureSeed
    {
        public int Id { get; set; }

        public int ProductItemId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ProductFeature ToProductFeature()
        {
            return new ProductFeature { Title = Title, Description = Description };
        }
    }
}
