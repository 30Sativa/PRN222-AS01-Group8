namespace OnlineLearningPlatform.Mvc.Models.Admin
{
    public class UpdateUserRoleViewModel
    {
        public string Id { get; set; }

        public string Role { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
