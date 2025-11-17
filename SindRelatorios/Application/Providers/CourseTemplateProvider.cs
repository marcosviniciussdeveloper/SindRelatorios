using SindRelatorios.Models;
using SindRelatorios.Providers;

namespace SindRelatorios.Application.Providers
{
    public class CourseTemplateProvider : ICourseTemplateProvider
    {
        // GABARITO 1: Primeira Habilitação (9 dias)
        private static readonly CourseTemplate FirstLicenseTemplate = new()
        {
            TotalDays = 9,
            SkipHolidays = true,
            SubjectTemplate = new()
            {
                { 1, "LEGISLAÇÃO (5 - HA)" },
                { 2, "LEGISLAÇÃO (5 - HA)" },
                { 3, "LEGISLAÇÃO (5 - HA)" },
                { 4, "LEGISLAÇÃO (3 - HA), MEIO AMBIENTE E CONVÍVIO SOCIAL (2 - HA)" },
                { 5, "MECÂNICA(3 - HA) CIDADANIA(2 - HA)" },
                { 6, "DIREÇÃO DEFENSIVA (5 - HA)" },
                { 7, "DIREÇÃO DEFENSIVA (5 - HA)" },
                { 8, "DIREÇÃO DEFENSIVA (5 - HA)" },
                { 9, "DIREÇÃO DEFENSIVA (1 - HA), PRIMEIROS SOCORROS (4 - HA)" }
            }
        };

       
        private static readonly CourseTemplate RecyclingTemplate = new()
        {
            TotalDays = 5,
            SkipHolidays = false, 
            SubjectTemplate = new()
            {
                { 1, "LEGISLAÇÃO (5 - HA)" },
                { 2, "LEGISLAÇÃO (5 - HA)" },
                { 3, "DIREÇÃO DEFENSIVA (6 - HA)" }, 
                { 4, "DIREÇÃO DEFENSIVA (2 - HA), PRIMEIROS SOCORROS (4 - HA)" },
                { 5, "RELACIONAMENTO INTERPESSOAL (6 - HA)" }
            }
        };

        public CourseTemplate GetTemplate(CourseType type)
        {
            return type switch
            {
                CourseType.FirstLicense => FirstLicenseTemplate,
                CourseType.Recycling => RecyclingTemplate,
                _ => throw new ArgumentException("Unknown course type")
            };
        }
    }
}