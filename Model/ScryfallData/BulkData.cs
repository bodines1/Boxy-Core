using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class BulkData
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("download_uri")]
        public Uri PermalinkUri { get; set; }
    }
}
