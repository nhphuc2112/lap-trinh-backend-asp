using TodoList.API.Models;
using TodoList.API.DTOs;
using TodoList.API.Repositories;

namespace TodoList.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TaskService(ITaskRepository taskRepository, ICategoryRepository categoryRepository)
        {
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<TaskDto> GetByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            return await MapToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetAllAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            var taskDtos = new List<TaskDto>();

            foreach (var task in tasks)
            {
                taskDtos.Add(await MapToDto(task));
            }

            return taskDtos;
        }

        public async Task<(IEnumerable<TaskDto> Tasks, int TotalCount)> GetFilteredAsync(TaskFilterDto filter)
        {
            var (tasks, totalCount) = await _taskRepository.GetFilteredAsync(filter);
            var taskDtos = new List<TaskDto>();

            foreach (var task in tasks)
            {
                taskDtos.Add(await MapToDto(task));
            }

            return (taskDtos, totalCount);
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto createTaskDto, int userId)
        {
            var task = new Models.Task
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = createTaskDto.Status,
                Priority = createTaskDto.Priority,
                DueDate = createTaskDto.DueDate,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            if (createTaskDto.CategoryIds != null && createTaskDto.CategoryIds.Any())
            {
                task.TaskCategories = createTaskDto.CategoryIds.Select(categoryId => new TaskCategory
                {
                    CategoryId = categoryId,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            var createdTask = await _taskRepository.CreateAsync(task);
            return await MapToDto(createdTask);
        }

        public async Task<TaskDto> UpdateAsync(int id, UpdateTaskDto updateTaskDto)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            if (!string.IsNullOrEmpty(updateTaskDto.Title))
            {
                task.Title = updateTaskDto.Title;
            }

            if (updateTaskDto.Description != null)
            {
                task.Description = updateTaskDto.Description;
            }

            if (updateTaskDto.Status.HasValue)
            {
                task.Status = updateTaskDto.Status.Value;
            }

            if (updateTaskDto.Priority.HasValue)
            {
                task.Priority = updateTaskDto.Priority.Value;
            }

            if (updateTaskDto.DueDate.HasValue)
            {
                task.DueDate = updateTaskDto.DueDate;
            }

            if (updateTaskDto.CategoryIds != null)
            {
                task.TaskCategories = updateTaskDto.CategoryIds.Select(categoryId => new TaskCategory
                {
                    CategoryId = categoryId,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            task.UpdatedAt = DateTime.UtcNow;
            var updatedTask = await _taskRepository.UpdateAsync(task);
            return await MapToDto(updatedTask);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _taskRepository.DeleteAsync(id);
        }

        public async Task<bool> AddCategoryAsync(int taskId, int categoryId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            return await _taskRepository.AddCategoryAsync(taskId, categoryId);
        }

        public async Task<bool> RemoveCategoryAsync(int taskId, int categoryId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            return await _taskRepository.RemoveCategoryAsync(taskId, categoryId);
        }

        private async Task<TaskDto> MapToDto(Models.Task task)
        {
            if (task == null) return null;

            var categories = await _categoryRepository.GetByTaskIdAsync(task.Id);

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UserId = task.UserId,
                Username = task.User?.Username,
                Categories = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Color = c.Color,
                    CreatedAt = c.CreatedAt
                }).ToList()
            };
        }
    }
} 