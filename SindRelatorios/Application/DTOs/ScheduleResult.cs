using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.DTOs;

public class ScheduleResult
{
    public List<ScheduleRow> Rows { get; set; } = new();

    public int TotalLoadHours { get; set; }
}