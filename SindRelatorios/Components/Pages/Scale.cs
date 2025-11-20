using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Components.Pages;

/// <summary>
/// Classe Responsável pela exibição e gerenciamento das escalas de abertura.
/// 
public partial class Scale
{
    [Inject] private IOpeningRepository OpeningRepo { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!; 

    private List<OpeningCalendar> Openings { get; set; } = new();
    private bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        IsLoading = true;
        try
        {
            Openings = await OpeningRepo.GetAllWithDetailsAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void NavigateToNew() => Navigation.NavigateTo("escalas/nova");

    private void NavigateToDetails(Guid id) => Navigation.NavigateTo($"escalas/{id}");

    private async Task HandleDelete(OpeningCalendar item)
    {
        bool confirmed = await JS.InvokeAsync<bool>("confirm", $"Tem certeza que deseja excluir a escala de {item.Region} do dia {item.Date:dd/MM}?");
        
        if (confirmed)
        {
            await OpeningRepo.DeleteAsync(item.Id);
            await LoadData(); 
        }
    }
}