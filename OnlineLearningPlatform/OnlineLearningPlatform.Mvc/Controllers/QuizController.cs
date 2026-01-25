using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

[Authorize]
public class QuizController : Controller
{
    private readonly IQuizService _quizService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public QuizController(IQuizService quizService, IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _quizService = quizService; _unitOfWork = unitOfWork; _context = context;
    }

    // --- PHẦN GIÁO VIÊN ---
    [HttpGet]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Create()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.CourseList = await _context.Courses.Where(c => c.TeacherId == userId).ToListAsync();
        return View();
    }

    [HttpGet]
    public async Task<JsonResult> GetSectionsByCourse(Guid courseId) =>
        Json(await _context.Sections.Where(s => s.CourseId == courseId).Select(s => new { s.SectionId, s.Title }).ToListAsync());

    [HttpGet]
    public async Task<JsonResult> GetLessonsBySection(int sectionId) =>
        Json(await _context.Lessons.Where(l => l.SectionId == sectionId).Select(l => new { l.LessonId, l.Title }).ToListAsync());

    [HttpPost]
    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Create(Quiz quiz, IFormCollection form)
    {
        try
        {
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var question = quiz.Questions.ElementAt(i);
                question.QuestionType = "mcq";
                if (int.TryParse(form[$"correct_{i}"], out int correctIdx))
                {
                    var answers = question.QuizAnswers.ToList();
                    if (correctIdx < answers.Count)
                    {
                        question.CorrectAnswer = answers.ElementAt(correctIdx).UserAnswer;
                    }
                }
                foreach (var ans in question.QuizAnswers)
                {
                    ans.AnswerId = Guid.NewGuid();
                    ans.AttemptId = null;
                }
            }

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return RedirectToAction("Menu", "Instructor");
        }
        catch (Exception ex)
        {
            return View(quiz);
        }
    }

    // --- PHẦN HỌC SINH ---
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> TakeByLesson(int lessonId)
    {
        var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.LessonId == lessonId);
        if (quiz == null) return RedirectToAction("Index", "Student");
        return RedirectToAction("Take", new { id = quiz.QuizId });
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Take(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!await _quizService.CanUserAttemptAsync(userId, id)) return Content("Bạn đã hết 3 lượt làm bài!");
        var quiz = await _context.Quizzes.Include(q => q.Questions).ThenInclude(qs => qs.QuizAnswers).FirstOrDefaultAsync(q => q.QuizId == id);
        return View(quiz);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Submit(int quizId, IFormCollection form)
    {
        var ans = new Dictionary<int, string>();
        foreach (var key in form.Keys.Where(k => k.StartsWith("q_")))
            ans.Add(int.Parse(key.Replace("q_", "")), form[key]);
        var res = await _quizService.SubmitQuizAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), quizId, ans);
        return RedirectToAction("Result", new { id = res.AttemptId });
    }

    public async Task<IActionResult> Result(Guid id) =>
        View(await _context.QuizAttempts.Include(a => a.Quiz).FirstOrDefaultAsync(a => a.AttemptId == id));

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var history = await _context.QuizAttempts
            .Include(a => a.Quiz) 
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AttemptedAt)
            .ToListAsync();

        return View(history);
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> AttemptDetail(Guid id)
    {
        var attempt = await _context.QuizAttempts
            .Include(a => a.Quiz)
            .ThenInclude(q => q.Questions)
            .FirstOrDefaultAsync(a => a.AttemptId == id);

        ViewBag.UserAnswers = await _context.QuizAnswers
            .Where(ans => ans.AttemptId == id)
            .ToListAsync();

        return View(attempt);
    }
}