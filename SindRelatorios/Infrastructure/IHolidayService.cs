namespace SindRelatorios.Services;

public interface IHolidayService
{
  Task<HashSet<DateTime>> GetHolidays(int ano);
}