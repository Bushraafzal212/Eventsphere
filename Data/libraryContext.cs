using Microsoft.EntityFrameworkCore;
using EventSphere.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EventSphere.Data
{
    public class EventSphereContext : IdentityDbContext<ApplicationUser>
    {
        public EventSphereContext(DbContextOptions<EventSphereContext> options)
            : base(options)
        {
        }

        // EventSphere Models
        public DbSet<Event> Events { get; set; } = default!;
        public DbSet<Registration> Registrations { get; set; } = default!;
        public DbSet<EventReview> EventReviews { get; set; } = default!;
        public DbSet<EventMedia> EventMedia { get; set; } = default!;
        public DbSet<SavedMedia> SavedMedia { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;

        // Legacy Models (for backward compatibility)

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventReview>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventReview>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventMedia>()
                .HasOne(m => m.Event)
                .WithMany(e => e.Media)
                .HasForeignKey(m => m.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventMedia>()
                .HasOne(m => m.Uploader)
                .WithMany()
                .HasForeignKey(m => m.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SavedMedia>()
                .HasOne(sm => sm.Media)
                .WithMany(m => m.SavedByUsers)
                .HasForeignKey(sm => sm.MediaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SavedMedia>()
                .HasOne(sm => sm.User)
                .WithMany(u => u.SavedMedia)
                .HasForeignKey(sm => sm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne(n => n.Event)
                .WithMany()
                .HasForeignKey(n => n.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Notification>()
                .HasOne(n => n.TargetUser)
                .WithMany()
                .HasForeignKey(n => n.TargetUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes for better performance
            builder.Entity<Event>()
                .HasIndex(e => e.EventDate);

            builder.Entity<Event>()
                .HasIndex(e => e.Category);

            builder.Entity<Event>()
                .HasIndex(e => e.Status);

            builder.Entity<Registration>()
                .HasIndex(r => new { r.EventId, r.UserId })
                .IsUnique();

            builder.Entity<EventReview>()
                .HasIndex(r => new { r.EventId, r.UserId })
                .IsUnique();

            builder.Entity<SavedMedia>()
                .HasIndex(sm => new { sm.MediaId, sm.UserId })
                .IsUnique();
        }
    }
}
