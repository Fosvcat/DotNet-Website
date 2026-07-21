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
    [Authorize(Roles = "Admin,Root")]
    public class LearningResourceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LearningResourceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: LearningResource
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LearningResources.Include(l => l.Category);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: LearningResource/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: LearningResource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

        // GET: LearningResource/Edit/5
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

        // POST: LearningResource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

        // GET: LearningResource/Details/5
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

            return View(learningResource);
        }

        // GET: LearningResource/Delete/5
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

        // POST: LearningResource/Delete/5
        [HttpPost, ActionName("Delete")]
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
