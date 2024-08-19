using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class BulkData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("download_uri")]
        public Uri PermalinkUri { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonPropertyName("compressed_size")]
        public int CompressedSize { get; set; }
        
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }
        
        [JsonPropertyName("content_encoding")]
        public string ContentEncoding { get; set; }
    }
}
