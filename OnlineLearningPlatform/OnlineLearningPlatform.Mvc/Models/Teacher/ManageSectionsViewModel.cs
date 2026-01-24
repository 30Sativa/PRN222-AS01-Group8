using OnlineLearningPlatform.Services.DTO.Response;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models.Teacher
{
    public class ManageSectionsViewModel
    {
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public List<SectionDto> Sections { get; set; } = new List<SectionDto>();
    }
}
