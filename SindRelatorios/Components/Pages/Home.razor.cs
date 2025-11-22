using Microsoft.AspNetCore.Components;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using SindRelatorios.Models.Entities.Enums;

using InstructorEntity = SindRelatorios.Models.Entities.Instructor;
using FeatureEntity = SindRelatorios.Models.Entities.AppFeature;

namespace SindRelatorios.Components.Pages;

public partial class Home
{
    [Inject]
    private IRepository<InstructorEntity> InstructorRepo { get; set; } = default!;

    [Inject]
    private IOpeningRepository OpeningRepo { get; set; } = default!;

    [Inject]
    private IRepository<FeatureEntity> FeatureRepo { get; set; } = default!;

    private int TotalInstructors { get; set; }
    private int TotalScales { get; set; }
    private List<OpeningCalendar> NextOpenings { get; set; } = new();
    private List<FeatureEntity> LatestFeatures { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var instructors = await InstructorRepo.GetAllAsync();
        TotalInstructors = instructors.Count;

        var openings = await OpeningRepo.GetAllAsync();
        TotalScales = openings.Count;
        
        NextOpenings = openings
            .Where(x => x.Date >= DateTime.UtcNow.Date)
            .OrderBy(x => x.Date)
            .ToList();

        var features = await FeatureRepo.GetAllAsync();
        
        LatestFeatures = features
            .OrderByDescending(x => x.CreatedAt)
            .Take(3)
            .ToList();
    }

    private string GetBadgeClass(FeatureStatus status) => status switch
    {
        FeatureStatus.Idea => "bg-info text-dark",
        FeatureStatus.InProgress => "bg-warning text-dark",
        FeatureStatus.Completed => "bg-success text-white",
        _ => "bg-secondary"
    };

    private string GetStatusTranslate(FeatureStatus status) => status switch
    {
        FeatureStatus.Idea => "IDEIA",
        FeatureStatus.InProgress => "DESENVOLVENDO",
        FeatureStatus.Completed => "NOVO",
        _ => ""
    };
}