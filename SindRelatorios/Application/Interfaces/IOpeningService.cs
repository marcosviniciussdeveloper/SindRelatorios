using SindRelatorios.Application.DTOs;

namespace SindRelatorios.Application.Interfaces;

public interface IOpeningService
{
    Task CreateOpeningAsync(CreateOpeningInput input);
}