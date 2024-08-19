using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class CardFace
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image_uris")]
        public ImageUris ImageUris { get; set; }
        
        [JsonPropertyName("flavor_text")]
        public string FlavorText { get; set; }
        
        [JsonPropertyName("artist")]
        public string Artist { get; set; }
    }
}
