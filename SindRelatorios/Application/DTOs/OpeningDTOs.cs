using System.ComponentModel.DataAnnotations;
using SindRelatorios.Models.Entities.Enums;

namespace SindRelatorios.Application.DTOs;

public class CreateOpeningInput
{
    [Required(ErrorMessage = "A data é obrigatória.")]
    public DateTime? Date { get; set; } = DateTime.Today;

    [Required]
    public RegionType Region { get; set; } = RegionType.Salvador;
    
    public OpeningType Type { get; set; } = OpeningType.Standard; 

    public List<CreateSlotInput> Slots { get; set; } = new();
}

public class CreateSlotInput
{
    [Required(ErrorMessage = "Selecione um instrutor.")]
    public string InstructorName { get; set; } = string.Empty;
    public Guid? InstructorId { get; set; } 

    public string Shift { get; set; } = "NOITE"; 
}