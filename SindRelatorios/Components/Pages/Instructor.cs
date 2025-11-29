using Microsoft.AspNetCore.Components;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages;

public partial class Instructor
{
    [Inject] private IRepository<InstructorEntity> InstructorRepository { get; set; } = default!;

    protected InstructorEntity CurrentInstructor { get; set; } = new();

    protected List<InstructorEntity> InstructorsList { get; set; } = new();
    protected string SearchTerm { get; set; } = "";

    protected bool ShowModal { get; set; } = false;
    protected bool IsEditing { get; set; } = false;

    protected List<InstructorEntity> FilteredList =>
        string.IsNullOrWhiteSpace(SearchTerm)
            ? InstructorsList
            : InstructorsList.Where(i => i.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        var data = await InstructorRepository.GetAllAsync();
        InstructorsList = data.OrderBy(x => x.Name).ToList();
    }

    // --- AÇÕES DO MODAL ---

    protected void OpenCreateModal()
    {
        CurrentInstructor = new InstructorEntity();
        IsEditing = false;
        ShowModal = true;
    }

    protected void OpenEditModal(InstructorEntity item)
    {
        CurrentInstructor = new InstructorEntity
        {
            Id = item.Id,
            Name = item.Name,
            Cpf = item.Cpf,
            Email = item.Email,
            TeleaulaPassword = item.TeleaulaPassword, // Novo campo
            Phone = item.Phone,
            Region = item.Region,
            AvailableMorning = item.AvailableMorning,
            AvailableAfternoon = item.AvailableAfternoon,
            AvailableNight = item.AvailableNight,
            Observation = item.Observation
        };
        IsEditing = true;
        ShowModal = true;
    }

    protected void CloseModal()
    {
        ShowModal = false;
    }

    // --- SALVAR (Cria ou Atualiza) ---
    protected async Task HandleSave()
    {
        if (string.IsNullOrWhiteSpace(CurrentInstructor.Name)) return;

        try
        {
            CurrentInstructor.Name = CurrentInstructor.Name.Trim().ToUpper();

            if (IsEditing)
            {
                await InstructorRepository.UpdateAsync(CurrentInstructor);
            }
            else
            {
                if (InstructorsList.Any(x => x.Name.Equals(CurrentInstructor.Name, StringComparison.OrdinalIgnoreCase)))
                    return;

                await InstructorRepository.AddAsync(CurrentInstructor);
            }

            await LoadData();
            CloseModal();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    protected async Task HandleDelete(Guid id)
    {
        await InstructorRepository.DeleteAsync(id);
        await LoadData();
    }

    protected string GetInitials(string name)
    {
        if (string.IsNullOrEmpty(name)) return "?";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0][0].ToString();
        return $"{parts[0][0]}{parts[parts.Length - 1][0]}";
    }
}