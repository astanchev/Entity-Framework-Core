namespace FastFood.DataProcessor.Dto.Import
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Models.Enums;

    [XmlType("Order")]
    public class OrderImportDto
    {
        [XmlElement(ElementName="Customer")]
        public string Customer { get; set; }

        [XmlElement(ElementName="Employee")]
        public string Employee { get; set; }

        [XmlElement(ElementName="DateTime")]
        public string DateTime { get; set; }

        [XmlElement(ElementName="Type")]
        public OrderType Type { get; set; }

        [XmlArray(ElementName="Items")]
        public List<ItemOrderImportDto> Items { get; set; }
    }
}