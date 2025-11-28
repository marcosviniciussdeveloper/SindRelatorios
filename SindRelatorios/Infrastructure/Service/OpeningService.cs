using SindRelatorios.Application.DTOs;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using SindRelatorios.Models.Entities.Enums;

namespace SindRelatorios.Infrastructure.Services;

public class OpeningService : IOpeningService
{
    private readonly IRepository<OpeningCalendar> _calendarRepository;
    private readonly IRepository<Instructor> _instructorRepository;

    public OpeningService(
        IRepository<OpeningCalendar> calendarRepository, 
        IRepository<Instructor> instructorRepository)
    {
        _calendarRepository = calendarRepository;
        _instructorRepository = instructorRepository;
    }

   public async Task CreateOpeningAsync(CreateOpeningInput input)
    {
        if (input.Date == null) throw new ArgumentException("A data é obrigatória.");

        var calendar = new OpeningCalendar
        {
            Date = DateTime.SpecifyKind(input.Date.Value, DateTimeKind.Utc),
            Region = input.Region,
            Type = input.Type,
            IsExtra = false,
            Slots = new List<OpeningSlot>()
        };

        var allInstructors = await _instructorRepository.GetAllAsync();

        foreach (var slotDto in input.Slots)
        {
            var instructor = allInstructors
                .FirstOrDefault(i => i.Name.Equals(slotDto.InstructorName, StringComparison.OrdinalIgnoreCase));

     
            var shifts = SplitShifts(slotDto.Shift);

            foreach (var singleShift in shifts)
            {
                calendar.Slots.Add(new OpeningSlot
                {
                    InstructorId = instructor?.Id,
                    Shift = singleShift, 
                    Status = SlotStatus.Planejado,
                    Observation = ""
                });
            }
        }

        await _calendarRepository.AddAsync(calendar);
    }

    private List<string> SplitShifts(string comboShift)
    {
        return comboShift switch
        {
            "MANHA_TARDE" => new List<string> { "MANHA", "TARDE" },
            "MANHA_NOITE" => new List<string> { "MANHA", "NOITE" },
            "TARDE_NOITE" => new List<string> { "TARDE", "NOITE" },
            "MANHA_TARDE_NOITE" => new List<string> { "MANHA", "TARDE", "NOITE" },
            "INTEGRAL" => new List<string> { "MANHA", "TARDE", "NOITE" },
            _ => new List<string> { comboShift } // Se for simples (NOITE), retorna ele mesmo
        };
    }
}