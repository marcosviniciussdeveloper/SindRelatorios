using SindRelatorios.Application.DTOs;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.Interfaces;

public interface IExcelExportService
{
    byte[] ExportReport(List<ScheduleRow> scheduleRows);
    
    byte[] ExportBatchReport(List<ScheduleResult> batchResults);
}