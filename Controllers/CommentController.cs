using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;
using Geekspace.Models;

namespace Geekspace.Controllers
{
    // Handles the discussion feature attached to each learning resource.
    // Any authenticated (registered) member may post a comment;
    // only the comment's author or an Admin may delete it.
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Comment/Create
        // learningResourceId is nullable: present = a comment on that
        // resource's discussion section, absent = a standalone Forum post.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? learningResourceId, string content, int? parentCommentId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["CommentError"] = "Comment cannot be empty.";
                return learningResourceId.HasValue
                    ? RedirectToAction("Details", "Resource", new { id = learningResourceId })
                    : RedirectToAction("Index", "Forum");
            }

            if (content.Length > 1000)
            {
                TempData["CommentError"] = "Comment cannot exceed 1000 characters.";
                return learningResourceId.HasValue
                    ? RedirectToAction("Details", "Resource", new { id = learningResourceId })
                    : RedirectToAction("Index", "Forum");
            }

            var userId = _userManager.GetUserId(User);

            var comment = new ResourceComment
            {
                LearningResourceId = learningResourceId,
                UserId = userId!,
                Content = content,
                PostedDate = DateTime.Now,
                ParentCommentId = parentCommentId
            };

            _context.ResourceComments.Add(comment);
            await _context.SaveChangesAsync();

            // If this is a reply, notify the parent comment's author
            // (unless they replied to their own comment).
            if (parentCommentId.HasValue)
            {
                var parentComment = await _context.ResourceComments.FindAsync(parentCommentId.Value);
                if (parentComment != null && parentComment.UserId != userId)
                {
                    _context.Notifications.Add(new Notification
                    {
                        RecipientUserId = parentComment.UserId,
                        ActorUserId = userId!,
                        Type = NotificationType.Reply,
                        CommentId = comment.Id,
                        CreatedDate = DateTime.Now,
                        IsRead = false
                    });
                    await _context.SaveChangesAsync();
                }
            }

            return learningResourceId.HasValue
                ? RedirectToAction("Details", "Resource", new { id = learningResourceId })
                : RedirectToAction("Index", "Forum");
        }

        // POST: Comment/Vote
        // Called via fetch() from site.js so voting never triggers a full
        // page reload. [Authorize] on the class still means an
        // unauthenticated POST here gets redirected to the login page by
        // the Identity middleware; the client-side JS detects that case
        // (a non-JSON response) and navigates there manually.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int commentId, string voteType)
        {
            var comment = await _context.ResourceComments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            bool isLike = voteType == "like";
            var userId = _userManager.GetUserId(User)!;

            var existingVote = await _context.CommentVotes
                .FirstOrDefaultAsync(v => v.ResourceCommentId == commentId && v.UserId == userId);

            bool? myVoteAfter;
            var thisType = isLike ? NotificationType.Like : NotificationType.Dislike;
            var otherType = isLike ? NotificationType.Dislike : NotificationType.Like;

            if (existingVote == null)
            {
                // No vote yet — record this one, and notify the comment's
                // author (unless they're voting on their own comment).
                _context.CommentVotes.Add(new CommentVote
                {
                    ResourceCommentId = commentId,
                    UserId = userId,
                    IsLike = isLike
                });
                myVoteAfter = isLike;

                await AddVoteNotificationAsync(comment, userId, thisType);
            }
            else if (existingVote.IsLike == isLike)
            {
                // Clicking the same button again removes the vote entirely,
                // and the notification that was created for it.
                _context.CommentVotes.Remove(existingVote);
                myVoteAfter = null;

                await RemoveVoteNotificationAsync(commentId, userId, thisType);
            }
            else
            {
                // Switching from like to dislike (or vice versa) — the
                // previous vote is replaced, never both at once. The old
                // notification is removed and a new one takes its place.
                existingVote.IsLike = isLike;
                myVoteAfter = isLike;

                await RemoveVoteNotificationAsync(commentId, userId, otherType);
                await AddVoteNotificationAsync(comment, userId, thisType);
            }

            await _context.SaveChangesAsync();

            var likeCount = await _context.CommentVotes.CountAsync(v => v.ResourceCommentId == commentId && v.IsLike);
            var dislikeCount = await _context.CommentVotes.CountAsync(v => v.ResourceCommentId == commentId && !v.IsLike);

            return Json(new { likeCount, dislikeCount, myVote = myVoteAfter });
        }

        // Creates a Like/Dislike notification for the comment's author,
        // skipping self-votes (voting on your own comment never notifies
        // you). Does not save — the caller's SaveChangesAsync covers it.
        private async Task AddVoteNotificationAsync(ResourceComment comment, string actorUserId, NotificationType type)
        {
            if (comment.UserId == actorUserId)
            {
                return;
            }

            _context.Notifications.Add(new Notification
            {
                RecipientUserId = comment.UserId,
                ActorUserId = actorUserId,
                Type = type,
                CommentId = comment.Id,
                CreatedDate = DateTime.Now,
                IsRead = false
            });

            await Task.CompletedTask;
        }

        // Removes a previously created Like/Dislike notification for this
        // exact (comment, actor, type) combination, if one exists.
        private async Task RemoveVoteNotificationAsync(int commentId, string actorUserId, NotificationType type)
        {
            var existing = await _context.Notifications
                .Where(n => n.CommentId == commentId && n.ActorUserId == actorUserId && n.Type == type)
                .ToListAsync();

            if (existing.Count > 0)
            {
                _context.Notifications.RemoveRange(existing);
            }
        }

        // POST: Comment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.ResourceComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            bool isOwner = comment.UserId == currentUserId;
            bool isCurrentRoot = User.IsInRole("Root");
            bool isCurrentAdmin = User.IsInRole("Admin");

            bool allowed;

            if (isOwner)
            {
                // Anyone may delete their own comment, regardless of role.
                allowed = true;
            }
            else if (isCurrentRoot)
            {
                // Root may delete any comment from anyone, including other Root or Admin accounts.
                allowed = true;
            }
            else if (isCurrentAdmin)
            {
                // Admin may delete anyone's comment EXCEPT a comment posted by a Root account.
                var commentAuthor = await _userManager.FindByIdAsync(comment.UserId);
                bool authorIsRoot = commentAuthor != null && await _userManager.IsInRoleAsync(commentAuthor, "Root");
                allowed = !authorIsRoot;
            }
            else
            {
                // Plain users may only delete their own comments (handled by isOwner above).
                allowed = false;
            }

            if (!allowed)
            {
                return Forbid();
            }

            int? resourceId = comment.LearningResourceId;
            _context.ResourceComments.Remove(comment);
            await _context.SaveChangesAsync();

            return resourceId.HasValue
                ? RedirectToAction("Details", "Resource", new { id = resourceId })
                : RedirectToAction("Index", "Forum");
        }
    }
}
