using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class Prices
    {
        [JsonPropertyName("usd")]
        public string Usd { get; set; }
        
        [JsonPropertyName("eur")]
        public string Eur { get; set; }
        
        [JsonPropertyName("tix")]
        public string Tix { get; set; }
    }
}
