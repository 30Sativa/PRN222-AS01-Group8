using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Services.DTO.Request;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<IdentityResult> RegisterAsync(RegisterRequest request);

        Task<List<ApplicationUser>> GetAllAccount(); 
    }
}
