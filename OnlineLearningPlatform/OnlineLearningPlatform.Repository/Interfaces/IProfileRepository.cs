using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Models.Identity;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IProfileRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
    }
}

