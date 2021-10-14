
namespace Company.Function
{
    using Newtonsoft.Json;

public class RatingItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId {get;set;}

        [JsonProperty("productId")]
        public string ProductId {get;set;}
        
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("locationName")]
        public string LocationName { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("userNotes")]
        public string UserNotes { get; set; }

    }
}