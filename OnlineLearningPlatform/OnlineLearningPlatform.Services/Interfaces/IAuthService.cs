using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Services.DTO;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
    }
}
