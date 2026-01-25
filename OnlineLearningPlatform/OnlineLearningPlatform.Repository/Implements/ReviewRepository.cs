using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        public async Task<Review?> GetUserReviewForCourseAsync(string userId, Guid courseId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);
        }

        public async Task<List<Review>> GetReviewsByCourseIdAsync(Guid courseId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.CourseId == courseId)
                .OrderByDescending(r => r.ReviewId) // Sắp xếp theo ReviewId (mới nhất trước)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Course)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReviewId) // Sắp xếp theo ReviewId (mới nhất trước)
                .ToListAsync();
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return false;
            }

            _context.Reviews.Remove(review);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<double> GetAverageRatingAsync(Guid courseId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.CourseId == courseId && r.Rating.HasValue)
                .Select(r => r.Rating.Value)
                .ToListAsync();

            if (reviews.Count == 0)
            {
                return 0;
            }

            return reviews.Average();
        }

        public async Task<int> GetReviewCountAsync(Guid courseId)
        {
            return await _context.Reviews
                .CountAsync(r => r.CourseId == courseId);
        }
    }
}
