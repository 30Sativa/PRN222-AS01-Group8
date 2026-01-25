using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Mvc.Models.Teacher;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

namespace OnlineLearningPlatform.Mvc.Controllers
{
    [Authorize(Roles = RolesNames.Instructor)]
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        // GET: Teacher/Index - Quản lý danh sách khóa học của giảng viên (gồm khóa chờ duyệt + đã xuất bản)
        public async Task<IActionResult> Index(string? searchString)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var pending = await _teacherService.GetTeacherPendingCoursesAsync(teacherId);
            var published = await _teacherService.GetTeacherCoursesAsync(teacherId, searchString);
            var rejected = await _teacherService.GetTeacherRejectedCoursesAsync(teacherId);

            ViewData["CurrentFilter"] = searchString;

            var viewModel = new TeacherIndexViewModel
            {
                PendingCourses = pending,
                PublishedCourses = published,
                RejectedCourses = rejected
            };

            return View(viewModel);
        }

        // ===== QUẢN LÝ DANH MỤC KHÓA HỌC =====

        // GET: Teacher/Categories - Danh sách danh mục khóa học
        public async Task<IActionResult> Categories()
        {
            var list = await _teacherService.GetCategoriesForManagementAsync();
            return View(list);
        }

        // GET: Teacher/CreateCategory
        public IActionResult CreateCategory()
        {
            return View(new CreateCategoryViewModel());
        }

        // POST: Teacher/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var id = await _teacherService.CreateCategoryAsync(model.CategoryName);
            TempData["SuccessMessage"] = "Tạo danh mục thành công!";
            return RedirectToAction(nameof(Categories));
        }

        // GET: Teacher/EditCategory/{id}
        public async Task<IActionResult> EditCategory(int id)
        {
            var categories = await _teacherService.GetCategoriesForManagementAsync();
            var cat = categories.FirstOrDefault(c => c.CategoryId == id);
            if (cat == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Categories));
            }
            var viewModel = new EditCategoryViewModel
            {
                CategoryId = cat.CategoryId,
                CategoryName = cat.CategoryName
            };
            return View(viewModel);
        }

        // POST: Teacher/EditCategory/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, EditCategoryViewModel model)
        {
            if (id != model.CategoryId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(model);
            var ok = await _teacherService.UpdateCategoryAsync(model.CategoryId, model.CategoryName);
            if (ok)
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
            else
                TempData["ErrorMessage"] = "Không thể cập nhật danh mục.";
            return RedirectToAction(nameof(Categories));
        }

        // POST: Teacher/DeleteCategory/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var (success, message) = await _teacherService.DeleteCategoryAsync(id);
            if (success)
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;
            return RedirectToAction(nameof(Categories));
        }

        // GET: Teacher/Create - Hiển thị form tạo khóa học
        public async Task<IActionResult> Create()
        {
            var categories = await _teacherService.GetCategoriesAsync();
            var viewModel = new CreateCourseViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Teacher/Create - Xử lý tạo khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _teacherService.GetCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
                return View(model);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var request = new CreateCourseRequest
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId
            };

            var courseId = await _teacherService.CreateCourseAsync(request, teacherId);

            TempData["SuccessMessage"] = "Tạo khóa học thành công!";
            return RedirectToAction(nameof(Edit), new { id = courseId });
        }

        // GET: Teacher/Edit/{id} - Hiển thị form chỉnh sửa khóa học
        public async Task<IActionResult> Edit(Guid id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var course = await _teacherService.GetTeacherCourseByIdAsync(id, teacherId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khóa học hoặc bạn không có quyền chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _teacherService.GetCategoriesAsync();
            var viewModel = new EditCourseViewModel
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList()
            };

            // Set CategoryId nếu có
            if (!string.IsNullOrEmpty(course.CategoryName))
            {
                var selectedCategory = categories.FirstOrDefault(c => c.CategoryName == course.CategoryName);
                if (selectedCategory != null)
                {
                    viewModel.CategoryId = selectedCategory.CategoryId;
                }
            }

            return View(viewModel);
        }

        // POST: Teacher/Edit/{id} - Xử lý cập nhật khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditCourseViewModel model)
        {
            if (id != model.CourseId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var categories = await _teacherService.GetCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
                return View(model);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var request = new UpdateCourseRequest
            {
                CourseId = model.CourseId,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId
            };

            var result = await _teacherService.UpdateCourseAsync(id, request, teacherId);

            if (result)
            {
                TempData["SuccessMessage"] = "Cập nhật khóa học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật khóa học.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Teacher/Delete/{id} - Xóa khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var result = await _teacherService.DeleteCourseAsync(id, teacherId);

            if (result)
            {
                TempData["SuccessMessage"] = "Xóa khóa học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa khóa học.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===== QUẢN LÝ SECTIONS VÀ LESSONS =====

        // GET: Teacher/ManageSections/{id} - Quản lý chương và bài học của khóa
        public async Task<IActionResult> ManageSections(Guid id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var course = await _teacherService.GetTeacherCourseByIdAsync(id, teacherId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khóa học.";
                return RedirectToAction(nameof(Index));
            }

            var sections = await _teacherService.GetCourseSectionsAsync(id, teacherId);

            // Lấy thông tin quiz cho tất cả lessons
            var allLessonIds = sections.SelectMany(s => s.Lessons?.Select(l => l.LessonId) ?? Enumerable.Empty<int>()).ToList();
            var quizzes = await _teacherService.GetQuizzesByLessonIdsAsync(allLessonIds);
            ViewBag.Quizzes = quizzes;

            var viewModel = new ManageSectionsViewModel
            {
                CourseId = id,
                CourseTitle = course.Title,
                Sections = sections
            };

            return View(viewModel);
        }

        // GET: Teacher/CreateSection?courseId=xxx
        public IActionResult CreateSection(Guid courseId)
        {
            var viewModel = new CreateSectionViewModel
            {
                CourseId = courseId
            };
            return View(viewModel);
        }

        // POST: Teacher/CreateSection
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSection(CreateSectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            try
            {
                var request = new CreateSectionRequest
                {
                    Title = model.Title,
                    OrderIndex = model.OrderIndex
                };

                await _teacherService.CreateSectionAsync(model.CourseId, request, teacherId);

                TempData["SuccessMessage"] = "Tạo chương mới thành công!";
                return RedirectToAction(nameof(ManageSections), new { id = model.CourseId });
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Teacher/EditSection/{id}
        public async Task<IActionResult> EditSection(int id)
        {
            var section = await _teacherService.GetSectionByIdAsync(id);
            if (section == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chương.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EditSectionViewModel
            {
                SectionId = section.SectionId,
                CourseId = section.CourseId,
                Title = section.Title,
                OrderIndex = section.OrderIndex
            };

            return View(viewModel);
        }

        // POST: Teacher/EditSection/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSection(int id, EditSectionViewModel model)
        {
            if (id != model.SectionId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var request = new UpdateSectionRequest
            {
                SectionId = model.SectionId,
                Title = model.Title,
                OrderIndex = model.OrderIndex
            };

            var result = await _teacherService.UpdateSectionAsync(id, request, teacherId);

            if (result)
            {
                TempData["SuccessMessage"] = "Cập nhật chương thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật chương.";
            }

            return RedirectToAction(nameof(ManageSections), new { id = model.CourseId });
        }

        // POST: Teacher/DeleteSection/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSection(int id, Guid courseId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var result = await _teacherService.DeleteSectionAsync(id, teacherId);

            if (result)
            {
                TempData["SuccessMessage"] = "Xóa chương thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa chương.";
            }

            return RedirectToAction(nameof(ManageSections), new { id = courseId });
        }

        // GET: Teacher/CreateLesson?sectionId=xxx
        public async Task<IActionResult> CreateLesson(int sectionId)
        {
            var section = await _teacherService.GetSectionByIdAsync(sectionId);
            if (section == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chương.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CreateLessonViewModel
            {
                SectionId = sectionId
            };
            ViewBag.CourseId = section.CourseId;
            return View(viewModel);
        }

        // POST: Teacher/CreateLesson
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLesson(CreateLessonViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            try
            {
                var section = await _teacherService.GetSectionByIdAsync(model.SectionId);
                if (section == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy chương.";
                    return RedirectToAction(nameof(Index));
                }

                var request = new CreateLessonRequest
                {
                    Title = model.Title,
                    LessonType = model.LessonType,
                    Content = model.Content,
                    OrderIndex = model.OrderIndex
                };

                await _teacherService.CreateLessonAsync(model.SectionId, request, teacherId);

                TempData["SuccessMessage"] = "Tạo bài học mới thành công!";
                return RedirectToAction(nameof(ManageSections), new { id = section.CourseId });
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Teacher/EditLesson/{id}
        public async Task<IActionResult> EditLesson(int id)
        {
            var lesson = await _teacherService.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài học.";
                return RedirectToAction(nameof(Index));
            }

            var section = await _teacherService.GetSectionByIdAsync(lesson.SectionId);
            if (section == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chương.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EditLessonViewModel
            {
                LessonId = lesson.LessonId,
                SectionId = lesson.SectionId,
                Title = lesson.Title,
                LessonType = lesson.LessonType,
                Content = lesson.Content,
                OrderIndex = lesson.OrderIndex
            };

            ViewBag.CourseId = section.CourseId;
            return View(viewModel);
        }

        // POST: Teacher/EditLesson/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(int id, EditLessonViewModel model)
        {
            if (id != model.LessonId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var section = await _teacherService.GetSectionByIdAsync(model.SectionId);
            if (section == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chương.";
                return RedirectToAction(nameof(Index));
            }

            var request = new UpdateLessonRequest
            {
                LessonId = model.LessonId,
                Title = model.Title,
                LessonType = model.LessonType,
                Content = model.Content,
                OrderIndex = model.OrderIndex
            };

            var result = await _teacherService.UpdateLessonAsync(id, request, teacherId);

            if (result)
            {
                TempData["SuccessMessage"] = "Cập nhật bài học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật bài học.";
            }

            return RedirectToAction(nameof(ManageSections), new { id = section.CourseId });
        }

        // POST: Teacher/DeleteLesson/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int id, Guid courseId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var result = await _teacherService.DeleteLessonAsync(id, teacherId);

            if (result)
            {
                TempData["SuccessMessage"] = "Xóa bài học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa bài học.";
            }

            return RedirectToAction(nameof(ManageSections), new { id = courseId });
        }

        // ===== QUẢN LÝ HỌC VIÊN =====

        // GET: Teacher/ViewEnrollments/{id} - Xem danh sách học viên đăng ký khóa học
        public async Task<IActionResult> ViewEnrollments(Guid id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            // Lấy thông tin khóa học
            var course = await _teacherService.GetTeacherCourseByIdAsync(id, teacherId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khóa học.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy danh sách học viên
            var enrollments = await _teacherService.GetCourseEnrollmentsAsync(id, teacherId);

            ViewBag.CourseId = id;
            ViewBag.CourseTitle = course.Title;

            return View(enrollments);
        }

        // GET: Teacher/ViewStudentProgress?courseId={courseId}&studentId={studentId}
        public async Task<IActionResult> ViewStudentProgress(Guid courseId, string studentId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var progress = await _teacherService.GetStudentProgressAsync(courseId, studentId, teacherId);

            if (progress == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin tiến độ.";
                return RedirectToAction(nameof(ViewEnrollments), new { id = courseId });
            }

            return View(progress);
        }

        // ===== QUẢN LÝ QUIZ =====

        // GET: Teacher/CreateQuiz?lessonId=xxx
        public async Task<IActionResult> CreateQuiz(int lessonId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var lesson = await _teacherService.GetLessonByIdAsync(lessonId);
            if (lesson == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài học.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy section để lấy courseId
            var section = await _teacherService.GetSectionByIdAsync(lesson.SectionId);
            if (section == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chương.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra xem đã có quiz chưa
            var hasQuiz = await _teacherService.HasQuizForLessonAsync(lessonId);
            if (hasQuiz)
            {
                TempData["ErrorMessage"] = "Bài học này đã có quiz. Vui lòng xóa quiz cũ trước khi tạo mới.";
                return RedirectToAction(nameof(ManageSections), new { id = section.CourseId });
            }

            ViewBag.LessonId = lessonId;
            ViewBag.LessonTitle = lesson.Title;
            ViewBag.CourseId = section.CourseId;

            return View(new CreateQuizRequest());
        }

        // POST: Teacher/CreateQuiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuiz(int lessonId, CreateQuizRequest request)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                var lesson = await _teacherService.GetLessonByIdAsync(lessonId);
                if (lesson != null)
                {
                    ViewBag.LessonId = lessonId;
                    ViewBag.LessonTitle = lesson.Title;
                    var section = await _teacherService.GetSectionByIdAsync(lesson.SectionId);
                    if (section != null)
                    {
                        ViewBag.CourseId = section.CourseId;
                    }
                }
                return View(request);
            }

            try
            {
                var lesson = await _teacherService.GetLessonByIdAsync(lessonId);
                if (lesson == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bài học.";
                    return RedirectToAction(nameof(Index));
                }

                var section = await _teacherService.GetSectionByIdAsync(lesson.SectionId);
                if (section == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy chương.";
                    return RedirectToAction(nameof(Index));
                }

                await _teacherService.CreateQuizAsync(lessonId, request, teacherId);

                TempData["SuccessMessage"] = "Tạo quiz thành công!";
                return RedirectToAction(nameof(ManageSections), new { id = section.CourseId });
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var lesson = await _teacherService.GetLessonByIdAsync(lessonId);
                if (lesson != null)
                {
                    var section = await _teacherService.GetSectionByIdAsync(lesson.SectionId);
                    if (section != null)
                    {
                        return RedirectToAction(nameof(ManageSections), new { id = section.CourseId });
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo quiz: " + ex.Message;
                var lesson = await _teacherService.GetLessonByIdAsync(lessonId);
                if (lesson != null)
                {
                    ViewBag.LessonId = lessonId;
                    ViewBag.LessonTitle = lesson.Title;
                    var section = await _teacherService.GetSectionByIdAsync(lesson.SectionId);
                    if (section != null)
                    {
                        ViewBag.CourseId = section.CourseId;
                    }
                }
                return View(request);
            }
        }

        // GET: Teacher/ViewQuiz/{id}
        public async Task<IActionResult> ViewQuiz(int id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var quiz = await _teacherService.GetQuizDetailsAsync(id, teacherId);
            if (quiz == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy quiz hoặc bạn không có quyền xem.";
                return RedirectToAction(nameof(Index));
            }

            return View(quiz);
        }

        // GET: Teacher/EditQuiz/{id}
        public async Task<IActionResult> EditQuiz(int id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var quiz = await _teacherService.GetQuizDetailsAsync(id, teacherId);
            if (quiz == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy quiz hoặc bạn không có quyền chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            // Convert QuizDetailDto sang UpdateQuizRequest
            var request = new UpdateQuizRequest
            {
                Title = quiz.Title,
                Questions = quiz.Questions.Select(q => 
                {
                    var correctIndex = q.Answers.FindIndex(a => a.IsCorrect);
                    if (correctIndex < 0 && q.Answers.Count > 0)
                    {
                        // Nếu không tìm thấy đáp án đúng, tìm theo CorrectAnswer string
                        correctIndex = q.Answers.FindIndex(a => a.UserAnswer == q.CorrectAnswer);
                        if (correctIndex < 0) correctIndex = 0; // Default to first answer
                    }
                    return new CreateQuizQuestionRequest
                    {
                        Content = q.Content,
                        Answers = q.Answers.Select(a => new CreateQuizAnswerRequest
                        {
                            UserAnswer = a.UserAnswer
                        }).ToList(),
                        CorrectAnswerIndex = correctIndex >= 0 ? correctIndex : 0
                    };
                }).ToList()
            };

            ViewBag.QuizId = id;
            ViewBag.LessonTitle = quiz.LessonTitle;
            ViewBag.CourseId = quiz.CourseId;
            return View(request);
        }

        // POST: Teacher/EditQuiz/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuiz(int id, [FromForm] UpdateQuizRequest request)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            // Đảm bảo Questions không null
            request.Questions ??= new List<CreateQuizQuestionRequest>();

            // Debug: Log request data
            System.Diagnostics.Debug.WriteLine($"EditQuiz POST - QuizId: {id}, Title: {request.Title}, Questions Count: {request.Questions.Count}");
            foreach (var q in request.Questions)
            {
                System.Diagnostics.Debug.WriteLine($"  Question: {q.Content}, Answers: {q.Answers?.Count ?? 0}, CorrectIndex: {q.CorrectAnswerIndex}");
            }

            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state?.Errors.Count > 0)
                    {
                        foreach (var error in state.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Validation Error - {key}: {error.ErrorMessage}");
                        }
                    }
                }

                var quiz = await _teacherService.GetQuizDetailsAsync(id, teacherId);
                if (quiz != null)
                {
                    ViewBag.QuizId = id;
                    ViewBag.LessonTitle = quiz.LessonTitle;
                    ViewBag.CourseId = quiz.CourseId;
                }
                return View(request);
            }

            try
            {
                var result = await _teacherService.UpdateQuizAsync(id, request, teacherId);
                if (result)
                {
                    var quiz = await _teacherService.GetQuizDetailsAsync(id, teacherId);
                    TempData["SuccessMessage"] = "Cập nhật quiz thành công!";
                    return RedirectToAction(nameof(ManageSections), new { id = quiz?.CourseId });
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật quiz. Vui lòng thử lại.";
                    var quiz = await _teacherService.GetQuizDetailsAsync(id, teacherId);
                    if (quiz != null)
                    {
                        ViewBag.QuizId = id;
                        ViewBag.LessonTitle = quiz.LessonTitle;
                        ViewBag.CourseId = quiz.CourseId;
                    }
                    return View(request);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật quiz: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"EditQuiz Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                var quiz = await _teacherService.GetQuizDetailsAsync(id, teacherId);
                if (quiz != null)
                {
                    ViewBag.QuizId = id;
                    ViewBag.LessonTitle = quiz.LessonTitle;
                    ViewBag.CourseId = quiz.CourseId;
                }
                return View(request);
            }
        }

        // POST: Teacher/DeleteQuiz/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuiz(int id, Guid courseId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId))
            {
                return Unauthorized();
            }

            var result = await _teacherService.DeleteQuizAsync(id, teacherId);
            if (result)
            {
                TempData["SuccessMessage"] = "Xóa quiz thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa quiz.";
            }

            return RedirectToAction(nameof(ManageSections), new { id = courseId });
        }
    }
}
