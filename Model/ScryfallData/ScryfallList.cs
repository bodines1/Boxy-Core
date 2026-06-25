using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class ScryfallList<T>
    {
        [JsonRequired]
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("next_page")]
        public required string? NextPage { get; set; }

        [JsonRequired]
        [JsonPropertyName("data")]
        public required T[] Data { get; set; }
    }
}
