﻿namespace CarDealer.DTO
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ImportCarDto
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("travelledDistance")]
        public long TravelledDistance { get; set; }

        [JsonProperty("partsId")]
        public List<int> PartsId { get; set; }
    }
}