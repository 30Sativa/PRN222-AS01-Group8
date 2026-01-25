namespace OnlineLearningPlatform.Services.DTO.Response.Review
{
    public class CourseReviewSummaryDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int Rating1Count { get; set; }
        public int Rating2Count { get; set; }
        public int Rating3Count { get; set; }
        public int Rating4Count { get; set; }
        public int Rating5Count { get; set; }
        public ReviewDto? UserReview { get; set; } // Review của user hiện tại (nếu có)
    }
}
