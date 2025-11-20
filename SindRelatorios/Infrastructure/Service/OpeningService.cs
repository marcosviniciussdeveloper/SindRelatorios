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
            Date = DateTime.SpecifyKind(input.Date.Value.Date, DateTimeKind.Utc),
            Region = input.Region,
            Type =  input.Type,
            IsExtra = false,
            Slots = new List<OpeningSlot>() 
        };

        var allInstructors = await _instructorRepository.GetAllAsync();

        foreach (var slotDto in input.Slots)
        {
            var instructor = allInstructors
                .FirstOrDefault(i => i.Name.Equals(slotDto.InstructorName, StringComparison.OrdinalIgnoreCase));

            var slot = new OpeningSlot
            {
                InstructorId = instructor?.Id, 
                Shift = slotDto.Shift,
                Status = SlotStatus.Planejado,
                Observation = ""
            };
            
            calendar.Slots.Add(slot);
        }

      
        await _calendarRepository.AddAsync(calendar);
    }
}