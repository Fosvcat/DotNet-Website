using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Identity's cookie auth re-validates a signed-in user's security stamp
// against the database on a timer (default: every 30 minutes) — if it
// doesn't match (or the account no longer exists), the user is signed
// out automatically. Banning/deleting an account bumps the security
// stamp (see UserManagementController), so shortening this interval is
// what makes a ban take effect on the banned user's browser quickly
// instead of up to half an hour later.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromSeconds(30);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Custom route so a resource's read-only view is reached at
// /Resource/{id} instead of /Resource/Details/{id}. This route only
// matches a bare numeric segment after "Resource" (e.g. /Resource/5),
// so /Resource/Edit/3, /Resource/Delete/3, and /Resource/Create are
// untouched and continue to fall through to the default route below.
app.MapControllerRoute(
    name: "resourceDetails",
    pattern: "Resource/{id:int}",
    defaults: new { controller = "Resource", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

   // Seed the Root and Admin roles and promote the designated accounts.
   using (var scope = app.Services.CreateScope())
   {
       await SeedRolesAsync(scope.ServiceProvider);

       var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
       await SeedData.InitializeAsync(dbContext);
   }
   app.Run();

   // Local function: creates the Root and Admin roles if missing, and
   // promotes the specified email addresses on every application startup.
   // Safe to run repeatedly — it only adds a role if the account doesn't
   // already have it.
   static async Task SeedRolesAsync(IServiceProvider services)
   {
       var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
       var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

       foreach (var role in new[] { "Root", "Admin" })
       {
           if (!await roleManager.RoleExistsAsync(role))
           {
               await roleManager.CreateAsync(new IdentityRole(role));
           }
       }

       await PromoteIfExistsAsync(userManager, "root@fosvcat.com", "Root");
       await PromoteIfExistsAsync(userManager, "admin@fosvcat.com", "Admin");
   }

   static async Task PromoteIfExistsAsync(UserManager<IdentityUser> userManager, string email, string role)
   {
       var user = await userManager.FindByEmailAsync(email);
       if (user != null && !await userManager.IsInRoleAsync(user, role))
       {
           await userManager.AddToRoleAsync(user, role);
       }
   }
