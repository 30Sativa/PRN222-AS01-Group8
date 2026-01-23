namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<bool> EnrollInCourseAsync(string userId, Guid courseId);
    }
}
