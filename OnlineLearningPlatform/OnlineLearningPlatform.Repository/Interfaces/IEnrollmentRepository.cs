using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<bool> IsUserEnrolledAsync(string userId, Guid courseId);
    }
}
