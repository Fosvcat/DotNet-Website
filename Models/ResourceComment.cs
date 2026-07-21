using System.ComponentModel.DataAnnotations;

namespace Geekspace.Models
{
    public class ResourceComment
    {
        public int Id { get; set; }

        public int LearningResourceId { get; set; }
        public LearningResource? LearningResource { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Comment cannot be empty")]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime PostedDate { get; set; } = DateTime.Now;
    }
}
