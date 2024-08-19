using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    /// <summary>
    /// All the URIs pointing to the various images for the card on scryfall.
    /// </summary>
    public class ImageUris
    {
        [JsonPropertyName("small")]
        public string Small { get; set; }
        
        [JsonPropertyName("png")]
        public string Png { get; set; }
        
        [JsonPropertyName("art_crop")]
        public string ArtCrop { get; set; }
        
        [JsonPropertyName("border_crop")]
        public string BorderCrop { get; set; }
    }
}
