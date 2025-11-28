using SindRelatorios.Application.DTOs;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly IHolidayService _holidayService;
    private readonly ICourseTemplateProvider _templateProvider;

    public ScheduleService(IHolidayService holidayService, ICourseTemplateProvider templateProvider)
    {
        _holidayService = holidayService;
        _templateProvider = templateProvider;
    }

    public async Task<ScheduleResult> GeneratePreviewAsync(GeneratorInput input)
    {
        CourseType typeEnum = input.CourseType == "Recycling" ? CourseType.Recycling : CourseType.FirstLicense;
        string shiftDisplayName = FormatShiftName(input.SelectedShift);

        // DEFINIÇÃO DE HORAS DIÁRIAS
        int dailyHours;
        if (typeEnum == CourseType.Recycling)
        {
            dailyHours = 6; // Reciclagem é fixo 6h para fechar 30h em 5 dias
        }
        else
        {
            dailyHours = GetHoursFromShift(input.SelectedShift);
        }

        int totalHoursTarget = typeEnum == CourseType.FirstLicense ? 45 : 30;
        
        // Carrega Feriados
        var holidays = await _holidayService.GetHolidays(input.StartDate.Year);
        if (input.StartDate.Month > 10)
            holidays.UnionWith(await _holidayService.GetHolidays(input.StartDate.Year + 1));

        var scheduleRows = new List<ScheduleRow>();
        var currentDate = input.StartDate.AddDays(-1);
        int currentTotalHours = 0;

        while (currentTotalHours < totalHoursTarget)
        {
            currentDate = currentDate.AddDays(1);

            if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                continue;

            if (holidays.Contains(currentDate.Date))
                continue;

            string subject = GetSubject(scheduleRows.Count, typeEnum);

            scheduleRows.Add(new ScheduleRow
            {
                Date = currentDate.Date,
                Shift = shiftDisplayName,
                Subject = subject,
                Instructor = input.InstructorName,
                Hours = dailyHours
            });

            currentTotalHours += dailyHours;
        }

        return new ScheduleResult
        {
            Rows = scheduleRows,
            TotalLoadHours = scheduleRows.Sum(x => x.Hours)
        };
    }

    private int GetHoursFromShift(string shiftCode)
    {
        return shiftCode switch
        {
            "MANHA" => 5, "TARDE" => 5, "NOITE" => 5,
            "MANHA_TARDE" => 10, "MANHA_NOITE" => 10, "TARDE_NOITE" => 10,
            "INTEGRAL" => 15, _ => 5
        };
    }

    private string FormatShiftName(string shiftCode)
    {
        return shiftCode.Replace("_", "/").Replace("MANHA", "Manhã").Replace("TARDE", "Tarde").Replace("NOITE", "Noite");
    }

    private string GetSubject(int dayIndex, CourseType type)
    {
        if (type == CourseType.Recycling)
        {
            // Padrão Reciclagem (30h - 5 dias)
            return dayIndex switch
            {
                0 => "LEGISLAÇÃO (6 - HA)",
                1 => "LEGISLAÇÃO (6 - HA)",
                2 => "DIREÇÃO DEFENSIVA (6 - HA)",
                3 => "DIREÇÃO DEFENSIVA (2 - HA), PRIMEIROS SOCORROS (4 - HA)",
                4 => "RELACIONAMENTO INTERPESSOAL (6 - HA)",
                _ => "REVISÃO GERAL (6 - HA)"
            };
        }
        else
        {
            // Padrão Primeira Habilitação (45h - 9 dias de 5h)
            if (dayIndex < 4) return "LEGISLAÇÃO (5 - HA)"; // Dias 1, 2, 3, 4
            if (dayIndex == 4) return "MECÂNICA(3 - HA) CIDADANIA(2 - HA)"; // Dia 5
            if (dayIndex < 8) return "DIREÇÃO DEFENSIVA (5 - HA)"; // Dias 6, 7, 8
            return "DIREÇÃO DEFENSIVA (1 - HA), PRIMEIROS SOCORROS (4 - HA)"; // Dia 9
        }
    }
}