using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Services.DTO.Response.User
{
    public class GetUserResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsLocked { get; set; }
    }
}
