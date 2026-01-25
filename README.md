# Online Learning Platform (PRN222-AS01-Group8)

Nền tảng học trực tuyến xây dựng theo mô hình nhiều tầng (MVC + Services + Repository + Models), hỗ trợ phân quyền theo vai trò và các nghiệp vụ học tập cơ bản như quản lý khóa học/bài học/quiz, đăng ký học, theo dõi tiến độ và đánh giá khóa học.

## Tech stack

- **.NET**: ASP.NET Core MVC `net8.0`
- **UI**: Razor Views (`.cshtml`) + static assets trong `wwwroot/`
- **Database**: SQL Server + Entity Framework Core
- **Auth**: ASP.NET Core Identity + Google OAuth (External Login)
- **Architecture**: Repository + Service layer + DTO

## Tính năng chính

### Theo vai trò (Roles)

- **Admin**
  - Dashboard thống kê (Admin area)
  - Quản lý người dùng: tạo, chỉnh sửa, khóa/mở khóa, đổi role
  - Duyệt khóa học: approve/reject
- **Instructor** (UI/Controller đặt tên là `Teacher`)
  - Tạo/sửa/xóa khóa học
  - Quản lý danh mục (categories)
  - Quản lý chương (sections) & bài học (lessons)
  - Tạo/sửa/xóa quiz theo bài học
  - Xem danh sách học viên đăng ký & tiến độ học
  - Xem đánh giá khóa học
- **Student**
  - Xem danh sách khóa học + tìm kiếm
  - Đăng ký khóa học (miễn phí hoặc trả phí)
  - Checkout/Thanh toán (demo)
  - Xem bài học & đánh dấu hoàn thành
  - Đánh giá khóa học (create/update/delete) + xem danh sách review

### Đăng nhập Google

- Có hỗ trợ **Google OAuth**: đăng nhập lần đầu sẽ tự tạo user và gán role **Student**.

## Cấu trúc dự án

Solution nằm trong thư mục `OnlineLearningPlatform/` (file `OnlineLearningPlatform.slnx`) gồm:

- `OnlineLearningPlatform.Mvc`: ASP.NET Core MVC (Controllers/Views/wwwroot, `Program.cs`)
- `OnlineLearningPlatform.Services`: Business logic + DTO (Request/Response)
- `OnlineLearningPlatform.Repository`: Data access layer (repositories + UnitOfWork)
- `OnlineLearningPlatform.Models`: Entities + `ApplicationDbContext` + Identity models

## Cấu trúc thư mục dự án (theo tầng)

Thứ tự: **Models** → **Repository** → **Services** → **Mvc** (Presentation).

```
PRN222-AS01-Group8/
├── .gitignore
├── README.md
└── OnlineLearningPlatform/
    ├── OnlineLearningPlatform.slnx
    │
    ├── OnlineLearningPlatform.Models/          # Tầng Domain / Entity
    │   ├── Entities/
    │   ├── Enums/
    │   ├── Identity/
    │   └── Migrations/
    │
    ├── OnlineLearningPlatform.Repository/      # Tầng truy cập dữ liệu
    │   ├── Interfaces/
    │   └── Implements/
    │
    ├── OnlineLearningPlatform.Services/        # Tầng nghiệp vụ
    │   ├── DTO/
    │   │   ├── Request/
    │   │   └── Response/
    │   ├── Interfaces/
    │   └── Implements/
    │
    └── OnlineLearningPlatform.Mvc/             # Tầng giao diện (Presentation)
        ├── Areas/
        │   └── Admin/
        ├── Controllers/
        ├── Models/
        ├── Views/
        ├── wwwroot/
        │   ├── css/
        │   ├── js/
        │   └── lib/
        └── Properties/
```

## Yêu cầu môi trường

- **.NET SDK 8.x**
- **SQL Server** (SQL Server/Express/LocalDB đều được)
- (Khuyến nghị) **Visual Studio 2022** hoặc `dotnet` CLI

## Cấu hình (Configuration)

### Database connection string

File: `OnlineLearningPlatform/OnlineLearningPlatform.Mvc/appsettings.json`

Mặc định đang dùng:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(local);Database=OnlineLearningPlatformDb;User ID=sa;Password=1234567890;MultipleActiveResultSets=true;TrustServerCertificate=True;"
}
```

Lưu ý:
- **Không nên dùng mật khẩu/connection string thật trong production**.
- Nếu bạn dùng **LocalDB**, có thể đổi thành (ví dụ):

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OnlineLearningPlatformDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
```

### Google OAuth

File: `OnlineLearningPlatform/OnlineLearningPlatform.Mvc/appsettings.json`

```json
"Authentication": {
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  }
}
```

## Khởi chạy dự án

### Chạy bằng .NET CLI

Từ thư mục repo:

```bash
dotnet restore "OnlineLearningPlatform/OnlineLearningPlatform.Mvc/OnlineLearningPlatform.Mvc.csproj"
dotnet run --project "OnlineLearningPlatform/OnlineLearningPlatform.Mvc/OnlineLearningPlatform.Mvc.csproj"
```

Mặc định (theo `launchSettings.json`) app chạy tại:
- `https://localhost:7137`
- `http://localhost:5059`

### Chạy bằng Visual Studio

- Mở `OnlineLearningPlatform/OnlineLearningPlatform.slnx`
- Set startup project: `OnlineLearningPlatform.Mvc`
- Run (https)

## Database setup (EF Core)

Repo hiện có thư mục `OnlineLearningPlatform.Models/Migrations`, nhưng nếu chưa có migration file nào thì bạn cần tạo migration lần đầu.

### Cài dotnet-ef (nếu máy chưa có)

```bash
dotnet tool install --global dotnet-ef
```

### Tạo migration (chỉ khi chưa có)

```bash
dotnet ef migrations add InitialCreate \
  --project "OnlineLearningPlatform/OnlineLearningPlatform.Models/OnlineLearningPlatform.Models.csproj" \
  --startup-project "OnlineLearningPlatform/OnlineLearningPlatform.Mvc/OnlineLearningPlatform.Mvc.csproj"
```

### Apply migrations tạo DB

```bash
dotnet ef database update \
  --project "OnlineLearningPlatform/OnlineLearningPlatform.Models/OnlineLearningPlatform.Models.csproj" \
  --startup-project "OnlineLearningPlatform/OnlineLearningPlatform.Mvc/OnlineLearningPlatform.Mvc.csproj"
```

## Tài khoản mặc định (seed)

Khi ứng dụng khởi động, hệ thống sẽ seed role và tài khoản admin trong `ApplicationDbContext.SeedAdminUserAsync()`:

- **Email**: `admin@gmail.com`
- **Password**: `Admin123@`

## Ghi chú

- **Thanh toán**: luồng thanh toán đang ở chế độ **demo** (phục vụ bài tập/assignment).
- Nếu gặp lỗi kết nối DB, hãy kiểm tra:
  - SQL Server instance đang chạy
  - Connection string đúng instance/user/password
  - Quyền truy cập DB

## Đóng góp

- Tạo branch theo tính năng: `feature/<name>`
- Ưu tiên thêm/chỉnh sửa logic trong `Services` và `Repository`, hạn chế nhồi nghiệp vụ vào `Controllers`
- Giữ DTO rõ ràng cho request/response trong `OnlineLearningPlatform.Services/DTO`