using System.ComponentModel.DataAnnotations;

namespace TodoList.API.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Color { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [StringLength(200)]
        public required string Description { get; set; }

        [StringLength(50)]
        public required string Color { get; set; }
    }

    public class UpdateCategoryDto
    {
        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }
    }
} 