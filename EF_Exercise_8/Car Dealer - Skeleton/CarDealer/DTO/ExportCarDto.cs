namespace CarDealer.DTO
{
    using Newtonsoft.Json;

    public class ExportCarDto
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Make")]
        public string Make { get; set; }

        [JsonProperty("Model")]
        public string Model { get; set; }

        [JsonProperty("TravelledDistance")]
        public long TravelledDistance { get; set; }
    }
}