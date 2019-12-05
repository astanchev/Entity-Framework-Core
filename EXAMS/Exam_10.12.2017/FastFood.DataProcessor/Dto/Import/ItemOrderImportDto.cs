namespace FastFood.DataProcessor.Dto.Import
{
    using System.Xml.Serialization;

    [XmlType("Item")]
    public class ItemOrderImportDto
    {
        [XmlElement(ElementName="Name")]
        public string Name { get; set; }

        [XmlElement(ElementName="Quantity")]
        public int Quantity { get; set; }
    }
}