namespace CarDealer.DTO
{
    using Newtonsoft.Json;

    public class SupplierDto
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("PartsCount")]
        public long PartsCount { get; set; }
    }
}