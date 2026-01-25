using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Models.Identity;
using OnlineLearningPlatform.Repositories.Implements;
using OnlineLearningPlatform.Repositories.Interfaces;
using OnlineLearningPlatform.Services.Implements;
using OnlineLearningPlatform.Services.Interfaces;

namespace OnlineLearningPlatform.Mvc
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Cấu hình DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

            //Cấu hình Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Cấu hình xác thực Google
            builder.Services.AddAuthentication().AddGoogle(op =>
            {
                op.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                op.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });
            // Cấu hình thời gian sống cookie xác thực
            builder.Services.ConfigureExternalCookie(op =>
            {
                op.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });
            // Đăng ký các dịch vụ tùy chỉnh - Repository
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<ILessonRepository, LessonRepository>();
            builder.Services.AddScoped<ITeacherRepository, TeacherRepository>(); // Teacher Repository
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Category Repository
            builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>(); // Lesson Progress Repository
            builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>(); // Payment Repository
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>(); // Review Repository

            // Đăng ký các dịch vụ tùy chỉnh - Service
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
            builder.Services.AddScoped<ILessonService, LessonService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IStatisticsService, StatisticsService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>(); // Payment Service
            builder.Services.AddScoped<IReviewService, ReviewService>(); // Review Service
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            // Area route (Admin, Manager, ...)
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

            // Default route (Home page - Landing page)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            // Seed admin user khi khởi động ứng dụng
            await ApplicationDbContext.SeedAdminUserAsync(app.Services);

            app.Run();
        }
    }
}
