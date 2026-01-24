using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Services.DTO.Response;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetProfileAsync(string userId);
        Task<IdentityResult> UpdateFullNameAsync(string userId, string fullName);
    }
}

