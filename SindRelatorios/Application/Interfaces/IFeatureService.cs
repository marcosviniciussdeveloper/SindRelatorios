using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.Interfaces;

public interface IFeatureService
{
    Task<List<AppFeature>> GetAllFeaturesAsync();
    Task<List<UserSuggestion>> GetAllSuggestionsAsync();
    
    Task SaveFeatureAsync(AppFeature feature); 
    Task DeleteFeatureAsync(Guid id);
    
    Task PromoteSuggestionAsync(UserSuggestion suggestion); 
    Task DeleteSuggestionAsync(Guid id);
}