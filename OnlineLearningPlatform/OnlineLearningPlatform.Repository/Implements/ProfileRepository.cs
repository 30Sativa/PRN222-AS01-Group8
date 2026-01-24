using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Repositories.Interfaces;

namespace OnlineLearningPlatform.Repositories.Implements
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            return _userManager.UpdateAsync(user);
        }
    }
}

