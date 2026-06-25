using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class BulkData
    {
        [JsonRequired]
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonRequired]
        [JsonPropertyName("download_uri")]
        public required Uri PermalinkUri { get; set; }
    }
}
