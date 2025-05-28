using TodoList.API.Models;
using TodoList.API.DTOs;

namespace TodoList.API.Repositories
{
    //khai bao task
    public interface ITaskRepository
    {
        Task<Models.Task> GetByIdAsync(int id);
        Task<IEnumerable<Models.Task>> GetAllAsync();
        Task<(IEnumerable<Models.Task> Tasks, int TotalCount)> GetFilteredAsync(TaskFilterDto filter);
        Task<Models.Task> CreateAsync(Models.Task task);
        Task<Models.Task> UpdateAsync(Models.Task task);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddCategoryAsync(int taskId, int categoryId);
        Task<bool> RemoveCategoryAsync(int taskId, int categoryId);
        Task<IEnumerable<Category>> GetTaskCategoriesAsync(int taskId);
    }
} 