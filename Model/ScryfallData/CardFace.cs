using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class CardFace
    {
        [JsonRequired]
        [JsonPropertyName("image_uris")]
        public required ImageUris ImageUris { get; set; }
    }
}
