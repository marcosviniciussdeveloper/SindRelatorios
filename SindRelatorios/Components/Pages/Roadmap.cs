using Microsoft.AspNetCore.Components;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using SindRelatorios.Models.Entities.Enums;

// Alias
using FeatureEntity = SindRelatorios.Models.Entities.AppFeature;
using SuggestionEntity = SindRelatorios.Models.Entities.UserSuggestion;

namespace SindRelatorios.Components.Pages;

public partial class Roadmap
{
    [Inject] private IRepository<FeatureEntity> FeatureRepo { get; set; } = default!;
    [Inject] private IRepository<SuggestionEntity> SuggestionRepo { get; set; } = default!; 

    protected List<FeatureEntity> Features { get; set; } = new();
    protected bool IsLoading { get; set; } = true;

    protected SuggestionEntity NewSuggestion { get; set; } = new();
    protected bool ShowSuggestionForm { get; set; } = false;
    protected bool SuggestionSent { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var data = await FeatureRepo.GetAllAsync();
            Features = data.OrderByDescending(x => x.CreatedAt).ToList();
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task HandleSubmitSuggestion()
    {
        if (string.IsNullOrWhiteSpace(NewSuggestion.Content)) return;

        await SuggestionRepo.AddAsync(NewSuggestion);
        
        NewSuggestion = new SuggestionEntity();
        ShowSuggestionForm = false;
        SuggestionSent = true;
    }
}