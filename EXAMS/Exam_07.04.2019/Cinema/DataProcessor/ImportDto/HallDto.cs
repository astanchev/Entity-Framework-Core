namespace Cinema.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;

    public class HallDto
    {
        [Required]
        [MaxLength(20), MinLength(3)]
        public string Name { get; set; }
        public bool Is4Dx { get; set; }
        public bool Is3D { get; set; }
        public int Seats { get; set; }
    }
}