using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class ScryfallList<T>
    {
        [JsonPropertyName("total_cards")]
        public long TotalCards { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("next_page")]
        public string NextPage { get; set; }

        [JsonPropertyName("data")]
        public T[] Data { get; set; }
    }
}
