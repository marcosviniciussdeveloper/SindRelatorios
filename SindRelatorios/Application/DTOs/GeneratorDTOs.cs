using System.ComponentModel.DataAnnotations;

namespace SindRelatorios.Application.DTOs;

public class GeneratorInput
{
    [Required(ErrorMessage = "A data de início é obrigatória.")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Selecione um instrutor.")]
    public string InstructorName { get; set; } = string.Empty;

    [Required]
    public string CourseType { get; set; } = "FirstLicense";

    [Required]
    public string SelectedShift { get; set; } = "NOITE";
}