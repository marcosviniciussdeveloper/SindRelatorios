using SindRelatorios.Models;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.Interfaces
{
    public interface IExcelExportService
    {
        byte[] ExportReport(List<ScheduleRow> scheduleRows);
    }
}