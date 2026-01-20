using OnlineLearningPlatform.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        
    }
}
