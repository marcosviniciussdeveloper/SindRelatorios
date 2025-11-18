namespace SindRelatorios.Application.Interfaces;

public interface IHolidayService
{
  Task<HashSet<DateTime>> GetHolidays(int ano);
}