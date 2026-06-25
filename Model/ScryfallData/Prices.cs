using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class Prices
    {
        [JsonPropertyName("usd")]
        public string Usd { get; set; }
    }
}
