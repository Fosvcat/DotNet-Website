using System.ComponentModel.DataAnnotations;

namespace Geekspace.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category names cannot be empty")]
        [StringLength(50, ErrorMessage = "Category names cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public ICollection<LearningResource> Resources { get; set; } = new List<LearningResource>();
    }
}
