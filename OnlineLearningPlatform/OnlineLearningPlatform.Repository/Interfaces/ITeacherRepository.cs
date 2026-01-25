using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Models.Enums;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    /// <summary>
    /// Repository riêng cho Teacher - Quản lý khóa học của giảng viên
    /// </summary>
    public interface ITeacherRepository
    {
        // Lấy danh sách khóa học của teacher
        Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId);

        // Lấy danh sách khóa học của teacher theo trạng thái (null = tất cả)
        Task<List<Course>> GetCoursesByTeacherIdAndStatusAsync(string teacherId, CourseStatus? status = null);

        // Lấy chi tiết khóa học với thống kê
        Task<Course?> GetCourseWithStatisticsAsync(Guid courseId, string teacherId);

        // Tạo khóa học mới
        Task<Course> CreateCourseAsync(Course course);

        // Cập nhật khóa học
        Task<bool> UpdateCourseAsync(Course course);

        // Xóa khóa học
        Task<bool> DeleteCourseAsync(Guid courseId, string teacherId);

        // Kiểm tra quyền sở hữu khóa học
        Task<bool> IsTeacherOwnsCourseAsync(Guid courseId, string teacherId);

        // ===== QUẢN LÝ SECTIONS =====
        // Lấy danh sách sections và lessons của khóa học
        Task<List<Section>> GetCourseSectionsAsync(Guid courseId, string teacherId);

        // Lấy chi tiết section
        Task<Section?> GetSectionByIdAsync(int sectionId);

        // Tạo section mới
        Task<Section> CreateSectionAsync(Section section);

        // Cập nhật section
        Task<bool> UpdateSectionAsync(Section section);

        // Xóa section
        Task<bool> DeleteSectionAsync(int sectionId);

        // ===== QUẢN LÝ LESSONS =====
        // Lấy chi tiết lesson
        Task<Lesson?> GetLessonByIdAsync(int lessonId);

        // Tạo lesson mới
        Task<Lesson> CreateLessonAsync(Lesson lesson);

        // Cập nhật lesson
        Task<bool> UpdateLessonAsync(Lesson lesson);

        // Xóa lesson
        Task<bool> DeleteLessonAsync(int lessonId);

        // ===== QUẢN LÝ QUIZ =====
        // Lấy quiz theo lessonId
        Task<Quiz?> GetQuizByLessonIdAsync(int lessonId);

        // Lấy quiz với đầy đủ questions và answers
        Task<Quiz?> GetQuizWithDetailsAsync(int quizId);

        // Lấy danh sách quiz theo danh sách lessonIds
        Task<List<Quiz>> GetQuizzesByLessonIdsAsync(List<int> lessonIds);

        // Tạo quiz mới
        Task<Quiz> CreateQuizAsync(Quiz quiz);

        // Cập nhật quiz
        Task<bool> UpdateQuizAsync(Quiz quiz);
        Task<bool> UpdateQuizWithQuestionsAsync(Quiz quiz, List<Question> questionsToDelete, List<QuizAnswer> answersToDelete, List<Question> newQuestions);

        // Xóa quiz
        Task<bool> DeleteQuizAsync(int quizId);
    }
}
