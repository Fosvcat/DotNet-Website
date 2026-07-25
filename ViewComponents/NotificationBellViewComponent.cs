using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;

namespace Geekspace.ViewComponents
{
    // Renders the notification bell in the navbar, with an unread-count
    // badge. Embedded once in _Layout.cshtml so every page gets it
    // without each controller needing to populate its own ViewBag data.
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationBellViewComponent(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return Content(string.Empty);
            }

            var userId = _userManager.GetUserId(user);
            var unreadCount = await _context.Notifications
                .CountAsync(n => n.RecipientUserId == userId && !n.IsRead);

            return View(unreadCount);
        }
    }
}
