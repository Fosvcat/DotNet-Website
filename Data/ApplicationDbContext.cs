using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Geekspace.Models;

namespace Geekspace.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<LearningResource> LearningResources { get; set; }
    public DbSet<ResourceComment> ResourceComments { get; set; }
    public DbSet<CommentVote> CommentVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Explicit configuration for the self-referencing "reply to a
        // comment" relationship. Leaving this to convention is a known
        // source of EF Core's migration model-differ flip-flopping on
        // self-referencing FKs. Deleting a parent comment detaches its
        // replies (ParentCommentId set to null) rather than deleting
        // them, so a reply's own content is never silently lost.
        builder.Entity<ResourceComment>()
            .HasOne(c => c.ParentComment)
            .WithMany()
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.SetNull);

        // LearningResourceId is nullable so a comment can be a Forum post
        // (LearningResourceId == null) instead of belonging to a specific
        // resource. That nullability alone would make EF Core default to
        // "detach on delete" — this explicit Cascade keeps the original
        // behavior for resource comments: deleting a resource deletes its
        // comments, it never turns them into orphaned Forum posts.
        builder.Entity<ResourceComment>()
            .HasOne(c => c.LearningResource)
            .WithMany(r => r.Comments)
            .HasForeignKey(c => c.LearningResourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
