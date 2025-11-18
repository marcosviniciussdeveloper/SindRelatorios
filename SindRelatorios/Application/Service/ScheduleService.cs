using SindRelatorios.Models;
using SindRelatorios.Infrastructure;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;


namespace SindRelatorios.Application
{
    public class ScheduleService : IScheduleService
    {
        private readonly IHolidayService _holidayService;
        private readonly ICourseTemplateProvider _templateProvider;

        public ScheduleService(IHolidayService holidayService, ICourseTemplateProvider templateProvider)
        {
            _holidayService = holidayService;
            _templateProvider = templateProvider;
        }

        public async Task<List<ScheduleRow>> GenerateSchedule(
            DateTime startDate, 
            string instructor, 
            string shiftText, 
            int dailyHours,
            CourseType type)
        {
            var template = _templateProvider.GetTemplate(type);

            var holidays = await _holidayService.GetHolidays(startDate.Year);
            if (startDate.Month > 10)
            {
                holidays.UnionWith(await _holidayService.GetHolidays(startDate.Year + 1));
            }

            var scheduleRows = new List<ScheduleRow>();
            var currentDate = startDate.AddDays(-1); 

            while (scheduleRows.Count < template.TotalDays)
            {
                currentDate = currentDate.AddDays(1);

                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                if (template.SkipHolidays && holidays.Contains(currentDate.Date))
                {
                    continue;
                }

               
                int classNumber = scheduleRows.Count + 1;
                
                var newRow = new ScheduleRow
                {
                    Date = currentDate.Date,
                    Shift = shiftText,
                    Subject = template.SubjectTemplate[classNumber], 
                    Instructor = instructor,
                    Hours = dailyHours
                };
                
                scheduleRows.Add(newRow);
            }

            return scheduleRows;
        }

    
    }
}