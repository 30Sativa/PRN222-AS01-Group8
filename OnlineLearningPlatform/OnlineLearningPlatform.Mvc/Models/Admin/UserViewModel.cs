using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models.Admin
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }
    }
}
