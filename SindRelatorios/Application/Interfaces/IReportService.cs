using SindRelatorios.Application.DTOs;

namespace SindRelatorios.Application.Interfaces;

public interface IReportService
{
    Task<List<InstructorReportDto>> GetReportsByDateRangeAsync(DateTime start, DateTime end);


}