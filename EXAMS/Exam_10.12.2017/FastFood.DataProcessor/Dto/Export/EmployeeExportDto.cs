namespace FastFood.DataProcessor.Dto.Export
{
    using System.Collections.Generic;
    using Models;
    using Newtonsoft.Json;

    public class EmployeeExportDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Orders")]
        public List<OrderDetailsDto> Orders { get; set; }

        [JsonProperty("TotalMade")]
        public decimal TotalMade { get; set; }
    }
}