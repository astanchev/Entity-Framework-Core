namespace CarDealer.DTO
{
    using Newtonsoft.Json;

    public class PartDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public string Price { get; set; }
    }
}