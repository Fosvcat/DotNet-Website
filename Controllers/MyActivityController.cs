using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;

namespace Geekspace.Controllers
{
    // Lets any authenticated member review and manage the comments
    // they have posted across the site. Open to every logged-in role
    // (User, Admin, Root) — this is personal activity, not administration.
    [Authorize]
    public class MyActivityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyActivityController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MyActivity
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            var myComments = await _context.ResourceComments
            .Include(c => c.LearningResource)
            .Where(c => c.UserId == currentUserId)
            .OrderByDescending(c => c.PostedDate)
            .ToListAsync();

            return View(myComments);
        }
    }
}
