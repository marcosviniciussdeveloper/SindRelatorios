
using SindRelatorios.Models.Entities;

namespace SindRelatorios.Application.Interfaces;

public interface ICourseTemplateProvider
{
    CourseTemplate GetTemplate(CourseType type);
}