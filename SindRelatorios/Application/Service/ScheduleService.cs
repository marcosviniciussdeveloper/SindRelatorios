using SindRelatorios.Application.DTOs;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models; // Enums
using SindRelatorios.Models.Entities; // Entidades

namespace SindRelatorios.Application;

public class ScheduleService : IScheduleService
{
    private readonly IHolidayService _holidayService;
    private readonly ICourseTemplateProvider _templateProvider;

    public ScheduleService(IHolidayService holidayService, ICourseTemplateProvider templateProvider)
    {
        _holidayService = holidayService;
        _templateProvider = templateProvider;
    }

    public async Task<List<ScheduleRow>> GeneratePreviewAsync(GeneratorInput input)
    {
        CourseType typeEnum = input.CourseType == "Recycling" ? CourseType.Recycling : CourseType.FirstLicense;
        int dailyHours = GetHoursFromShift(input.SelectedShift);
        string shiftDisplayName = FormatShiftName(input.SelectedShift);

        var template = _templateProvider.GetTemplate(typeEnum);

        var holidays = await _holidayService.GetHolidays(input.StartDate.Year);
        if (input.StartDate.Month > 10)
        {
            holidays.UnionWith(await _holidayService.GetHolidays(input.StartDate.Year + 1));
        }

        var scheduleRows = new List<ScheduleRow>();
        var currentDate = input.StartDate.AddDays(-1);

        // 4. Loop de Geração
        while (scheduleRows.Count < template.TotalDays)
        {
            currentDate = currentDate.AddDays(1);

            // Pula Fim de Semana
            if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                continue;

            if (template.SkipHolidays && holidays.Contains(currentDate.Date))
                continue;

            int classNumber = scheduleRows.Count + 1;

            string subject = template.SubjectTemplate.ContainsKey(classNumber)
                ? template.SubjectTemplate[classNumber]
                : "AULA PRÁTICA / REVISÃO";

            scheduleRows.Add(new ScheduleRow
            {
                Date = currentDate.Date,
                Shift = shiftDisplayName,
                Subject = subject,
                Instructor = input.InstructorName,
                Hours = dailyHours
            });
        }

        return scheduleRows;
    }

    private int GetHoursFromShift(string shiftCode)
    {
        return shiftCode switch
        {
            "MANHA" => 5,
            "TARDE" => 5,
            "NOITE" => 5,
            "MANHA_TARDE" => 10,
            "MANHA_NOITE" => 10,
            "TARDE_NOITE" => 10,
            "MANHA_TARDE_NOITE" => 15,
            "INTEGRAL" => 15,
            _ => 5
        };
    }

    private string FormatShiftName(string shiftCode)
    {
        return shiftCode.Replace("_", "/").Replace("MANHA", "Manhã").Replace("TARDE", "Tarde").Replace("NOITE", "Noite");
    }
}