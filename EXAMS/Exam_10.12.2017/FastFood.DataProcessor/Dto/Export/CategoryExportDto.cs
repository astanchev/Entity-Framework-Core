namespace FastFood.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;

    [XmlType("Category")]
    public class CategoryExportDto
    {
        [XmlElement(ElementName="Name")]
        public string Name { get; set; }

        [XmlElement(ElementName="MostPopularItem")]
        public MostPopularItemExportDto MostPopularItem { get; set; }
    }
}