using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;
using Geekspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Geekspace.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ResourceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Resource
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LearningResources.Include(l => l.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Resource/Search?q=...
        // Reuses the Index view to render results, flagged via
        // ViewData["IsSearchView"] so the view can swap its heading and
        // hide the "Create New" button.
        [AllowAnonymous]
        public async Task<IActionResult> Search(string q)
        {
            var query = _context.LearningResources.Include(l => l.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(r =>
                    r.Title.Contains(term) ||
                    r.Description.Contains(term) ||
                    (r.Content != null && r.Content.Contains(term)));
            }

            var results = await query.ToListAsync();

            ViewData["IsSearchView"] = true;
            ViewData["SearchTerm"] = q;

            return View("Index", results);
        }


        // GET: Resource/Create
        [Authorize(Roles = "Admin,Root")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Resource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin,Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Content,Type,MediaUrl,CategoryId,CreatedDate,IsPublished")] LearningResource learningResource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learningResource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", learningResource.CategoryId);
            return View(learningResource);
        }

        // GET: Resource/Edit/5
        [Authorize(Roles = "Admin,Root")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningResource = await _context.LearningResources.FindAsync(id);
            if (learningResource == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", learningResource.CategoryId);
            return View(learningResource);
        }

        // POST: Resource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin,Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Content,Type,MediaUrl,CategoryId,CreatedDate,IsPublished")] LearningResource learningResource)
        {
            if (id != learningResource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learningResource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningResourceExists(learningResource.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", learningResource.CategoryId);
            return View(learningResource);
        }

        // GET: Resource/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningResource = await _context.LearningResources
            .Include(l => l.Category)
            .Include(l => l.Comments)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (learningResource == null)
            {
                return NotFound();
            }

            // Build a lookup of user id -> display name so the view can show
            // who posted each comment without an extra navigation property.
            var userIds = learningResource.Comments.Select(c => c.UserId).Distinct().ToList();
            var authorNames = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "Unknown");
            ViewBag.CommentAuthors = authorNames;

            // Build a set of user IDs that belong to Root accounts, so the view
            // can hide the Delete button from Admins looking at a Root's comment
            // (the server-side check in CommentController is the real enforcement;
            // this just avoids showing a button that would be rejected anyway).
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

            // Vote counts (visible to everyone, including anonymous users).
            var commentIds = learningResource.Comments.Select(c => c.Id).ToList();

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

            // The current user's own vote per comment (true = liked,
            // false = disliked, absent = no vote), used to render the
            // filled vs. outline icon state. Only computed when signed in.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var currentUserId = _userManager.GetUserId(User);
                var myVotes = await _context.CommentVotes
                    .Where(v => commentIds.Contains(v.ResourceCommentId) && v.UserId == currentUserId)
                    .ToDictionaryAsync(v => v.ResourceCommentId, v => v.IsLike);
                ViewBag.MyVotes = myVotes;
            }

            return View(learningResource);
        }

        // GET: Resource/Delete/5
        [Authorize(Roles = "Admin,Root")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningResource = await _context.LearningResources
            .Include(l => l.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (learningResource == null)
            {
                return NotFound();
            }

            return View(learningResource);
        }

        // POST: Resource/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var learningResource = await _context.LearningResources.FindAsync(id);
            if (learningResource != null)
            {
                _context.LearningResources.Remove(learningResource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearningResourceExists(int id)
        {
            return _context.LearningResources.Any(e => e.Id == id);
        }
    }
}
