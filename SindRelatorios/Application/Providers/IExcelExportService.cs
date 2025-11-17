using SindRelatorios.Models;

namespace SindRelatorios.Application
{
    public interface IExcelExportService
    {
        byte[] ExportReport(List<ScheduleRow> scheduleRows);
    }
}