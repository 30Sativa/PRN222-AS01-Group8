namespace OnlineLearningPlatform.Services.DTO.Response.Teacher
{
    public class QuizDetailDto
    {
        public int QuizId { get; set; }
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<QuestionDetailDto> Questions { get; set; } = new List<QuestionDetailDto>();
    }

    public class QuestionDetailDto
    {
        public int QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public List<AnswerDetailDto> Answers { get; set; } = new List<AnswerDetailDto>();
    }

    public class AnswerDetailDto
    {
        public Guid AnswerId { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
