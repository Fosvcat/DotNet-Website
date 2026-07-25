using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Geekspace.ViewModels;

namespace Geekspace.Controllers
{
    // Administration screen for managing registered accounts.
    // Visible to both Root and Admin, but the permission boundaries
    // between the two are enforced entirely server-side below —
    // never rely on hiding a button in the view alone.
    [Authorize(Roles = "Root,Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagementController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Returns "Root", "Admin", or "User" for a given account.
        private async Task<string> GetRoleAsync(IdentityUser user)
        {
            if (await _userManager.IsInRoleAsync(user, "Root")) return "Root";
            if (await _userManager.IsInRoleAsync(user, "Admin")) return "Admin";
            return "User";
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            ViewBag.CurrentUserId = _userManager.GetUserId(User);

            var users = _userManager.Users.ToList();
            var list = new List<UserListItem>();

            foreach (var u in users)
            {
                list.Add(new UserListItem
                {
                    Id = u.Id,
                    UserName = u.UserName ?? "(no username)",
                    Email = u.Email ?? "(no email)",
                         Role = await GetRoleAsync(u),
                         IsBanned = await _userManager.IsLockedOutAsync(u)
                });
            }

            // Root always sits at the very top of the table, everyone
            // else sorted alphabetically by email underneath.
            var ordered = list
                .OrderByDescending(u => u.Role == "Root")
                .ThenBy(u => u.Email)
                .ToList();

            return View(ordered);
        }

        // POST: UserManagement/PromoteToAdmin/{id}
        // Both Root and Admin may promote a plain User to Admin.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var target = await _userManager.FindByIdAsync(id);
            if (target == null) return NotFound();

            var targetRole = await GetRoleAsync(target);
            if (targetRole != "User")
            {
                TempData["UserManagementError"] = "Only regular users can be promoted to Admin.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.AddToRoleAsync(target, "Admin");
            return RedirectToAction(nameof(Index));
        }

        // POST: UserManagement/DemoteToUser/{id}
        // Only Root may demote an Admin back to a regular User.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DemoteToUser(string id)
        {
            if (!User.IsInRole("Root"))
            {
                return Forbid();
            }

            var target = await _userManager.FindByIdAsync(id);
            if (target == null) return NotFound();

            var targetRole = await GetRoleAsync(target);
            if (targetRole != "Admin")
            {
                TempData["UserManagementError"] = "Only Admin accounts can be demoted.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.RemoveFromRoleAsync(target, "Admin");
            return RedirectToAction(nameof(Index));
        }

        // Root may ban/unban any Admin or User (never Root, never itself).
        // Admin may ban/unban only plain User accounts (never itself,
        // never other Admins or Root).
        private bool CanBan(string targetRole, bool isSelf)
        {
            if (isSelf || targetRole == "Root")
            {
                return false;
            }

            if (User.IsInRole("Root"))
            {
                return true;
            }

            if (User.IsInRole("Admin"))
            {
                return targetRole == "User";
            }

            return false;
        }

        // POST: UserManagement/Ban/{id}
        // Bans by setting a permanent lockout on the account. Identity's
        // sign-in check already rejects locked-out accounts on the next
        // login attempt; bumping the security stamp also signs the user
        // out of any session they're currently using (see the
        // SecurityStampValidatorOptions configured in Program.cs).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ban(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var target = await _userManager.FindByIdAsync(id);
            if (target == null) return NotFound();

            var targetRole = await GetRoleAsync(target);
            bool isSelf = id == currentUserId;

            if (!CanBan(targetRole, isSelf))
            {
                return Forbid();
            }

            await _userManager.SetLockoutEnabledAsync(target, true);
            await _userManager.SetLockoutEndDateAsync(target, DateTimeOffset.MaxValue);
            await _userManager.UpdateSecurityStampAsync(target);

            return RedirectToAction(nameof(Index));
        }

        // POST: UserManagement/Unban/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unban(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var target = await _userManager.FindByIdAsync(id);
            if (target == null) return NotFound();

            var targetRole = await GetRoleAsync(target);
            bool isSelf = id == currentUserId;

            if (!CanBan(targetRole, isSelf))
            {
                return Forbid();
            }

            await _userManager.SetLockoutEndDateAsync(target, null);

            return RedirectToAction(nameof(Index));
        }

        // POST: UserManagement/Delete/{id}
        // Root may delete Admin or User accounts (but never Root, never itself).
        // Admin may delete only plain User accounts.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (id == currentUserId)
            {
                TempData["UserManagementError"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var target = await _userManager.FindByIdAsync(id);
            if (target == null) return NotFound();

            var targetRole = await GetRoleAsync(target);

            // Root accounts can never be deleted through this panel.
            if (targetRole == "Root")
            {
                return Forbid();
            }

            bool isCurrentRoot = User.IsInRole("Root");
            bool isCurrentAdmin = User.IsInRole("Admin");

            bool allowed = isCurrentRoot || (isCurrentAdmin && targetRole == "User");
            if (!allowed)
            {
                return Forbid();
            }

            // Reset the ban status before deleting, so a stale permanent
            // lockout can never resurface if this id/email is ever reused.
            await _userManager.SetLockoutEndDateAsync(target, null);
            await _userManager.DeleteAsync(target);
            return RedirectToAction(nameof(Index));
        }
    }
}
