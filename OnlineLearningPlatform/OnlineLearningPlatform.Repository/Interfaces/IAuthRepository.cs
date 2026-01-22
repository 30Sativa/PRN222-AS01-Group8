using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Models.Identity;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<List<ApplicationUser?>> GetAllUsersAsync();
    }
}
