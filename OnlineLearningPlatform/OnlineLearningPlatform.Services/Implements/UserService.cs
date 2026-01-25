using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task CreateAsync(CreateUserRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                LockoutEnabled = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Add role
            if (!string.IsNullOrEmpty(request.Role))
            {
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            //// Lock nếu cần
            //if (request.IsLocked)
            //{
            //    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            //}
        }

        public async Task<List<GetUserResponse>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<GetUserResponse>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                // Hide admin accounts from user management listing
                if (roles.Contains(RolesNames.Admin))
                {
                    continue;
                }

                var primaryRole = roles.FirstOrDefault();

                result.Add(new GetUserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = primaryRole,
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
            var primaryRole = roles.Contains(RolesNames.Admin) ? RolesNames.Admin : roles.FirstOrDefault();

            return new GetUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = primaryRole,
                EmailConfirmed = user.EmailConfirmed,
                IsLocked = user.LockoutEnd > DateTimeOffset.Now
            };
        }

        public async Task<List<string>> GetRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task LockAsync(LockUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return;

            // Prevent locking admin accounts
            if (await _userManager.IsInRoleAsync(user, RolesNames.Admin))
                return;

            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);
        }

        public async Task UnlockAsync(UnlockUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return;

            // Prevent unlocking admin accounts (no-op for safety)
            if (await _userManager.IsInRoleAsync(user, RolesNames.Admin))
                return;

            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateAsync(UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
                throw new Exception("User not found");

            // Prevent editing admin accounts from management features
            if (await _userManager.IsInRoleAsync(user, RolesNames.Admin))
                return;

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.UserName = request.Email;
            user.EmailConfirmed = request.EmailConfirmed;

            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateRoleAsync(UpdateUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return;

            // Prevent changing role of admin accounts
            if (await _userManager.IsInRoleAsync(user, RolesNames.Admin))
                return;

            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, request.Role);
        }
    }
}
