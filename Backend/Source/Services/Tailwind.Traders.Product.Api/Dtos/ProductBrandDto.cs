using Newtonsoft.Json;

namespace Tailwind.Traders.Product.Api.Dtos
{
    public class ProductBrandDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
