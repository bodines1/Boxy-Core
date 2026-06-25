using System.Text.Json.Serialization;

namespace Boxy_Core.Model.ScryfallData
{
    public class Card
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("oracle_id")]
        public string OracleId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("layout")]
        public string Layout { get; set; }

        [JsonPropertyName("image_uris")]
        public ImageUris ImageUris { get; set; }

        [JsonPropertyName("card_faces")]
        public List<CardFace> CardFaces { get; set; }

        [JsonPropertyName("set")]
        public string Set { get; set; }

        [JsonPropertyName("set_name")]
        public string SetName { get; set; }

        [JsonPropertyName("prints_search_uri")]
        public string PrintsSearchUri { get; set; }

        [JsonPropertyName("collector_number")]
        public string CollectorNumber { get; set; }

        [JsonPropertyName("digital")]
        public bool Digital { get; set; }
        
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
