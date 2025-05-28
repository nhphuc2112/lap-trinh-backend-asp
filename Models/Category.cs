using System.ComponentModel.DataAnnotations;

namespace TodoList.API.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public string Color { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // thuoc tinh chuyen doi
        public virtual ICollection<TaskCategory> TaskCategories { get; set; }
    }
} 