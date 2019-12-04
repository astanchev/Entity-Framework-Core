namespace SoftJail.DataProcessor.ExportDto
{
    using Newtonsoft.Json;

    using System.Collections.Generic;

    public class PrisonerExportDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("CellNumber")]
        public int? CellNumber { get; set; }

        [JsonProperty("Officers")]
        public List<OfficerExportDto> Officers { get; set; }

        [JsonProperty("TotalOfficerSalary")]
        public decimal TotalOfficerSalary { get; set; }
    }
}