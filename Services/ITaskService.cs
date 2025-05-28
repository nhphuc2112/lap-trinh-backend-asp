using TodoList.API.Models;
using TodoList.API.DTOs;

namespace TodoList.API.Services
{
    public interface ITaskService
    {
        Task<TaskDto> GetByIdAsync(int id);
        Task<IEnumerable<TaskDto>> GetAllAsync();
        Task<(IEnumerable<TaskDto> Tasks, int TotalCount)> GetFilteredAsync(TaskFilterDto filter);
        Task<TaskDto> CreateAsync(CreateTaskDto createTaskDto, int userId);
        Task<TaskDto> UpdateAsync(int id, UpdateTaskDto updateTaskDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddCategoryAsync(int taskId, int categoryId);
        Task<bool> RemoveCategoryAsync(int taskId, int categoryId);
    }
} 