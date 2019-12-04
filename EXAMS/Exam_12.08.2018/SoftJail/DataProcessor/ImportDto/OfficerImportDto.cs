namespace SoftJail.DataProcessor.ImportDto
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Officer")]
    public class OfficerImportDto
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        [XmlElement(ElementName="Name")]
        public string Name { get; set; }

        [Range(typeof(decimal),"0.0", "79228162514264337593543950335")]
        [XmlElement(ElementName="Money")]
        public decimal Money { get; set; }

        [XmlElement(ElementName="Position")]
        public string Position { get; set; }

        [XmlElement(ElementName="Weapon")]
        public string Weapon { get; set; }

        [XmlElement(ElementName="DepartmentId")]
        public int DepartmentId { get; set; }

        [XmlArray(ElementName="Prisoners")]
        public List<PrisonerOfficerImportDto> Prisoners { get; set; }
    }
}