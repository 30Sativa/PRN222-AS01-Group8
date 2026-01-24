using OnlineLearningPlatform.Models.Enums;

namespace OnlineLearningPlatform.Mvc.Models
{
    public class CourseManagerViewModel
    {
        public Guid CourseId { get; set; }

        public string Title { get; set; } = "";

        public string TeacherName { get; set; } = "";

        public string PriceText { get; set; } = "";

        public CourseStatus Status { get; set; }

        public string StatusText { get; set; } = "";
    }
}
