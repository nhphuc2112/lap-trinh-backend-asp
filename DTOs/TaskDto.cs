using System.ComponentModel.DataAnnotations;
using TodoList.API.Models;

namespace TodoList.API.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public Models.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required List<CategoryDto> Categories { get; set; }
    }

    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public required string Title { get; set; }

        [StringLength(1000)]
        public required string Description { get; set; }

        [Required]
        public Models.TaskStatus Status { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        public DateTime? DueDate { get; set; }
        public required List<int> CategoryIds { get; set; }
    }

    public class UpdateTaskDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public Models.TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public List<int>? CategoryIds { get; set; }
    }

    public class TaskFilterDto
    {
        public string? SearchTerm { get; set; }
        public Models.TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
} 