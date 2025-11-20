using Microsoft.AspNetCore.Components;
using SindRelatorios.Application.DTOs;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using SindRelatorios.Models.Entities.Enums;
using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages;
/// <summary>
/// Classe Responsável pela criação de uma nova escala de abertura.
/// </summary>
public partial class NewOpening
{
    [Inject] private IOpeningService OpeningService { get; set; } = default!;
    [Inject] private IRepository<InstructorEntity> InstructorRepo { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromForm]
    public CreateOpeningInput InputData { get; set; } = new();

    protected List<InstructorEntity> AllInstructors { get; set; } = new();
    
    private OpeningType _activeTab = OpeningType.Standard;

    protected override async Task OnInitializedAsync()
    {
        var data = await InstructorRepo.GetAllAsync();
        AllInstructors = data.OrderBy(x => x.Name).ToList();

        EnsureSlots();
    }

    private void ChangeTab(OpeningType type)
    {
        _activeTab = type;
        InputData.Type = type;
        
        InputData.Slots.Clear();
        EnsureSlots();
    }

    private void EnsureSlots()
    {
        if (_activeTab == OpeningType.Standard)
        {
            if (!InputData.Slots.Any()) { AddSlot(); AddSlot(); AddSlot(); }
        }
        else
        {
            if (!InputData.Slots.Any()) { AddSlot(); }
        }
    }

    protected void AddSlot() => InputData.Slots.Add(new CreateSlotInput { Shift = "NOITE" });
    protected void RemoveSlot(CreateSlotInput slot) => InputData.Slots.Remove(slot);

    protected async Task HandleSave()
    {
        InputData.Type = _activeTab;

        var validSlots = InputData.Slots.Where(s => !string.IsNullOrWhiteSpace(s.InstructorName)).ToList();
        if (!validSlots.Any()) return;

        InputData.Slots = validSlots;
        await OpeningService.CreateOpeningAsync(InputData);
        Navigation.NavigateTo("escalas");
    }
}