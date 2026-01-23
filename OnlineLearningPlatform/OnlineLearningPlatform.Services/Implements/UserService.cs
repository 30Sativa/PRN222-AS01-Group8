using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Services.DTO.Request.User;
using OnlineLearningPlatform.Services.DTO.Response.User;
using OnlineLearningPlatform.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<GetUserResponse>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<GetUserResponse>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                result.Add(new GetUserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = roles.FirstOrDefault(),
                    EmailConfirmed = u.EmailConfirmed,
                    IsLocked = u.LockoutEnd > DateTimeOffset.Now
                });
            }

            return result;
        }

        public async Task<GetUserResponse> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new GetUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
                EmailConfirmed = user.EmailConfirmed,
                IsLocked = user.LockoutEnd > DateTimeOffset.Now
            };
        }

        public async Task LockAsync(LockUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);
        }

        public async Task UnlockAsync(UnlockUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateAsync(UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
                throw new Exception("User not found");

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.UserName = request.Email;
            user.EmailConfirmed = request.EmailConfirmed;

            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateRoleAsync(UpdateUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, request.Role);
        }
    }
}
