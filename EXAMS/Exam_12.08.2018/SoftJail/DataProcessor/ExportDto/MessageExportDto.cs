namespace SoftJail.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Message")]
    public class MessageExportDto
    {
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }
}