using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Mvc.Models.Admin;
using OnlineLearningPlatform.Services.DTO.Request.User;
using OnlineLearningPlatform.Services.Implements;
using OnlineLearningPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OnlineLearningPlatform.Mvc.Areas.Admin.Controllers
{
    [Area(RolesNames.Admin)]
    [Authorize(Roles = RolesNames.Admin)]
    public class UserController : Controller
    {
        private readonly IUserService _service;
       

        public UserController(IUserService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllAsync();

            var vm = dtos.Select(x => new UserViewModel
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                Role = x.Role,
                IsLocked = x.IsLocked
            }).ToList();

            return View(vm);
        }

        public async Task<IActionResult> Lock(string id)
        {
            await _service.LockAsync(new LockUserRequest { UserId = id });

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Unlock(string id)
        {
            await _service.UnlockAsync(new UnlockUserRequest { UserId = id });

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null) return NotFound();

            var vm = new UserEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var request = new UpdateUserRequest
            {
                UserId = vm.Id,
                FullName = vm.FullName,
                Email = vm.Email,
                EmailConfirmed = true
            };

            await _service.UpdateAsync(request);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var users = await _service.GetAllAsync();

            var user = users.FirstOrDefault(x => x.Id == id);

            var vm = new UpdateUserRoleViewModel
            {
                Id = user.Id,
                Role = user.Role,
                Roles = new List<string>
        {
            RolesNames.Admin,
            RolesNames.Instructor,
            RolesNames.Student
        }
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(UpdateUserRoleViewModel model)
        {
            var request = new UpdateUserRoleRequest
            {
                UserId = model.Id,
                Role = model.Role
            };

            await _service.UpdateRoleAsync(request);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Create()
        {
            var roles = await _service.GetRolesAsync();

            var vm = new CreateUserViewModel
            {
                Roles = roles
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var request = new CreateUserRequest
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Password = vm.Password,
                Role = vm.Role,
            
            };

            await _service.CreateAsync(request);

            return RedirectToAction(nameof(Index));
        }



    }
}
