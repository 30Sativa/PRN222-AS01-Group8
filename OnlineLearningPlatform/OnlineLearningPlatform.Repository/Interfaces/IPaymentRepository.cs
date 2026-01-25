using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment?> GetPaymentByIdAsync(Guid paymentId);
        Task<Payment?> GetPaymentByEnrollmentIdAsync(Guid enrollmentId);
        Task<List<Payment>> GetUserPaymentsAsync(string userId);
        Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status, string? transactionId = null);
        Task<bool> UpdatePaymentEnrollmentIdAsync(Guid paymentId, Guid enrollmentId);
    }
}
