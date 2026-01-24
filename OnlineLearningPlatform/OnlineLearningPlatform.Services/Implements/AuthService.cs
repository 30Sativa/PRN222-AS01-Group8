using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.DTO.Response;
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

        public async Task<List<ApplicationUser>> GetAllAccount()
        {
            return await _authRepository.GetAllUsersAsync();
        }

        public async Task<UserProfileResponse> GetMyProfileAsync(string userId)
        {
            var user = await _authRepository.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found.");

            return new UserProfileResponse
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            if (user == null)
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
            try
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName
                };

                var result = await _authRepository.CreateUserAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return result;
                }
                return await _signInManager.UserManager.AddToRoleAsync(user, request.Role);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("An error occurred while registering the user.", ex);

            }
        }

        public async Task UpdateProfileAsync(string userId, UpdateUserProfileRequest request)
        {
            var user = await _authRepository.GetByIdAsync(userId)
                ?? throw new ApplicationException("User not found.");

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;

            await _authRepository.UpdateAsync(user);
        }
    }
}
