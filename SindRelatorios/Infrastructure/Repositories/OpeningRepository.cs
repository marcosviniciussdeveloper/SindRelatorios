using Microsoft.EntityFrameworkCore;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Infrastructure.Data;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Infrastructure.Repositories;

public class OpeningRepository : Repository<OpeningCalendar>, IOpeningRepository
{
    public OpeningRepository(SindDbContext context) : base(context) { }

    public async Task<List<OpeningCalendar>> GetAllWithDetailsAsync()
    {
        return await _context.OpeningCalendars
            .Include(c => c.Slots)
            .ThenInclude(s => s.Instructor)
            .OrderByDescending(c => c.Date)
            .ToListAsync();
    }

    public async Task<OpeningCalendar?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.OpeningCalendars
            .Include(c => c.Slots)
            .ThenInclude(s => s.Instructor)
            .AsNoTracking() 
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}