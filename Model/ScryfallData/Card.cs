using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class Card
    {
        [JsonRequired]
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonRequired]
        [JsonPropertyName("oracle_id")]
        public required string OracleId { get; set; }

        [JsonRequired]
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonRequired]
        [JsonPropertyName("layout")]
        public required string Layout { get; set; }

        [JsonRequired]
        [JsonPropertyName("image_uris")]
        public required ImageUris ImageUris { get; set; }

        [JsonRequired]
        [JsonPropertyName("card_faces")]
        public required List<CardFace> CardFaces { get; set; }

        [JsonRequired]
        [JsonPropertyName("prints_search_uri")]
        public required string PrintsSearchUri { get; set; }

        [JsonRequired]
        [JsonPropertyName("collector_number")]
        public required string CollectorNumber { get; set; }

        [JsonRequired]
        [JsonPropertyName("digital")]
        public bool Digital { get; set; }

        [JsonRequired]
        [JsonPropertyName("prices")]
        public required Prices Prices { get; set; }
        
        public bool IsDoubleFaced
        {
            get
            {
                return ImageUris == null && CardFaces != null && CardFaces.Count == 2;
            }
        }

        public bool IsToken
        {
            get
            {
                return Layout == "token" || Layout == "double_faced_token";
            }
        }
    }
}
