using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList.API.Models
{
    public class TaskCategory
    {
        public int TaskId { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        // thuoc tinh chuyen doi
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
} 