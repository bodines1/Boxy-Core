using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class RelatedUris
    {
        [JsonPropertyName("gatherer")]
        public string Gatherer { get; set; }
        
        [JsonPropertyName("tcgplayer_decks")]
        public string TcgplayerDecks { get; set; }
        
        [JsonPropertyName("edhrec")]
        public string Edhrec { get; set; }
        
        [JsonPropertyName("mtgtop8")]
        public string Mtgtop8 { get; set; }
    }
}
