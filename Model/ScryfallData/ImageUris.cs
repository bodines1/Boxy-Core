using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    /// <summary>
    /// All the URIs pointing to the various images for the card on scryfall.
    /// </summary>
    public class ImageUris
    {
        [JsonRequired]
        [JsonPropertyName("small")]
        public required string Small { get; set; }

        [JsonRequired]
        [JsonPropertyName("border_crop")]
        public required string BorderCrop { get; set; }
    }
}
