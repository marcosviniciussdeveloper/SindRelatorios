using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.DTOs;

public class ScheduleResult
{
    public List<ScheduleRow> Rows { get; set; } = new();

    // Propriedade calculada ou atribuída para exibir na tela
    public int TotalLoadHours { get; set; }
}