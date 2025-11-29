using System.ComponentModel.DataAnnotations;

namespace SindRelatorios.Models.Entities;

public class Instructor
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Nome é obrigatório")]
    public string Name { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public string TeleaulaPassword { get; set; } = string.Empty;

    // Delegacia / Região (Ex: Salvador, Lauro de Freitas)
    public string Region { get; set; } = "Salvador";

    public bool AvailableMorning { get; set; } = false;
    public bool AvailableAfternoon { get; set; } = false;
    public bool AvailableNight { get; set; } = false;

    public string Observation { get; set; } = string.Empty;
}