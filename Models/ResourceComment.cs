using System.ComponentModel.DataAnnotations;

namespace Geekspace.Models
{
    public class ResourceComment
    {
        public int Id { get; set; }

        // Null = a Forum post, not attached to any specific resource.
        // Set = a comment on that resource's discussion section.
        public int? LearningResourceId { get; set; }
        public LearningResource? LearningResource { get; set; }

        // Set = a comment on a /Questions/{id} quiz page's discussion
        // section. Questions are not database-backed (see
        // QuestionsCatalog), so this is a plain integer tag, not a real
        // foreign key — a comment belongs to exactly one of
        // LearningResourceId, QuestionId, or neither (a Forum post).
        public int? QuestionId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Comment cannot be empty")]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime PostedDate { get; set; } = DateTime.Now;

        // Null = a top-level comment. When set, this comment is a reply
        // to another comment on the same resource. Nullable/optional FK,
        // so deleting the parent simply detaches the reply (sets this to
        // null) rather than deleting the reply itself.
        public int? ParentCommentId { get; set; }
        public ResourceComment? ParentComment { get; set; }
    }
}
