using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    [Authorize]
    public class RoleTestController : Controller
    {

        // ai login cũng vào được
        public IActionResult Index()
        {
            return View();
        }

        // chỉ ADMIN vào được
        [Authorize(Roles = RolesNames.Admin)]
        public IActionResult AdminOnly()
        {
            return View();
        }

        // chỉ INSTRUCTOR vào được
        [Authorize(Roles = RolesNames.Instructor)]
        public IActionResult InstructorOnly()
        {
            return View();
        }

        // chỉ STUDENT vào được
        [Authorize(Roles = RolesNames.Student)]
        public IActionResult StudentOnly()
        {
            return View();
        }
    }
}
