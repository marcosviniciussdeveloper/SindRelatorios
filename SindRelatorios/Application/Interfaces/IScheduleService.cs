using SindRelatorios.Application.DTOs;
using SindRelatorios.Models;
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleResult> GeneratePreviewAsync(GeneratorInput input);

    }
}