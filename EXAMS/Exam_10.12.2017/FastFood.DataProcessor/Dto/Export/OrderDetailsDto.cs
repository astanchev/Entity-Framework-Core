namespace FastFood.DataProcessor.Dto.Export
{
    using System.Collections.Generic;
    using Models;
    using Newtonsoft.Json;

    public class OrderDetailsDto
    {
        [JsonProperty("Customer")]
        public string Customer { get; set; }

        [JsonProperty("Items")]
        public ICollection<ItemExportDto> Items { get; set; }

        [JsonProperty("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}