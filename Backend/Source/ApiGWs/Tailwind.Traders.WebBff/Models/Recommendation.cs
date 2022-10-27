using Newtonsoft.Json;

namespace Tailwind.Traders.WebBff.Models
{
    public class Recommendation
    {
        [JsonProperty("recommended_products")]
        public int[] Products { get; set; }
    }
}
