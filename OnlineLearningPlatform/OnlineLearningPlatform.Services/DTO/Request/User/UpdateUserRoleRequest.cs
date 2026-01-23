using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Services.DTO.Request.User
{
    public class UpdateUserRoleRequest
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}
