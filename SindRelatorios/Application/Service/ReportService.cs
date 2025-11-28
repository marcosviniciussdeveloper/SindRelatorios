using Microsoft.EntityFrameworkCore;
using SindRelatorios.Application.DTOs;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Infrastructure.Data;
using SindRelatorios.Models.Entities;
using SindRelatorios.Models.Entities.Enums;

namespace SindRelatorios.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly SindDbContext _context;

    public ReportService(SindDbContext context)
    {
        _context = context;
    }

    public async Task<List<InstructorReportDto>> GetReportsByDateRangeAsync(DateTime start, DateTime end)
    {
       
        var startUtc = DateTime.SpecifyKind(start.Date, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(end.Date, DateTimeKind.Utc);

        var calendars = await _context.OpeningCalendars
            .Include(c => c.Slots)
            .ThenInclude(s => s.Instructor)
            .Where(c => c.Date >= startUtc && c.Date <= endUtc)
            .AsNoTracking()
            .ToListAsync();

        var reports = new List<InstructorReportDto>();

        var allSlots = calendars
            .SelectMany(c => c.Slots)
            .Where(s => s.Instructor != null && s.Status == SlotStatus.Aberto)
            .ToList();

        var grouped = allSlots.GroupBy(s => s.Instructor!.Id);

        foreach (var group in grouped)
        {
            var instructorName = group.First().Instructor!.Name;
            
            var classes = group.Select(slot => new ScheduleRow
            {
                Date = calendars.First(c => c.Id == slot.OpeningCalendarId).Date,
                Shift = slot.Shift,
                Subject = "AULA TEÓRICA - LEGISLAÇÃO", // Texto padrão para NF
                Instructor = instructorName,
                Hours = GetHours(slot.Shift)
            }).OrderBy(x => x.Date).ThenBy(x => x.Shift).ToList();

            reports.Add(new InstructorReportDto
            {
                InstructorId = group.Key,
                InstructorName = instructorName,
                Classes = classes,
                TotalHours = classes.Sum(x => x.Hours),
                IsSelected = true 
            });
        }

        return reports.OrderBy(x => x.InstructorName).ToList();
    }

    private int GetHours(string shift) => shift switch
    {
        "MANHA" => 5, "TARDE" => 5, "NOITE" => 5,
        "MANHA_TARDE" => 10, "MANHA_NOITE" => 10, "TARDE_NOITE" => 10,
        "INTEGRAL" => 15, _ => 5
    };

  
}