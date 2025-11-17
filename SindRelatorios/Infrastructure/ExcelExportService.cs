using ClosedXML.Excel;
using SindRelatorios.Application;
using SindRelatorios.Models;

namespace SindRelatorios.Infrastructure
{
    public class ExcelExportService : IExcelExportService
    {
        public byte[] ExportReport(List<ScheduleRow> scheduleRows)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Relatório de Aulas"); // Nome da aba em Português

            // Cabeçalho (em Português, pois é para o usuário final)
            worksheet.Cell(1, 1).Value = "DATA";
            worksheet.Cell(1, 2).Value = "TURNO";
            worksheet.Cell(1, 3).Value = "DISCIPLINA";
            worksheet.Cell(1, 4).Value = "INSTRUTOR";
            worksheet.Cell(1, 5).Value = "CARGA HORARIA";
            var headerRange = worksheet.Range("A1:E1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F81BD");
            headerRange.Style.Font.FontColor = XLColor.White;

            int currentRow = 2;
            foreach (var row in scheduleRows)
            {
                worksheet.Cell(currentRow, 1).Value = row.Date;
                worksheet.Cell(currentRow, 1).Style.DateFormat.Format = "dd/MM/yyyy";
                worksheet.Cell(currentRow, 2).Value = row.Shift;
                worksheet.Cell(currentRow, 3).Value = row.Subject;
                worksheet.Cell(currentRow, 4).Value = row.Instructor;
                worksheet.Cell(currentRow, 5).Value = row.Hours;
                currentRow++;
            }

            // Totalizador
            int totalRow = currentRow;
            worksheet.Cell(totalRow, 4).Value = "TOTAL"; // Em Português
            worksheet.Cell(totalRow, 4).Style.Font.Bold = true;
            worksheet.Cell(totalRow, 5).FormulaA1 = $"=SUM(E2:E{currentRow - 1})";
            worksheet.Cell(totalRow, 5).Style.Font.Bold = true;

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}