using OnlineLearningPlatform.Models.Entities;
using OnlineLearningPlatform.Models.Enums;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.DTO.Request;
using OnlineLearningPlatform.Services.DTO.Response;
using OnlineLearningPlatform.Services.DTO.Response.Teacher;
using OnlineLearningPlatform.Services.DTO.Teacher;
using OnlineLearningPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OnlineLearningPlatform.Services.Implements
{
    /// <summary>
    /// Service implementation cho Teacher - Tách riêng để tránh conflict với team
    /// </summary>
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILessonProgressRepository _lessonProgressRepository;

        public TeacherService(
            ITeacherRepository teacherRepository,
            ICategoryRepository categoryRepository,
            IEnrollmentRepository enrollmentRepository,
            ILessonProgressRepository lessonProgressRepository)
        {
            _teacherRepository = teacherRepository;
            _categoryRepository = categoryRepository;
            _enrollmentRepository = enrollmentRepository;
            _lessonProgressRepository = lessonProgressRepository;
        }

        public async Task<List<TeacherCourseDto>> GetTeacherCoursesAsync(string teacherId, string? keyword = null)
        {
            var courses = await _teacherRepository.GetCoursesByTeacherIdAndStatusAsync(teacherId, CourseStatus.Published);
            
            // Lọc theo keyword nếu có
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchTerm = keyword.Trim().ToLower();
                courses = courses.Where(c => 
                    c.Title.ToLower().Contains(searchTerm) || 
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm))
                ).ToList();
            }
            
            return MapCoursesToDtos(courses);
        }

        public async Task<List<TeacherCourseDto>> GetTeacherPendingCoursesAsync(string teacherId)
        {
            var courses = await _teacherRepository.GetCoursesByTeacherIdAndStatusAsync(teacherId, CourseStatus.Pending);
            return MapCoursesToDtos(courses);
        }

        public async Task<List<TeacherCourseDto>> GetTeacherRejectedCoursesAsync(string teacherId)
        {
            var courses = await _teacherRepository.GetCoursesByTeacherIdAndStatusAsync(teacherId, CourseStatus.Rejected);
            return MapCoursesToDtos(courses);
        }

        private static List<TeacherCourseDto> MapCoursesToDtos(List<Course> courses)
        {
            return courses.Select(course => new TeacherCourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description ?? string.Empty,
                CategoryName = course.Category?.CategoryName,
                Price = course.Price,
                CreatedAt = course.CreatedAt,
                Status = course.Status,
                RejectionReason = course.RejectionReason,
                TotalEnrollments = course.Enrollments?.Count ?? 0,
                TotalSections = course.Sections?.Count ?? 0,
                TotalLessons = course.Sections?.Sum(s => s.Lessons?.Count ?? 0) ?? 0
            }).ToList();
        }

        public async Task<TeacherCourseDto?> GetTeacherCourseByIdAsync(Guid courseId, string teacherId)
        {
            var course = await _teacherRepository.GetCourseWithStatisticsAsync(courseId, teacherId);

            if (course == null)
                return null;

            return new TeacherCourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description ?? string.Empty,
                CategoryName = course.Category?.CategoryName,
                Price = course.Price,
                CreatedAt = course.CreatedAt,
                Status = course.Status,
                RejectionReason = course.RejectionReason,
                TotalEnrollments = course.Enrollments?.Count ?? 0,
                TotalSections = course.Sections?.Count ?? 0,
                TotalLessons = course.Sections?.Sum(s => s.Lessons?.Count ?? 0) ?? 0
            };
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
        }

        // ===== QUẢN LÝ DANH MỤC KHÓA HỌC =====
        public async Task<List<CategoryDto>> GetCategoriesForManagementAsync()
        {
            return await GetCategoriesAsync();
        }

        public async Task<int> CreateCategoryAsync(string categoryName)
        {
            var category = new Category
            {
                CategoryName = categoryName.Trim()
            };
            var created = await _categoryRepository.CreateAsync(category);
            return created.CategoryId;
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, string categoryName)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
                return false;
            category.CategoryName = categoryName.Trim();
            return await _categoryRepository.UpdateAsync(category);
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
                return (false, "Không tìm thấy danh mục.");
            var courseCount = await _categoryRepository.GetCourseCountByCategoryIdAsync(categoryId);
            if (courseCount > 0)
                return (false, $"Không thể xóa danh mục \"{category.CategoryName}\" vì đang có {courseCount} khóa học sử dụng.");
            var ok = await _categoryRepository.DeleteAsync(categoryId);
            return ok ? (true, "Xóa danh mục thành công.") : (false, "Không thể xóa danh mục.");
        }

        public async Task<Guid> CreateCourseAsync(CreateCourseRequest request, string teacherId)
        {
            var course = new Course
            {
                CourseId = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                TeacherId = teacherId,
                CreatedAt = DateTime.UtcNow
            };

            var createdCourse = await _teacherRepository.CreateCourseAsync(course);
            return createdCourse.CourseId;
        }

        public async Task<bool> UpdateCourseAsync(Guid courseId, UpdateCourseRequest request, string teacherId)
        {
            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(courseId, teacherId);
            if (!isOwner)
                return false;

            var course = await _teacherRepository.GetCourseWithStatisticsAsync(courseId, teacherId);
            if (course == null)
                return false;

            // Cập nhật thông tin
            course.Title = request.Title;
            course.Description = request.Description;
            course.Price = request.Price;
            course.CategoryId = request.CategoryId;

            return await _teacherRepository.UpdateCourseAsync(course);
        }

        public async Task<bool> DeleteCourseAsync(Guid courseId, string teacherId)
        {
            return await _teacherRepository.DeleteCourseAsync(courseId, teacherId);
        }

        // ===== QUẢN LÝ SECTIONS =====
        public async Task<List<TeacherSectionDto>> GetCourseSectionsAsync(Guid courseId, string teacherId)
        {
            var sections = await _teacherRepository.GetCourseSectionsAsync(courseId, teacherId);
            var sectionDtos = new List<TeacherSectionDto>();

            foreach (var section in sections)
            {
                var sectionDto = new TeacherSectionDto
                {
                    SectionId = section.SectionId,
                    CourseId = section.CourseId,
                    Title = section.Title,
                    OrderIndex = section.OrderIndex,
                    TotalLessons = section.Lessons?.Count ?? 0
                };

                if (section.Lessons != null)
                {
                    sectionDto.Lessons = section.Lessons.Select(l => new TeacherLessonDto
                    {
                        LessonId = l.LessonId,
                        SectionId = l.SectionId,
                        Title = l.Title,
                        LessonType = l.LessonType,
                        Content = l.Content,
                        OrderIndex = l.OrderIndex
                    }).ToList();
                }

                sectionDtos.Add(sectionDto);
            }

            return sectionDtos;
        }

        public async Task<TeacherSectionDto?> GetSectionByIdAsync(int sectionId)
        {
            var section = await _teacherRepository.GetSectionByIdAsync(sectionId);
            if (section == null)
                return null;

            var sectionDto = new TeacherSectionDto
            {
                SectionId = section.SectionId,
                CourseId = section.CourseId,
                Title = section.Title,
                OrderIndex = section.OrderIndex,
                TotalLessons = section.Lessons?.Count ?? 0
            };

            if (section.Lessons != null)
            {
                sectionDto.Lessons = section.Lessons.Select(l => new TeacherLessonDto
                {
                    LessonId = l.LessonId,
                    SectionId = l.SectionId,
                    Title = l.Title,
                    LessonType = l.LessonType,
                    Content = l.Content,
                    OrderIndex = l.OrderIndex
                }).ToList();
            }

            return sectionDto;
        }

        public async Task<int> CreateSectionAsync(Guid courseId, CreateSectionRequest request, string teacherId)
        {
            // Kiểm tra quyền sở hữu khóa học
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(courseId, teacherId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Bạn không có quyền thêm chương vào khóa học này");

            var section = new Section
            {
                CourseId = courseId,
                Title = request.Title,
                OrderIndex = request.OrderIndex
            };

            var createdSection = await _teacherRepository.CreateSectionAsync(section);
            return createdSection.SectionId;
        }

        public async Task<bool> UpdateSectionAsync(int sectionId, UpdateSectionRequest request, string teacherId)
        {
            var section = await _teacherRepository.GetSectionByIdAsync(sectionId);
            if (section == null)
                return false;

            // Kiểm tra quyền sở hữu khóa học
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(section.CourseId, teacherId);
            if (!isOwner)
                return false;

            section.Title = request.Title;
            section.OrderIndex = request.OrderIndex;

            return await _teacherRepository.UpdateSectionAsync(section);
        }

        public async Task<bool> DeleteSectionAsync(int sectionId, string teacherId)
        {
            var section = await _teacherRepository.GetSectionByIdAsync(sectionId);
            if (section == null)
                return false;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(section.CourseId, teacherId);
            if (!isOwner)
                return false;

            return await _teacherRepository.DeleteSectionAsync(sectionId);
        }

        // ===== QUẢN LÝ LESSONS =====
        public async Task<TeacherLessonDto?> GetLessonByIdAsync(int lessonId)
        {
            var lesson = await _teacherRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null)
                return null;

            return new TeacherLessonDto
            {
                LessonId = lesson.LessonId,
                SectionId = lesson.SectionId,
                Title = lesson.Title,
                LessonType = lesson.LessonType,
                Content = lesson.Content,
                OrderIndex = lesson.OrderIndex
            };
        }

        public async Task<int> CreateLessonAsync(int sectionId, CreateLessonRequest request, string teacherId)
        {
            var section = await _teacherRepository.GetSectionByIdAsync(sectionId);
            if (section == null)
                throw new ArgumentException("Không tìm thấy chương");

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(section.CourseId, teacherId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Bạn không có quyền thêm bài học vào khóa học này");

            var lesson = new Lesson
            {
                SectionId = sectionId,
                Title = request.Title,
                LessonType = request.LessonType,
                Content = request.Content,
                OrderIndex = request.OrderIndex
            };

            var createdLesson = await _teacherRepository.CreateLessonAsync(lesson);
            return createdLesson.LessonId;
        }

        public async Task<bool> UpdateLessonAsync(int lessonId, UpdateLessonRequest request, string teacherId)
        {
            var lesson = await _teacherRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null || lesson.Section == null)
                return false;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(lesson.Section.CourseId, teacherId);
            if (!isOwner)
                return false;

            lesson.Title = request.Title;
            lesson.LessonType = request.LessonType;
            lesson.Content = request.Content;
            lesson.OrderIndex = request.OrderIndex;

            return await _teacherRepository.UpdateLessonAsync(lesson);
        }

        public async Task<bool> DeleteLessonAsync(int lessonId, string teacherId)
        {
            var lesson = await _teacherRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null || lesson.Section == null)
                return false;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(lesson.Section.CourseId, teacherId);
            if (!isOwner)
                return false;

            return await _teacherRepository.DeleteLessonAsync(lessonId);
        }

        // ===== QUẢN LÝ HỌC VIÊN =====
        
        public async Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, string teacherId)
        {
            // Kiểm tra quyền sở hữu khóa học
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(courseId, teacherId);
            if (!isOwner)
                return new List<EnrollmentDto>();

            var enrollments = await _enrollmentRepository.GetCourseEnrollmentsAsync(courseId);

            return enrollments.Select(e => new EnrollmentDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.UserId,
                StudentName = e.User?.FullName ?? "Unknown",
                StudentEmail = e.User?.Email ?? "Unknown",
                EnrolledAt = e.EnrolledAt
            }).ToList();
        }

        public async Task<StudentProgressDto?> GetStudentProgressAsync(Guid courseId, string studentId, string teacherId)
        {
            // Kiểm tra quyền sở hữu khóa học
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(courseId, teacherId);
            if (!isOwner)
                return null;

            // Lấy enrollment
            var enrollment = await _enrollmentRepository.GetEnrollmentByStudentAndCourseAsync(studentId, courseId);
            if (enrollment == null || enrollment.Course == null || enrollment.User == null)
                return null;

            // Lấy tiến độ bài học
            var lessonProgresses = await _lessonProgressRepository.GetStudentProgressInCourseAsync(studentId, courseId);

            // Tính tổng số lessons, quizzes, assignments
            var allLessons = enrollment.Course.Sections?
                .SelectMany(s => s.Lessons ?? new List<Lesson>())
                .ToList() ?? new List<Lesson>();

            var totalLessons = allLessons.Count;
            var totalQuizzes = allLessons.Count(l => l.LessonType?.ToLower() == "quiz");
            var totalAssignments = allLessons.Count(l => l.LessonType?.ToLower() == "assignment");

            // Đếm số đã hoàn thành
            var completedLessonIds = lessonProgresses.Where(lp => lp.IsCompleted).Select(lp => lp.LessonId).ToList();
            var completedLessons = completedLessonIds.Count;
            var completedQuizzes = allLessons.Count(l => l.LessonType?.ToLower() == "quiz" && completedLessonIds.Contains(l.LessonId));
            var completedAssignments = allLessons.Count(l => l.LessonType?.ToLower() == "assignment" && completedLessonIds.Contains(l.LessonId));

            // Tạo progress theo từng section
            var sectionProgressList = new List<SectionProgressDto>();

            if (enrollment.Course.Sections != null)
            {
                foreach (var section in enrollment.Course.Sections.OrderBy(s => s.OrderIndex))
                {
                    var sectionLessons = section.Lessons?.ToList() ?? new List<Lesson>();
                    var sectionTotalLessons = sectionLessons.Count;
                    var sectionCompletedLessons = sectionLessons.Count(l => completedLessonIds.Contains(l.LessonId));

                    var lessonProgressList = sectionLessons
                        .OrderBy(l => l.OrderIndex)
                        .Select(lesson =>
                        {
                            var lessonProgress = lessonProgresses.FirstOrDefault(lp => lp.LessonId == lesson.LessonId);
                            return new LessonProgressDto
                            {
                                LessonId = lesson.LessonId,
                                LessonTitle = lesson.Title,
                                OrderIndex = lesson.OrderIndex ?? 0,
                                LessonType = lesson.LessonType ?? "text",
                                IsCompleted = lessonProgress?.IsCompleted ?? false,
                                CompletedAt = lessonProgress?.CompletedAt,
                                ProgressPercentage = (lessonProgress?.IsCompleted ?? false) ? 100 : 0
                            };
                        }).ToList();

                    sectionProgressList.Add(new SectionProgressDto
                    {
                        SectionId = section.SectionId,
                        SectionTitle = section.Title,
                        OrderIndex = section.OrderIndex ?? 0,
                        TotalLessons = sectionTotalLessons,
                        CompletedLessons = sectionCompletedLessons,
                        Progress = sectionTotalLessons > 0 ? (decimal)sectionCompletedLessons / sectionTotalLessons * 100 : 0,
                        LessonProgress = lessonProgressList
                    });
                }
            }

            // Tính phần trăm tổng thể
            var overallProgress = totalLessons > 0 ? (decimal)completedLessons / totalLessons * 100 : 0;

            return new StudentProgressDto
            {
                StudentId = enrollment.UserId,
                StudentName = enrollment.User.FullName,
                StudentEmail = enrollment.User.Email ?? "Unknown",
                CourseId = enrollment.CourseId,
                CourseTitle = enrollment.Course.Title,
                EnrolledAt = enrollment.EnrolledAt,
                OverallProgress = overallProgress,
                TotalLessons = totalLessons,
                CompletedLessons = completedLessons,
                TotalQuizzes = totalQuizzes,
                CompletedQuizzes = completedQuizzes,
                TotalAssignments = totalAssignments,
                CompletedAssignments = completedAssignments,
                SectionProgress = sectionProgressList
            };
        }

        // ===== QUẢN LÝ QUIZ =====
        public async Task<bool> HasQuizForLessonAsync(int lessonId)
        {
            var quiz = await _teacherRepository.GetQuizByLessonIdAsync(lessonId);
            return quiz != null;
        }

        public async Task<Dictionary<int, (int QuizId, string Title)>> GetQuizzesByLessonIdsAsync(List<int> lessonIds)
        {
            var quizzes = await _teacherRepository.GetQuizzesByLessonIdsAsync(lessonIds);
            return quizzes.ToDictionary(
                q => q.LessonId,
                q => (q.QuizId, q.Title ?? string.Empty)
            );
        }

        public async Task<int> CreateQuizAsync(int lessonId, CreateQuizRequest request, string teacherId)
        {
            // Kiểm tra lesson tồn tại và quyền sở hữu
            var lesson = await _teacherRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null || lesson.Section == null)
                throw new ArgumentException("Không tìm thấy bài học");

            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(lesson.Section.CourseId, teacherId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Bạn không có quyền tạo quiz cho bài học này");

            // Kiểm tra xem đã có quiz chưa
            var existingQuiz = await _teacherRepository.GetQuizByLessonIdAsync(lessonId);
            if (existingQuiz != null)
                throw new InvalidOperationException("Bài học này đã có quiz. Vui lòng xóa quiz cũ trước khi tạo mới.");

            // Tạo quiz entity
            var quiz = new Quiz
            {
                LessonId = lessonId,
                Title = request.Title,
                Questions = new List<Question>()
            };

            // Tạo questions và answers
            foreach (var questionRequest in request.Questions)
            {
                var question = new Question
                {
                    Content = questionRequest.Content,
                    QuestionType = "mcq",
                    QuizAnswers = new List<QuizAnswer>()
                };

                // Tạo answers
                for (int i = 0; i < questionRequest.Answers.Count; i++)
                {
                    var answerRequest = questionRequest.Answers[i];
                    var answer = new QuizAnswer
                    {
                        AnswerId = Guid.NewGuid(),
                        UserAnswer = answerRequest.UserAnswer,
                        AttemptId = null
                    };
                    question.QuizAnswers.Add(answer);

                    // Set đáp án đúng
                    if (i == questionRequest.CorrectAnswerIndex)
                    {
                        question.CorrectAnswer = answerRequest.UserAnswer;
                    }
                }

                quiz.Questions.Add(question);
            }

            var createdQuiz = await _teacherRepository.CreateQuizAsync(quiz);
            return createdQuiz.QuizId;
        }

        public async Task<QuizDetailDto?> GetQuizDetailsAsync(int quizId, string teacherId)
        {
            var quiz = await _teacherRepository.GetQuizWithDetailsAsync(quizId);
            if (quiz == null || quiz.Lesson == null || quiz.Lesson.Section == null)
                return null;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(quiz.Lesson.Section.CourseId, teacherId);
            if (!isOwner)
                return null;

            var questions = new List<QuestionDetailDto>();
            if (quiz.Questions != null)
            {
                foreach (var question in quiz.Questions)
                {
                    var answers = new List<AnswerDetailDto>();
                    if (question.QuizAnswers != null)
                    {
                        foreach (var answer in question.QuizAnswers.Where(a => a.AttemptId == null))
                        {
                            answers.Add(new AnswerDetailDto
                            {
                                AnswerId = answer.AnswerId,
                                UserAnswer = answer.UserAnswer ?? string.Empty,
                                IsCorrect = answer.UserAnswer == question.CorrectAnswer
                            });
                        }
                    }

                    questions.Add(new QuestionDetailDto
                    {
                        QuestionId = question.QuestionId,
                        Content = question.Content ?? string.Empty,
                        QuestionType = question.QuestionType ?? "mcq",
                        CorrectAnswer = question.CorrectAnswer ?? string.Empty,
                        Answers = answers
                    });
                }
            }

            return new QuizDetailDto
            {
                QuizId = quiz.QuizId,
                LessonId = quiz.LessonId,
                LessonTitle = quiz.Lesson.Title ?? string.Empty,
                CourseId = quiz.Lesson.Section.CourseId,
                CourseTitle = quiz.Lesson.Section.Course?.Title ?? string.Empty,
                Title = quiz.Title ?? string.Empty,
                Questions = questions
            };
        }

        public async Task<bool> UpdateQuizAsync(int quizId, UpdateQuizRequest request, string teacherId)
        {
            var quiz = await _teacherRepository.GetQuizWithDetailsAsync(quizId);
            if (quiz == null || quiz.Lesson == null || quiz.Lesson.Section == null)
                return false;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(quiz.Lesson.Section.CourseId, teacherId);
            if (!isOwner)
                return false;

            // Cập nhật title
            quiz.Title = request.Title;

            // Thu thập danh sách questions và template answers cần xóa
            var questionsToDelete = quiz.Questions?.ToList() ?? new List<Question>();
            var templateAnswersToDelete = questionsToDelete
                .SelectMany(q => q.QuizAnswers?.Where(a => a.AttemptId == null) ?? new List<QuizAnswer>())
                .ToList();

            // Tạo questions và answers mới
            var newQuestions = new List<Question>();
            foreach (var questionRequest in request.Questions)
            {
                var question = new Question
                {
                    QuizId = quizId,
                    Content = questionRequest.Content,
                    QuestionType = "mcq",
                    QuizAnswers = new List<QuizAnswer>()
                };

                // Tạo answers
                for (int i = 0; i < questionRequest.Answers.Count; i++)
                {
                    var answerRequest = questionRequest.Answers[i];
                    var answer = new QuizAnswer
                    {
                        AnswerId = Guid.NewGuid(),
                        UserAnswer = answerRequest.UserAnswer,
                        AttemptId = null
                    };
                    question.QuizAnswers.Add(answer);

                    // Set đáp án đúng
                    if (i == questionRequest.CorrectAnswerIndex)
                    {
                        question.CorrectAnswer = answerRequest.UserAnswer;
                    }
                }

                newQuestions.Add(question);
            }

            return await _teacherRepository.UpdateQuizWithQuestionsAsync(quiz, questionsToDelete, templateAnswersToDelete, newQuestions);
        }

        public async Task<bool> DeleteQuizAsync(int quizId, string teacherId)
        {
            var quiz = await _teacherRepository.GetQuizWithDetailsAsync(quizId);
            if (quiz == null || quiz.Lesson == null || quiz.Lesson.Section == null)
                return false;

            // Kiểm tra quyền sở hữu
            var isOwner = await _teacherRepository.IsTeacherOwnsCourseAsync(quiz.Lesson.Section.CourseId, teacherId);
            if (!isOwner)
                return false;

            return await _teacherRepository.DeleteQuizAsync(quizId);
        }
    }
}
