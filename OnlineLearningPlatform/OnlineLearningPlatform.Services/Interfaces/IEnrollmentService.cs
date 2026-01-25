using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<bool> EnrollInCourseAsync(string userId, Guid courseId);
        Task<PaymentResponseDto?> EnrollWithPaymentAsync(string userId, Guid courseId, string? paymentMethod = null);
        Task<bool> CompleteEnrollmentAfterPaymentAsync(Guid paymentId);
    }
}
