// ... (Usings anteriores)
using ClosedXML.Excel;
using SindRelatorios.Application.DTOs;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities; // Importante

namespace SindRelatorios.Infrastructure.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] ExportReport(List<ScheduleRow> scheduleRows)
    {
        using var workbook = new XLWorkbook();
        CreateSheet(workbook, scheduleRows, "Relatório");
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportBatchReport(List<ScheduleResult> batchResults)
    {
        using var workbook = new XLWorkbook();

        foreach (var result in batchResults)
        {
            if (!result.Rows.Any()) continue;

            // Pega o nome do instrutor para nomear a aba
            var instructorName = result.Rows.First().Instructor;
            
            var safeName = new string(instructorName.Take(30).ToArray())
                .Replace(":", "").Replace("/", "").Replace("\\", "").Replace("?", "").Replace("*", "").Replace("[", "").Replace("]", "");

            // Cria a aba para este instrutor
            CreateSheet(workbook, result.Rows, safeName);
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // Método privado para não duplicar código de formatação
    private void CreateSheet(XLWorkbook workbook, List<ScheduleRow> rows, string sheetName)
    {
        var worksheet = workbook.Worksheets.Add(sheetName);

        // CABEÇALHO
        worksheet.Cell(1, 1).Value = "DATA";
        worksheet.Cell(1, 2).Value = "DIA DA SEMANA";
        worksheet.Cell(1, 3).Value = "TURNO";
        worksheet.Cell(1, 4).Value = "MATÉRIA";
        worksheet.Cell(1, 5).Value = "INSTRUTOR";
        worksheet.Cell(1, 6).Value = "CARGA HORÁRIA";

        var headerRange = worksheet.Range("A1:F1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F81BD");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // DADOS
        int currentRow = 2;
        foreach (var row in rows)
        {
            worksheet.Cell(currentRow, 1).Value = row.Date;
            worksheet.Cell(currentRow, 1).Style.DateFormat.Format = "dd/MM/yyyy";
            worksheet.Cell(currentRow, 2).Value = row.Date.ToString("dddd").ToUpper();
            worksheet.Cell(currentRow, 3).Value = row.Shift;
            worksheet.Cell(currentRow, 4).Value = row.Subject;
            worksheet.Cell(currentRow, 5).Value = row.Instructor;
            worksheet.Cell(currentRow, 6).Value = row.Hours;

            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
        }

        // TOTAL
        int totalRow = currentRow;
        worksheet.Cell(totalRow, 5).Value = "TOTAL GERAL";
        worksheet.Cell(totalRow, 5).Style.Font.Bold = true;
        worksheet.Cell(totalRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        worksheet.Cell(totalRow, 6).FormulaA1 = $"=SUM(F2:F{currentRow - 1})";
        worksheet.Cell(totalRow, 6).Style.Font.Bold = true;
        worksheet.Cell(totalRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // BORDAS
        var tableRange = worksheet.Range(1, 1, totalRow, 6);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        worksheet.Columns().AdjustToContents();
    }
}