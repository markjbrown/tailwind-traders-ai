namespace Tailwind.Traders.Profile.Api
{
    public class AzureCosmosDbConfig
    {
        public static string ConfigKey = "CosmosDb";

        public string Host { get; set; }
        public string Key { get; set; }
        public string Database { get; set; }
    }
}
