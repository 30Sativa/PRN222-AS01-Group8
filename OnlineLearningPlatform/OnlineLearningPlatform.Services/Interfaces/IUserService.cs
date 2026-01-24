using OnlineLearningPlatform.Services.DTO.Request.User;
using OnlineLearningPlatform.Services.DTO.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<GetUserResponse>> GetAllAsync();
        Task<GetUserResponse> GetByIdAsync(string id);
        Task UpdateAsync(UpdateUserRequest request);
        Task<List<string>> GetRolesAsync();
        Task CreateAsync(CreateUserRequest request);
        Task LockAsync(LockUserRequest request);

        Task UnlockAsync(UnlockUserRequest request);

        Task UpdateRoleAsync(UpdateUserRoleRequest request);
    }
}
