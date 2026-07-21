using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Geekspace.Models;

namespace Geekspace.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<LearningResource> LearningResources { get; set; }
    public DbSet<ResourceComment> ResourceComments { get; set; }
}
