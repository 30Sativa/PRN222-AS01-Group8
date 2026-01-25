using OnlineLearningPlatform.Services.DTO.Teacher;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models.Teacher
{
    public class ManageSectionsViewModel
    {
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public List<TeacherSectionDto> Sections { get; set; } = new List<TeacherSectionDto>();
    }
}
