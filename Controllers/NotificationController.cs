using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;

namespace Geekspace.Controllers
{
    // [Authorize] with no per-action overrides: any anonymous request
    // here is automatically redirected to the login page by Identity's
    // middleware, satisfying "not logged in -> go to login".
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /notification
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var notifications = await _context.Notifications
                .Include(n => n.Comment)
                    .ThenInclude(c => c!.LearningResource)
                .Where(n => n.RecipientUserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            var actorIds = notifications.Select(n => n.ActorUserId).Distinct().ToList();
            var actorNames = await _context.Users
                .Where(u => actorIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "Unknown");
            ViewBag.ActorNames = actorNames;

            return View(notifications);
        }

        // POST: Notification/MarkRead/5
        // Called via fetch() so it never triggers a page reload.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = _userManager.GetUserId(User);
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.RecipientUserId == userId);

            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: Notification/MarkAllRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = _userManager.GetUserId(User);
            var unread = await _context.Notifications
                .Where(n => n.RecipientUserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Notification/DeleteAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            var userId = _userManager.GetUserId(User);
            var all = await _context.Notifications
                .Where(n => n.RecipientUserId == userId)
                .ToListAsync();

            _context.Notifications.RemoveRange(all);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Notification/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.RecipientUserId == userId);

            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
