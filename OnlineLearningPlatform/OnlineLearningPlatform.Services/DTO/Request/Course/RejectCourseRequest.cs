using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Services.DTO.Request.Course
{
    public class RejectCourseRequest
    {
        public Guid CourseId { get; set; }

        public string Reason { get; set; } = null!;
    }
}
