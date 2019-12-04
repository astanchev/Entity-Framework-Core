namespace SoftJail.DataProcessor.ImportDto
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class DepartmentImportDto
    {
        [Required]
        [MinLength(3), MaxLength(25)]
        public string Name { get; set; }

        public List<CellImportDto> Cells { get; set; }
    }
}