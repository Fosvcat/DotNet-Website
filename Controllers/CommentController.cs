using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int learningResourceId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["CommentError"] = "Comment cannot be empty.";
                return RedirectToAction("Details", "LearningResource", new { id = learningResourceId });
            }

            var userId = _userManager.GetUserId(User);

            var comment = new ResourceComment
            {
                LearningResourceId = learningResourceId,
                UserId = userId!,
                Content = content,
                PostedDate = DateTime.Now
            };

            _context.ResourceComments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "LearningResource", new { id = learningResourceId });
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
            bool isAdmin = User.IsInRole("Admin");

            if (!isOwner && !isAdmin)
            {
                return Forbid();
            }

            int resourceId = comment.LearningResourceId;
            _context.ResourceComments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "LearningResource", new { id = resourceId });
        }
    }
}
