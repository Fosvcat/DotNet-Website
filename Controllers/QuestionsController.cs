using System.Collections.Generic;
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
    // The quiz-only "Questions" section. [Authorize] with no per-action
    // overrides means any anonymous request — including just viewing the
    // list — is redirected to the login page automatically by Identity's
    // middleware, matching "only logged-in users can access".
    [Authorize]
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public QuestionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Questions
        public IActionResult Index()
        {
            return View(QuestionsCatalog.All);
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var quiz = QuestionsCatalog.Get(id);
            if (quiz == null)
            {
                return NotFound();
            }

            var comments = await _context.ResourceComments
                .Where(c => c.QuestionId == id)
                .Include(c => c.ParentComment)
                .OrderByDescending(c => c.PostedDate)
                .ToListAsync();

            // Same ViewBag population pattern as ResourceController.Details
            // and ForumController.Index, consumed by the shared _CommentList
            // partial.
            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
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

            var commentIds = comments.Select(c => c.Id).ToList();
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

            var currentUserId = _userManager.GetUserId(User);
            var myVotes = await _context.CommentVotes
                .Where(v => commentIds.Contains(v.ResourceCommentId) && v.UserId == currentUserId)
                .ToDictionaryAsync(v => v.ResourceCommentId, v => v.IsLike);
            ViewBag.MyVotes = myVotes;

            ViewBag.Comments = comments;

            return View(quiz);
        }
    }
}
