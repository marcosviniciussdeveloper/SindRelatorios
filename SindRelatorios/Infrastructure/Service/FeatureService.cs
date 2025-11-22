using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using SindRelatorios.Models.Entities.Enums;

namespace SindRelatorios.Infrastructure.Services;

public class FeatureService : IFeatureService
{
    private readonly IRepository<AppFeature> _featureRepo;
    private readonly IRepository<UserSuggestion> _suggestionRepo;

    public FeatureService(
        IRepository<AppFeature> featureRepo, 
        IRepository<UserSuggestion> suggestionRepo)
    {
        _featureRepo = featureRepo;
        _suggestionRepo = suggestionRepo;
    }

    public async Task<List<AppFeature>> GetAllFeaturesAsync()
    {
        var data = await _featureRepo.GetAllAsync();
        // Regra de ordenação fica aqui no serviço, não na tela
        return data
            .OrderBy(x => x.Status == FeatureStatus.Completed)
            .ThenByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<List<UserSuggestion>> GetAllSuggestionsAsync()
    {
        var data = await _suggestionRepo.GetAllAsync();
        return data.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public async Task SaveFeatureAsync(AppFeature feature)
    {
        // REGRA DE NEGÓCIO: Se marcou como concluído, define a data automaticamente
        if (feature.Status == FeatureStatus.Completed && feature.CompletedAt == null)
        {
            feature.CompletedAt = DateTime.UtcNow;
        }

        if (feature.Id == Guid.Empty)
        {
            await _featureRepo.AddAsync(feature);
        }
        else
        {
            await _featureRepo.UpdateAsync(feature);
        }
    }

    public async Task DeleteFeatureAsync(Guid id)
    {
        await _featureRepo.DeleteAsync(id);
    }

    public async Task PromoteSuggestionAsync(UserSuggestion suggestion)
    {
        // REGRA DE NEGÓCIO: Promover = Criar Feature + Deletar Sugestão
        var newFeature = new AppFeature
        {
            Title = "Sugestão: " + (suggestion.Content.Length > 20 ? suggestion.Content.Substring(0, 20) + "..." : suggestion.Content),
            Description = suggestion.Content,
            Status = FeatureStatus.Idea,
            CreatedAt = DateTime.UtcNow
        };

        await _featureRepo.AddAsync(newFeature);
        await _suggestionRepo.DeleteAsync(suggestion.Id);
    }

    public async Task DeleteSuggestionAsync(Guid id)
    {
        await _suggestionRepo.DeleteAsync(id);
    }
}