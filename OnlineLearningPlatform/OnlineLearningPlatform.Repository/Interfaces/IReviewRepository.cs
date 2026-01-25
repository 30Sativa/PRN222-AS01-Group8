using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task<Review?> GetUserReviewForCourseAsync(string userId, Guid courseId);
        Task<List<Review>> GetReviewsByCourseIdAsync(Guid courseId);
        Task<List<Review>> GetReviewsByUserIdAsync(string userId);
        Task<Review> CreateReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<double> GetAverageRatingAsync(Guid courseId);
        Task<int> GetReviewCountAsync(Guid courseId);
    }
}
