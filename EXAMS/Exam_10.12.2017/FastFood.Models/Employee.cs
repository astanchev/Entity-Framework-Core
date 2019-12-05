namespace FastFood.Models
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Employee
	{
        public int Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }

        [Range(15,80)] 
        public int Age { get; set; }
        public int PositionId { get; set; }

        [Required]
        public Position Position { get; set; }
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();

	}
}