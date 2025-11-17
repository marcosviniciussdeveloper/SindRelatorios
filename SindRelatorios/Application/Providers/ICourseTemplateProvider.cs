using SindRelatorios.Data;
using SindRelatorios.Models;

namespace SindRelatorios.Providers;

public interface ICourseTemplateProvider
{
    CourseTemplate GetTemplate(CourseType type);
}