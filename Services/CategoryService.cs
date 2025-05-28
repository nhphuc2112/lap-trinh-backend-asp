using TodoList.API.Models;
using TodoList.API.DTOs;
using TodoList.API.Repositories;

namespace TodoList.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            return MapToDto(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                Color = createCategoryDto.Color
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);
            return MapToDto(createdCategory);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            if (!string.IsNullOrEmpty(updateCategoryDto.Name))
            {
                category.Name = updateCategoryDto.Name;
            }

            if (updateCategoryDto.Description != null)
            {
                category.Description = updateCategoryDto.Description;
            }

            if (updateCategoryDto.Color != null)
            {
                category.Color = updateCategoryDto.Color;
            }

            category.UpdatedAt = DateTime.UtcNow;
            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            return MapToDto(updatedCategory);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CategoryDto>> GetByTaskIdAsync(int taskId)
        {
            var categories = await _categoryRepository.GetByTaskIdAsync(taskId);
            return categories.Select(MapToDto);
        }

        private static CategoryDto MapToDto(Category category)
        {
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Color = category.Color,
                CreatedAt = category.CreatedAt
            };
        }
    }
} 