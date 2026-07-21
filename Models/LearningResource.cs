using System.ComponentModel.DataAnnotations;

namespace Geekspace.Models
{
    public enum ResourceType
    {
        Article,
        Video,
        VirtualLab,
        Simulation,
        SelfAssessment
    }

    public class LearningResource
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title cannot be empty")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description cannot be empty.")]
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Content")]
        [DataType(DataType.MultilineText)]
        [StringLength(10000, ErrorMessage = "Content cannot exceed 10,000 characters.")]
        public string? Content { get; set; }

        [Display(Name = "Resource Type")]
        public ResourceType Type { get; set; }

        [Display(Name = "Media File Path")]
        [StringLength(300)]
        public string? MediaUrl { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Published or not")]
        public bool IsPublished { get; set; } = true;

        public ICollection<ResourceComment> Comments { get; set; } = new List<ResourceComment>();
    }
}
