using OnlineLearningPlatform.Services.DTO.Response.Course;

namespace OnlineLearningPlatform.Mvc.Models
{
    public class CourseDetailViewModel
    {
        public CourseDetailDto? Course { get; set; }
        public bool CanEnroll { get; set; }
    }
}
