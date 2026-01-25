using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Mvc.Areas.Admin.Controllers
{
    [Area(RolesNames.Admin)]
    [Authorize(Roles = RolesNames.Admin)]
    public class AdminController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public AdminController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _statisticsService.GetDashboardStatisticsAsync();
            return View(model);
        }
    }
}
