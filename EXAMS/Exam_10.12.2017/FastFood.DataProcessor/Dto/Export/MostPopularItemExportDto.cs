namespace FastFood.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;

    [XmlType("MostPopularItem")]
    public class MostPopularItemExportDto
    {
        [XmlElement(ElementName="Name")]
        public string Name { get; set; }

        [XmlElement(ElementName="TotalMade")]
        public decimal TotalMade { get; set; }

        [XmlElement(ElementName="TimesSold")]
        public int TimesSold { get; set; }
    }
}