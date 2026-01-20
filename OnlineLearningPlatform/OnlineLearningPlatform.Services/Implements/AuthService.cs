using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
