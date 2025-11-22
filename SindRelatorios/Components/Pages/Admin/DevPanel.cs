using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using SindRelatorios.Models.Entities.Enums;

namespace SindRelatorios.Components.Pages.Admin;

public partial class DevPanel
{
    // Agora injetamos o SERVIÇO, não o Repositório direto
    [Inject] private IFeatureService FeatureService { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    protected bool IsAuthenticated { get; set; } = false;
    protected string PasswordInput { get; set; } = "";
    protected string LoginMessage { get; set; } = "";

    protected List<AppFeature> FeaturesList { get; set; } = new();
    protected List<UserSuggestion> SuggestionsList { get; set; } = new();
    protected AppFeature CurrentFeature { get; set; } = new();
    
    protected bool ShowModal { get; set; } = false;
    protected bool IsEditing { get; set; } = false;
    protected string ActiveTab { get; set; } = "Features";

    protected void TryLogin()
    {
        var realPassword = Configuration["AdminSettings:MasterPassword"] ?? "";

        if (string.IsNullOrEmpty(realPassword))
        {
            LoginMessage = "Erro: Senha não configurada.";
            return;
        }

        if (PasswordInput == realPassword)
        {
            IsAuthenticated = true;
            LoginMessage = "";
            LoadData();
        }
        else
        {
            LoginMessage = "Senha incorreta.";
            PasswordInput = "";
        }
    }

    protected void Logout()
    {
        IsAuthenticated = false;
        PasswordInput = "";
        Nav.NavigateTo("/");
    }

    protected void EnterKey(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter") TryLogin();
    }

    private async void LoadData()
    {
        // O componente apenas pede os dados prontos ao serviço
        FeaturesList = await FeatureService.GetAllFeaturesAsync();
        SuggestionsList = await FeatureService.GetAllSuggestionsAsync();
        StateHasChanged();
    }

    protected void OpenCreateModal()
    {
        CurrentFeature = new AppFeature();
        IsEditing = false;
        ShowModal = true;
    }

    protected void OpenEditModal(AppFeature item)
    {
        CurrentFeature = new AppFeature
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Status = item.Status,
            CreatedAt = item.CreatedAt,
            CompletedAt = item.CompletedAt
        };
        IsEditing = true;
        ShowModal = true;
    }

    protected void CloseModal()
    {
        ShowModal = false;
    }

    protected async Task HandleSave()
    {
        // Delega a lógica de salvar/atualizar/datas para o serviço
        await FeatureService.SaveFeatureAsync(CurrentFeature);
        LoadData();
        CloseModal();
    }

    protected async Task HandleDelete(Guid id)
    {
        await FeatureService.DeleteFeatureAsync(id);
        LoadData();
    }

    protected async Task DeleteSuggestion(Guid id)
    {
        await FeatureService.DeleteSuggestionAsync(id);
        LoadData();
    }

    protected async Task PromoteToFeature(UserSuggestion suggestion)
    {
        // Delega a transação complexa para o serviço
        await FeatureService.PromoteSuggestionAsync(suggestion);
        LoadData();
        ActiveTab = "Features";
    }

    protected string GetBadgeClass(FeatureStatus status) => status switch
    {
        FeatureStatus.Idea => "bg-info text-dark",
        FeatureStatus.InProgress => "bg-warning text-dark",
        FeatureStatus.Completed => "bg-success text-white",
        _ => "bg-secondary"
    };
}