using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class CardFace
    {
        [JsonPropertyName("image_uris")]
        public ImageUris ImageUris { get; set; }
    }
}
