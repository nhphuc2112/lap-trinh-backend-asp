using TodoList.API.Models;
using TodoList.API.DTOs;

namespace TodoList.API.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CategoryDto>> GetByTaskIdAsync(int taskId);
    }
} 