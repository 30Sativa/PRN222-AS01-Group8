using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(IAuthRepository authRepository, SignInManager<ApplicationUser> signInManager)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
        }



        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            if(user == null)
            {
                return SignInResult.Failed;
            }
            return await _signInManager.PasswordSignInAsync(
                     user.UserName,
                     password,
                     rememberMe,
                     lockoutOnFailure: false
            );
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName
            };

            return await _authRepository.CreateUserAsync(user, request.Password);
        }
    }
}
