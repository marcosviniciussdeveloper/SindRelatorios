using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.Interfaces;

public interface IOpeningRepository : IRepository<OpeningCalendar>
{
    Task<List<OpeningCalendar>> GetAllWithDetailsAsync();
    Task<OpeningCalendar?> GetByIdWithDetailsAsync(Guid id);
}