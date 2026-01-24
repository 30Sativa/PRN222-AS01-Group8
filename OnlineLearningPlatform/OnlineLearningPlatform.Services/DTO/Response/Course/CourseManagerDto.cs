using OnlineLearningPlatform.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Services.DTO.Response.Course
{
    public class CourseManagerDto
    {
        public Guid CourseId { get; set; }

        public string Title { get; set; } = null!;
        public string TeacherName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;

        public decimal Price { get; set; }

        public CourseStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? RejectReason { get; set; }
    }
}
