using SindRelatorios.Models;

namespace SindRelatorios.Application
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