using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Mvc.Models.Admin
{
    public class CreateUserViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; }

        public bool IsLocked { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
