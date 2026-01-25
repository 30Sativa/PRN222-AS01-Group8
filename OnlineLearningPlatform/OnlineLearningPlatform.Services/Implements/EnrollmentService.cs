using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.DTO.Response;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository,
            IPaymentRepository paymentRepository,
            IPaymentService paymentService)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }

        public async Task<bool> EnrollInCourseAsync(string userId, Guid courseId)
        {
            // Check if course exists
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return false;
            }

            // If course is free (Price = 0), enroll directly
            if (course.Price == 0)
            {
                // Check if already enrolled
                if (await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId))
                {
                    return false; // Already enrolled
                }

                // Create enrollment without payment
                var enrollment = new Enrollment
                {
                    EnrollmentId = Guid.NewGuid(),
                    UserId = userId,
                    CourseId = courseId,
                    EnrolledAt = DateTime.UtcNow
                };

                await _enrollmentRepository.CreateEnrollmentAsync(enrollment);
                return true;
            }

            // For paid courses, require payment
            return false;
        }

        public async Task<PaymentResponseDto?> EnrollWithPaymentAsync(string userId, Guid courseId, string? paymentMethod = null)
        {
            // Check if course exists
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return null;
            }

            // Check if already enrolled
            if (await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId))
            {
                return null; // Already enrolled
            }

            // If course is free, enroll directly
            if (course.Price == 0)
            {
                var enrollment = new Enrollment
                {
                    EnrollmentId = Guid.NewGuid(),
                    UserId = userId,
                    CourseId = courseId,
                    EnrolledAt = DateTime.UtcNow
                };

                await _enrollmentRepository.CreateEnrollmentAsync(enrollment);
                return null; // No payment needed
            }

            // Create payment request
            var paymentRequest = new CreatePaymentRequest
            {
                UserId = userId,
                CourseId = courseId,
                Amount = course.Price,
                PaymentMethod = paymentMethod ?? "demo"
            };

            // Create payment
            var payment = await _paymentService.CreatePaymentAsync(paymentRequest);
            return payment;
        }

        public async Task<bool> CompleteEnrollmentAfterPaymentAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (payment == null || payment.Status != "success")
            {
                return false;
            }

            // Check if enrollment already exists
            if (payment.EnrollmentId.HasValue)
            {
                var existingEnrollment = await _enrollmentRepository.GetEnrollmentByStudentAndCourseAsync(
                    payment.UserId, 
                    payment.CourseId);
                if (existingEnrollment != null)
                {
                    return true; // Already enrolled
                }
            }

            // Check if already enrolled
            if (await _enrollmentRepository.IsUserEnrolledAsync(payment.UserId, payment.CourseId))
            {
                return false; // Already enrolled
            }

            // Create enrollment
            var enrollment = new Enrollment
            {
                EnrollmentId = Guid.NewGuid(),
                UserId = payment.UserId,
                CourseId = payment.CourseId,
                PaymentId = payment.PaymentId,
                EnrolledAt = DateTime.UtcNow
            };

            await _enrollmentRepository.CreateEnrollmentAsync(enrollment);

            // Update payment with enrollment ID
            await _paymentRepository.UpdatePaymentEnrollmentIdAsync(payment.PaymentId, enrollment.EnrollmentId);

            return true;
        }
    }
}
