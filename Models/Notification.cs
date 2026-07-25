using System.ComponentModel.DataAnnotations;

namespace Geekspace.Models
{
    public enum NotificationType
    {
        Reply,
        Like,
        Dislike
    }

    // A notification sent to a user when someone replies to, likes, or
    // dislikes one of their comments.
    //
    // CommentId's meaning depends on Type:
    //  - Reply: the NEW reply comment that was posted (so "View" jumps
    //    straight to what the other user said).
    //  - Like / Dislike: the recipient's own comment that received the
    //    vote (so "View" jumps to their own comment).
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string RecipientUserId { get; set; } = string.Empty;

        [Required]
        public string ActorUserId { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public int CommentId { get; set; }
        public ResourceComment? Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
    }
}
