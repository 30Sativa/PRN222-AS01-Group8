using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
        }

        public async Task<bool> EnrollInCourseAsync(string userId, Guid courseId)
        {
            // Check if course exists
            if (!await _courseRepository.CourseExistsAsync(courseId))
            {
                return false;
            }

            // Check if already enrolled
            if (await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId))
            {
                return false; // Already enrolled
            }

            // Create enrollment
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
    }
}
