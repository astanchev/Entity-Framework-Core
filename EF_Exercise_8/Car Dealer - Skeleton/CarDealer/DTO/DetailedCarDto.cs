namespace CarDealer.DTO
{
    using System.Collections.Generic;
    using Models;
    using Newtonsoft.Json;

    public class DetailedCarDto
    {
        [JsonProperty("car")]
        public CarDto Car { get; set; }

        [JsonProperty("parts")]
        public List<PartDto> Parts { get; set; }
    }
}