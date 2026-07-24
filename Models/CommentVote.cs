using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Geekspace.Models
{
    // Records a single user's like or dislike on a comment. The unique
    // index on (ResourceCommentId, UserId) enforces "one vote per user
    // per comment" at the database level, not just in application code,
    // so duplicate votes are impossible even under concurrent requests.
    [Index(nameof(ResourceCommentId), nameof(UserId), IsUnique = true)]
    public class CommentVote
    {
        public int Id { get; set; }

        public int ResourceCommentId { get; set; }
        public ResourceComment? ResourceComment { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // true = like (thumbs up), false = dislike (thumbs down)
        public bool IsLike { get; set; }
    }
}
