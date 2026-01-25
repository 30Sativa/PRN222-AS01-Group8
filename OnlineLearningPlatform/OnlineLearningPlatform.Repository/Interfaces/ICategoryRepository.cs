using OnlineLearningPlatform.Models.Entities;

namespace OnlineLearningPlatform.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<Category> CreateAsync(Category category);
        Task<bool> UpdateAsync(Category category);
        /// <summary>Xóa danh mục. Trả về false nếu danh mục đang được khóa học sử dụng.</summary>
        Task<bool> DeleteAsync(int categoryId);
        Task<int> GetCourseCountByCategoryIdAsync(int categoryId);
    }
}
