using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Mvc.Models
{
    public class CourseListViewModel
    {
        public List<CourseDto> Courses { get; set; } = new();
    }
}
