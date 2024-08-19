using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class Legalities
    {
        [JsonPropertyName("standard")]
        public string Standard { get; set; }

        [JsonPropertyName("future")]
        public string Future { get; set; }

        [JsonPropertyName("historic")]
        public string Historic { get; set; }

        [JsonPropertyName("pioneer")]
        public string Pioneer { get; set; }

        [JsonPropertyName("modern")]
        public string Modern { get; set; }

        [JsonPropertyName("legacy")]
        public string Legacy { get; set; }

        [JsonPropertyName("pauper")]
        public string Pauper { get; set; }

        [JsonPropertyName("vintage")]
        public string Vintage { get; set; }

        [JsonPropertyName("penny")]
        public string Penny { get; set; }

        [JsonPropertyName("commander")]
        public string Commander { get; set; }

        [JsonPropertyName("brawl")]
        public string Brawl { get; set; }

        [JsonPropertyName("duel")]
        public string Duel { get; set; }
        
        [JsonPropertyName("oldschool")]
        public string Oldschool { get; set; }
    }
}
