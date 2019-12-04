﻿namespace SoftJail.DataProcessor.ExportDto
{
    using Newtonsoft.Json;

    public class OfficerExportDto
    {
        [JsonProperty("OfficerName")]
        public string OfficerName { get; set; }

        [JsonProperty("Department")]
        public string Department { get; set; }
    }
}