using ClosedXML.Excel;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Infrastructure.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] ExportReport(List<ScheduleRow> scheduleRows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Relatório de Aulas");

        // 1. CABEÇALHO
        worksheet.Cell(1, 1).Value = "DATA";
        worksheet.Cell(1, 2).Value = "TURNO";
        worksheet.Cell(1, 3).Value = "DISCIPLINA";
        worksheet.Cell(1, 4).Value = "INSTRUTOR";
        worksheet.Cell(1, 5).Value = "CARGA HORARIA";

        var headerRange = worksheet.Range("A1:E1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F81BD"); // Azul bonito
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // 2. DADOS
        int currentRow = 2;
        foreach (var row in scheduleRows)
        {
            worksheet.Cell(currentRow, 1).Value = row.Date;
            worksheet.Cell(currentRow, 1).Style.DateFormat.Format = "dd/MM/yyyy";
            worksheet.Cell(currentRow, 2).Value = row.Shift;
            worksheet.Cell(currentRow, 3).Value = row.Subject;
            worksheet.Cell(currentRow, 4).Value = row.Instructor;
            worksheet.Cell(currentRow, 5).Value = row.Hours;

            // Centralizar data e carga horária
            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
        }

        // 3. LINHA DE TOTAL
        int totalRow = currentRow;
        worksheet.Cell(totalRow, 4).Value = "TOTAL";
        worksheet.Cell(totalRow, 4).Style.Font.Bold = true;
        worksheet.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        worksheet.Cell(totalRow, 5).FormulaA1 = $"=SUM(E2:E{currentRow - 1})";
        worksheet.Cell(totalRow, 5).Style.Font.Bold = true;
        worksheet.Cell(totalRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // 4. FORMATAÇÃO FINAL (BORDAS E AJUSTE)
        // Define o range da tabela inteira (Do A1 até a linha do total, coluna E)
        var tableRange = worksheet.Range(1, 1, totalRow, 5);

        // Aplica as bordas finas em tudo (Igual sua foto)
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}