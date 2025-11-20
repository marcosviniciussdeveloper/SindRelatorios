namespace SindRelatorios.Models.Entities;

public class ScheduleRow
{
    public Guid Id { get; set; }
   
    public DateTime Date { get; set; } // Data

    public string Subject { get; set; } = string.Empty; // Disciplina
    public string Shift { get; set; } = string.Empty; // Turno
   
    public string Instructor { get; set; } = string.Empty; // Instrutor
   
    public int Hours { get; set; } // CargaHoraria
}

