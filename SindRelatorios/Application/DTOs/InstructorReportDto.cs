using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.DTOs;

//dto para relatorio de instrutor em lotes

public class InstructorReportDto
{
    
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int TotalHours { get; set; }
    public List<ScheduleRow> Classes { get; set; } = new();
    public bool IsSelected { get; set; } = true; 
    
}