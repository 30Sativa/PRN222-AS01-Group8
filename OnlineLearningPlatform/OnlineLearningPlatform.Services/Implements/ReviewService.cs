using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Request.Review;
using OnlineLearningPlatform.Services.DTO.Response.Review;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ApplicationDbContext _context;

        public ReviewService(
            IReviewRepository reviewRepository,
            IEnrollmentRepository enrollmentRepository,
            ApplicationDbContext context)
        {
            _reviewRepository = reviewRepository;
            _enrollmentRepository = enrollmentRepository;
            _context = context;
        }

        public async Task<bool> CanUserReviewCourseAsync(string userId, Guid courseId)
        {
            // Chỉ cho phép review nếu user đã đăng ký khóa học
            return await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId);
        }

        public async Task<ReviewDto?> CreateReviewAsync(string userId, CreateReviewRequest request)
        {
            // Kiểm tra user đã đăng ký khóa học chưa
            if (!await CanUserReviewCourseAsync(userId, request.CourseId))
            {
                return null; // User chưa đăng ký khóa học
            }

            // Kiểm tra user đã review chưa
            var existingReview = await _reviewRepository.GetUserReviewForCourseAsync(userId, request.CourseId);
            if (existingReview != null)
            {
                return null; // User đã review rồi, cần update thay vì create
            }

            var review = new Review
            {
                CourseId = request.CourseId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            var createdReview = await _reviewRepository.CreateReviewAsync(review);
            return await MapToDtoAsync(createdReview, userId);
        }

        public async Task<ReviewDto?> UpdateReviewAsync(string userId, UpdateReviewRequest request)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId);
            if (review == null || review.UserId != userId)
            {
                return null; // Review không tồn tại hoặc không phải của user này
            }

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            var updated = await _reviewRepository.UpdateReviewAsync(review);
            if (!updated)
            {
                return null;
            }

            return await MapToDtoAsync(review, userId);
        }

        public async Task<bool> DeleteReviewAsync(string userId, int reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
            {
                return false; // Review không tồn tại hoặc không phải của user này
            }

            return await _reviewRepository.DeleteReviewAsync(reviewId);
        }

        public async Task<List<ReviewDto>> GetCourseReviewsAsync(Guid courseId, string? currentUserId = null)
        {
            var reviews = await _reviewRepository.GetReviewsByCourseIdAsync(courseId);
            var reviewDtos = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                var dto = await MapToDtoAsync(review, currentUserId);
                if (dto != null)
                {
                    reviewDtos.Add(dto);
                }
            }

            return reviewDtos;
        }

        public async Task<ReviewDto?> GetUserReviewForCourseAsync(string userId, Guid courseId)
        {
            var review = await _reviewRepository.GetUserReviewForCourseAsync(userId, courseId);
            if (review == null)
            {
                return null;
            }

            return await MapToDtoAsync(review, userId);
        }

        public async Task<CourseReviewSummaryDto> GetCourseReviewSummaryAsync(Guid courseId, string? currentUserId = null)
        {
            var averageRating = await _reviewRepository.GetAverageRatingAsync(courseId);
            var totalReviews = await _reviewRepository.GetReviewCountAsync(courseId);

            // Đếm số lượng theo từng rating
            var reviews = await _context.Reviews
                .Where(r => r.CourseId == courseId && r.Rating.HasValue)
                .Select(r => r.Rating.Value)
                .ToListAsync();

            var ratingCounts = new Dictionary<int, int>
            {
                { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
            };

            foreach (var rating in reviews)
            {
                if (ratingCounts.ContainsKey(rating))
                {
                    ratingCounts[rating]++;
                }
            }

            ReviewDto? userReview = null;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                userReview = await GetUserReviewForCourseAsync(currentUserId, courseId);
            }

            return new CourseReviewSummaryDto
            {
                AverageRating = Math.Round(averageRating, 1),
                TotalReviews = totalReviews,
                Rating1Count = ratingCounts[1],
                Rating2Count = ratingCounts[2],
                Rating3Count = ratingCounts[3],
                Rating4Count = ratingCounts[4],
                Rating5Count = ratingCounts[5],
                UserReview = userReview
            };
        }

        private async Task<ReviewDto?> MapToDtoAsync(Review review, string? currentUserId = null)
        {
            if (review == null)
            {
                return null;
            }

            // Load User và Course nếu chưa được include
            if (review.User == null || review.Course == null)
            {
                review = await _context.Reviews
                    .Include(r => r.User)
                    .Include(r => r.Course)
                    .FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);
                
                if (review == null)
                {
                    return null;
                }
            }

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                CourseId = review.CourseId ?? Guid.Empty,
                CourseTitle = review.Course?.Title ?? "",
                UserId = review.UserId,
                UserName = review.User?.FullName ?? review.User?.Email ?? "Unknown",
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = DateTime.UtcNow, // Entity không có CreatedAt, dùng thời gian hiện tại
                CanEdit = !string.IsNullOrEmpty(currentUserId) && review.UserId == currentUserId
            };
        }
    }
}
