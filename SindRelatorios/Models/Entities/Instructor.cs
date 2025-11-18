using System.ComponentModel.DataAnnotations;

namespace SindRelatorios.Models.Entities
{
    public class Instructor
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
    }
}