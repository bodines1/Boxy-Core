using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class Prices
    {
        [JsonRequired]
        [JsonPropertyName("usd")]
        public required string Usd { get; set; }
    }
}
