using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.DTO.Response;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICourseRepository _courseRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ICourseRepository courseRepository)
        {
            _paymentRepository = paymentRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PaymentResponseDto?> CreatePaymentAsync(CreatePaymentRequest request)
        {
            // Verify course exists and get course info
            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId);
            if (course == null)
            {
                return null;
            }

            // Create payment
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                UserId = request.UserId,
                CourseId = request.CourseId,
                Amount = request.Amount,
                Status = "pending",
                PaymentMethod = request.PaymentMethod ?? "demo",
                PaymentDate = DateTime.UtcNow
            };

            var createdPayment = await _paymentRepository.CreatePaymentAsync(payment);

            return MapToDto(createdPayment, course.Title);
        }

        public async Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return null;
            }

            return MapToDto(payment, payment.Course?.Title ?? "");
        }

        public async Task<bool> ProcessPaymentAsync(Guid paymentId, string paymentMethod)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (payment == null || payment.Status != "pending")
            {
                return false;
            }

            // Demo payment processing - simulate success
            // In real application, this would call payment gateway API
            var transactionId = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{paymentId.ToString().Substring(0, 8).ToUpper()}";
            
            return await _paymentRepository.UpdatePaymentStatusAsync(
                paymentId, 
                "success", 
                transactionId);
        }

        public async Task<List<PaymentResponseDto>> GetUserPaymentsAsync(string userId)
        {
            var payments = await _paymentRepository.GetUserPaymentsAsync(userId);
            return payments.Select(p => MapToDto(p, p.Course?.Title ?? "")).ToList();
        }

        private PaymentResponseDto MapToDto(Payment payment, string courseTitle)
        {
            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                UserId = payment.UserId,
                CourseId = payment.CourseId,
                CourseTitle = courseTitle,
                EnrollmentId = payment.EnrollmentId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate,
                CompletedAt = payment.CompletedAt
            };
        }
    }
}
