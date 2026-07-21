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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

   // Seed the Admin role and promote a designated account to Admin
   using (var scope = app.Services.CreateScope())
   {
       await SeedAdminRoleAsync(scope.ServiceProvider);
   }

   app.Run();

   // Local function: creates the Admin role if missing, and promotes
   // the specified email address to Admin on every application startup.
   static async Task SeedAdminRoleAsync(IServiceProvider services)
   {
       var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
       var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

       const string adminRole = "Admin";
       if (!await roleManager.RoleExistsAsync(adminRole))
       {
           await roleManager.CreateAsync(new IdentityRole(adminRole));
       }

       const string adminEmail = "admin@fosvcat.com";
       var adminUser = await userManager.FindByEmailAsync(adminEmail);
       if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, adminRole))
       {
           await userManager.AddToRoleAsync(adminUser, adminRole);
       }
   }
