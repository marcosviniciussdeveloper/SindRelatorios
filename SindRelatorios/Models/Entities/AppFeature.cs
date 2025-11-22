using System.ComponentModel.DataAnnotations;
using SindRelatorios.Models.Entities.Enums;

namespace SindRelatorios.Models.Entities;

public class AppFeature
{
    [Key]
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "O titulo é obrigatório.")]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public FeatureStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
}