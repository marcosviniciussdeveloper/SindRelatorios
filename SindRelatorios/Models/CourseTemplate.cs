namespace SindRelatorios.Models;

public class CourseTemplate
{
    public int TotalDays { get; set; } // TotalDias
    public bool SkipHolidays { get; set; } // PularFeriados
    public Dictionary<int, string> SubjectTemplate { get; set; } = new(); // GabaritoDisciplinas
}