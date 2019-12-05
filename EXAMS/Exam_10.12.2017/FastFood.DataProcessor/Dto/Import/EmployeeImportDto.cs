﻿namespace FastFood.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;

    public class EmployeeImportDto
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }

        [Range(15,80)] 
        public int Age { get; set; }

        [Required]
        [MinLength(3), MaxLength(30)]
        public string Position { get; set; }
    }
}