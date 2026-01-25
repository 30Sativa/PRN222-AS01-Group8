namespace OnlineLearningPlatform.Services.DTO.Response.Teacher
{
    /// <summary>
    /// DTO chi tiết tiến độ học tập của 1 học viên trong khóa học
    /// </summary>
    public class StudentProgressDto
    {
        // Thông tin học viên
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        
        // Thông tin khóa học
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        
        // Thông tin đăng ký
        public DateTime EnrolledAt { get; set; }
        public decimal OverallProgress { get; set; } // Phần trăm tổng thể (0-100)
        
        // Thống kê
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalQuizzes { get; set; }
        public int CompletedQuizzes { get; set; }
        public int TotalAssignments { get; set; }
        public int CompletedAssignments { get; set; }
        
        // Chi tiết tiến độ theo section
        public List<SectionProgressDto> SectionProgress { get; set; } = new();
    }
    
    /// <summary>
    /// Tiến độ của 1 section
    /// </summary>
    public class SectionProgressDto
    {
        public int SectionId { get; set; }
        public string SectionTitle { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public decimal Progress { get; set; } // Phần trăm (0-100)
        
        // Chi tiết từng bài học
        public List<LessonProgressDto> LessonProgress { get; set; } = new();
    }
    
    /// <summary>
    /// Tiến độ của 1 bài học
    /// </summary>
    public class LessonProgressDto
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public string LessonType { get; set; } = string.Empty; // video, text, quiz
        
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal ProgressPercentage { get; set; } // 0 hoặc 100
    }
}
