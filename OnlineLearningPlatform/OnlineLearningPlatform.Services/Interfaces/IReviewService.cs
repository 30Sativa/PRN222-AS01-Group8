using OnlineLearningPlatform.Services.DTO.Request.Review;
using OnlineLearningPlatform.Services.DTO.Response.Review;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto?> CreateReviewAsync(string userId, CreateReviewRequest request);
        Task<ReviewDto?> UpdateReviewAsync(string userId, UpdateReviewRequest request);
        Task<bool> DeleteReviewAsync(string userId, int reviewId);
        Task<List<ReviewDto>> GetCourseReviewsAsync(Guid courseId, string? currentUserId = null);
        Task<ReviewDto?> GetUserReviewForCourseAsync(string userId, Guid courseId);
        Task<CourseReviewSummaryDto> GetCourseReviewSummaryAsync(Guid courseId, string? currentUserId = null);
        Task<bool> CanUserReviewCourseAsync(string userId, Guid courseId);
    }
}
