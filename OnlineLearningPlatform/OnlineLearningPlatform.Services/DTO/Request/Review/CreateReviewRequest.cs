using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Services.DTO.Request.Review
{
    public class CreateReviewRequest
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Bình luận phải có ít nhất 10 ký tự")]
        [MaxLength(1000, ErrorMessage = "Bình luận không được vượt quá 1000 ký tự")]
        public string Comment { get; set; }
    }
}
