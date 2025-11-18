using SindRelatorios.Models;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleRow>> GenerateSchedule(
            DateTime startDate,
            string instructor,
            string shiftText,
            int dailyHours,
            CourseType type
        );
    }
}