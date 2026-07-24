using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;

namespace Geekspace.Controllers
{
    // A general discussion board, reusing the same comment/reply/vote
    // system used on resource pages. Posts here are ResourceComment rows
    // with LearningResourceId == null (see ResourceComment.cs), so all
    // the existing Comment/Vote/Delete/Reply logic works unchanged —
    // this controller only needs to load and display them.
    [AllowAnonymous]
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ForumController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Forum
        public async Task<IActionResult> Index()
        {
            var posts = await _context.ResourceComments
                .Where(c => c.LearningResourceId == null)
                .Include(c => c.ParentComment)
                .OrderByDescending(c => c.PostedDate)
                .ToListAsync();

            // Same ViewBag population pattern as ResourceController.Details,
            // consumed by the shared _CommentList partial.
            var userIds = posts.Select(c => c.UserId).Distinct().ToList();
            var authorNames = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "Unknown");
            ViewBag.CommentAuthors = authorNames;

            var rootUserIds = new HashSet<string>();
            foreach (var uid in userIds)
            {
                var u = await _userManager.FindByIdAsync(uid);
                if (u != null && await _userManager.IsInRoleAsync(u, "Root"))
                {
                    rootUserIds.Add(uid);
                }
            }
            ViewBag.RootUserIds = rootUserIds;

            var commentIds = posts.Select(c => c.Id).ToList();
            var voteCounts = await _context.CommentVotes
                .Where(v => commentIds.Contains(v.ResourceCommentId))
                .GroupBy(v => v.ResourceCommentId)
                .Select(g => new
                {
                    CommentId = g.Key,
                    Likes = g.Count(v => v.IsLike),
                    Dislikes = g.Count(v => !v.IsLike)
                })
                .ToListAsync();

            ViewBag.LikeCounts = voteCounts.ToDictionary(v => v.CommentId, v => v.Likes);
            ViewBag.DislikeCounts = voteCounts.ToDictionary(v => v.CommentId, v => v.Dislikes);

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var currentUserId = _userManager.GetUserId(User);
                var myVotes = await _context.CommentVotes
                    .Where(v => commentIds.Contains(v.ResourceCommentId) && v.UserId == currentUserId)
                    .ToDictionaryAsync(v => v.ResourceCommentId, v => v.IsLike);
                ViewBag.MyVotes = myVotes;
            }

            return View(posts);
        }
    }
}
