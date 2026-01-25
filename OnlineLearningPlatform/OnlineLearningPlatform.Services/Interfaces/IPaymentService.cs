using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto?> CreatePaymentAsync(CreatePaymentRequest request);
        Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid paymentId);
        Task<bool> ProcessPaymentAsync(Guid paymentId, string paymentMethod);
        Task<List<PaymentResponseDto>> GetUserPaymentsAsync(string userId);
    }
}
