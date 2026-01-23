using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;

namespace OnlineLearningPlatform.Mvc.Areas.Admin.Controllers
{
    [Area(RolesNames.Admin)]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
