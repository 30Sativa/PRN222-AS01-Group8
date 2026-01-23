using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Mvc.Models
{
    public class CourseDetailViewModel
    {
        public CourseDetailDto? Course { get; set; }
        public bool CanEnroll { get; set; }
    }
}
