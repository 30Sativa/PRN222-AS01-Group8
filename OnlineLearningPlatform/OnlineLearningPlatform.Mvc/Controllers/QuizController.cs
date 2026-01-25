using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Services.Interfaces;
using System.Security.Claims;

[Authorize]
public class QuizController : Controller
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    // --- PHẦN GIÁO VIÊN ---
    [HttpGet]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Create()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.CourseList = await _quizService.GetCoursesByTeacherIdAsync(userId);
        return View();
    }

    [HttpGet]
    public async Task<JsonResult> GetSectionsByCourse(Guid courseId) =>
        Json(await _quizService.GetSectionsByCourseIdAsync(courseId));

    [HttpGet]
    public async Task<JsonResult> GetLessonsBySection(int sectionId) =>
        Json(await _quizService.GetLessonsBySectionIdAsync(sectionId));

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

            await _quizService.CreateQuizAsync(quiz);

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
        var quiz = await _quizService.GetQuizByLessonIdAsync(lessonId);
        if (quiz == null) return RedirectToAction("Index", "Student");
        return RedirectToAction("Take", new { id = quiz.QuizId });
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Take(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!await _quizService.CanUserAttemptAsync(userId, id)) return Content("Bạn đã hết 3 lượt làm bài!");
        var quiz = await _quizService.GetQuizWithQuestionsAsync(id);
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
        View(await _quizService.GetQuizAttemptByIdAsync(id));

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var history = await _quizService.GetQuizAttemptHistoryByUserIdAsync(userId);
        return View(history);
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> AttemptDetail(Guid id)
    {
        var attempt = await _quizService.GetQuizAttemptWithDetailsAsync(id);
        ViewBag.UserAnswers = await _quizService.GetQuizAnswersByAttemptIdAsync(id);
        return View(attempt);
    }
}