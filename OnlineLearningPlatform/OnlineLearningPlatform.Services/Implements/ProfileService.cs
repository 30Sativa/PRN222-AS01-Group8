using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Response;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ICourseService _courseService;

        public ProfileService(IProfileRepository profileRepository, ICourseService courseService)
        {
            _profileRepository = profileRepository;
            _courseService = courseService;
        }

        public async Task<ProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _profileRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var enrolledCourses = await _courseService.GetUserEnrolledCoursesAsync(userId);
            var courseCount = enrolledCourses?.Count ?? 0;

            return new ProfileDto
            {
                UserId = userId,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                CourseCount = courseCount,
                ArticleCount = 0
            };
        }

        public async Task<IdentityResult> UpdateFullNameAsync(string userId, string fullName)
        {
            var user = await _profileRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found"
                });
            }

            user.FullName = fullName;
            return await _profileRepository.UpdateAsync(user);
        }
    }
}

