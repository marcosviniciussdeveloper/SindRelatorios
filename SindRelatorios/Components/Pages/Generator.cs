using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SindRelatorios.Application.DTOs;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using SindRelatorios.Infrastructure.Services;
using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages;

public partial class Generator
{
    [Inject] private IRepository<InstructorEntity> InstructorRepository { get; set; } = default!;
    [Inject] private IScheduleService ScheduleService { get; set; } = default!;
    [Inject] private IExcelExportService ExcelService { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    [SupplyParameterFromForm]
    public GeneratorInput InputData { get; set; } = new();
    
    public DateTime BatchStartDate { get; set; } = DateTime.Today;
    public string BatchCourseType { get; set; } = "FirstLicense";
    public string BatchShift { get; set; } = "NOITE";
    
    protected List<InstructorEntity> InstructorsList { get; set; } = new();
    protected ScheduleResult? ScheduleResult { get; set; }
    
    protected List<InstructorSelection> BatchSelectionList { get; set; } = new();
    protected bool BatchAllSelected { get; set; } = false;
    protected List<ScheduleResult> BatchResults { get; set; } = new();

    protected string ActiveTab { get; set; } = "Individual";
    protected ScheduleRow? _editingRow = null;
    protected string? PrintTargetName { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        var data = await InstructorRepository.GetAllAsync();
        InstructorsList = data.OrderBy(x => x.Name).ToList();

        BatchSelectionList = InstructorsList.Select(x => new InstructorSelection { 
            Id = x.Id, Name = x.Name, IsSelected = false 
        }).ToList();
    }

    protected async Task HandleGenerate()
    {
        ScheduleResult = await ScheduleService.GeneratePreviewAsync(InputData);
        _editingRow = null;
    }

    protected async Task HandleBatchGenerate()
    {
        BatchResults.Clear();
        _editingRow = null;

        var selectedInstructors = BatchSelectionList.Where(x => x.IsSelected).ToList();
        if (!selectedInstructors.Any()) return;

        foreach (var instr in selectedInstructors)
        {
            var input = new GeneratorInput
            {
                StartDate = BatchStartDate,
                InstructorName = instr.Name,
                CourseType = BatchCourseType,
                SelectedShift = BatchShift
            };

            var result = await ScheduleService.GeneratePreviewAsync(input);
            if (result.Rows.Any()) BatchResults.Add(result);
        }
    }

    protected void EnableEdit(ScheduleRow row) => _editingRow = row;
    
    // --- CORREÇÃO: Recalcula o total ao salvar ---
    protected void SaveEdit(ScheduleResult parentResult)
    {
        _editingRow = null;
        // Recalcula a soma na hora
        parentResult.TotalLoadHours = parentResult.Rows.Sum(x => x.Hours);
    }

    // --- CORREÇÃO: Recalcula o total ao deletar ---
    protected void DeleteRow(ScheduleResult parentResult, ScheduleRow row)
    {
        parentResult.Rows.Remove(row);
        parentResult.TotalLoadHours = parentResult.Rows.Sum(x => x.Hours);
    }

    protected void ToggleBatchAll(ChangeEventArgs e)
    {
        BatchAllSelected = (bool)e.Value!;
        foreach(var i in BatchSelectionList) i.IsSelected = BatchAllSelected;
    }

    protected async Task HandleExportExcel(ScheduleResult result, string instructorName)
    {
        if (result == null || !result.Rows.Any()) return;
        
        var excelBytes = ExcelService.ExportReport(result.Rows);
        var fileName = $"Relatorio_{instructorName}_{DateTime.Now:ddMM}.xlsx";
        
        using var stream = new MemoryStream(excelBytes);
        using var streamRef = new DotNetStreamReference(stream);
        await JS.InvokeVoidAsync("downloadFileFromStream", fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", streamRef);
    }

    protected async Task HandleBatchExportExcel()
    {
        if (BatchResults == null || !BatchResults.Any()) return;

        var excelBytes = ExcelService.ExportBatchReport(BatchResults);
        var fileName = $"Relatorio_Lote_{DateTime.Now:ddMMyy}.xlsx";
        
        using var stream = new MemoryStream(excelBytes);
        using var streamRef = new DotNetStreamReference(stream);
        await JS.InvokeVoidAsync("downloadFileFromStream", fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", streamRef);
    }

    protected async Task PrintIndividual(string instructorName)
    {
        PrintTargetName = instructorName;
        StateHasChanged();
        await Task.Delay(100);
        await JS.InvokeVoidAsync("print");
        PrintTargetName = null; 
    }

    public class InstructorSelection
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsSelected { get; set; }
    }
}