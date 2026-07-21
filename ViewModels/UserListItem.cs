namespace Geekspace.ViewModels
{
    // A lightweight projection of a registered account, used only for
    // rendering the admin user-management screen. Kept separate from
    // the Identity model so the view never binds directly to IdentityUser.
    public class UserListItem
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // "Root", "Admin", or "User"
        public string Role { get; set; } = "User";
    }
}
