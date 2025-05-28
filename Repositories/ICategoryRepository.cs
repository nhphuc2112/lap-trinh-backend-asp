using TodoList.API.Models;

namespace TodoList.API.Repositories
{
    //khai bao phan loai
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Category>> GetByTaskIdAsync(int taskId);
    }
} 