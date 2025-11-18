using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SindRelatorios.Application;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models;
using SindRelatorios.Models.Entities;
// CRUCIAL: Cria um apelido para garantir que estamos usando a classe do Banco, não a Página
using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages
{
    public partial class Gerador
    {
        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        private IExcelExportService ExcelService { get; set; } = default!;

        // Use o apelido InstructorEntity
        [Inject]
        private IRepository<InstructorEntity> InstructorRepository { get; set; } = default!;

        private List<ScheduleRow>? generatedClasses;
        private ReportGeneratorInputModel input = new();
        private bool isLoading = false;

        // Use o apelido InstructorEntity
        private List<InstructorEntity> availableInstructors = new();

        protected override async Task OnInitializedAsync()
        {
            var list = await InstructorRepository.GetAllAsync();
            // Agora o 'i.Name' vai funcionar porque ele sabe que é a entidade do banco
            availableInstructors = list.OrderBy(i => i.Name).ToList();

            if (availableInstructors.Any())
            {
                input.DefaultInstructor = availableInstructors.First().Name;
            }
        }

        private class ReportGeneratorInputModel
        {
            public DateTime StartDate { get; set; } = DateTime.Today;
            public string DefaultInstructor { get; set; } = string.Empty;
            public CourseType Type { get; set; }
            public string SelectedShift { get; set; } = "NOITE,5";
        }

        private async Task HandleGenerateSchedule()
        {
            isLoading = true;
            generatedClasses = null;

            int dailyHours = 5;
            string shiftText = "NOITE";

            if (input.Type == CourseType.FirstLicense)
            {
                var parts = input.SelectedShift.Split(',');
                shiftText = parts[0];
                dailyHours = int.Parse(parts[1]);
            }
            else if (input.Type == CourseType.Recycling)
            {
                shiftText = "NOITE";
                dailyHours = 6;
            }

            generatedClasses = await ScheduleService.GenerateSchedule(
                input.StartDate,
                input.DefaultInstructor,
                shiftText,
                dailyHours,
                input.Type
            );

            isLoading = false;
        }

        private void OnCourseTypeChange()
        {
            generatedClasses = null;
        }

        private async Task ExportToExcel()
        {
            if (generatedClasses == null || !generatedClasses.Any()) return;

            var fileBytes = ExcelService.ExportReport(generatedClasses);

            var fileName = $"Relatorio_{input.DefaultInstructor}_{DateTime.Today:yyyy-MM-dd}.xlsx";

            await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileBytes);
        }
    }
}