using System.ComponentModel.DataAnnotations;

namespace EventSphere.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; }

        [StringLength(100)]
        public string? TargetRole { get; set; } // null for all users, specific role for targeted

        public string? TargetUserId { get; set; } // null for broadcast, specific user for targeted

        public int? EventId { get; set; } // null for general notifications

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ReadAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Event? Event { get; set; }
        public ApplicationUser? TargetUser { get; set; }
    }

    public enum NotificationType
    {
        EventCreated,
        EventUpdated,
        EventCancelled,
        RegistrationConfirmed,
        RegistrationCancelled,
        EventReminder,
        CertificateReady,
        GeneralAnnouncement,
        SystemAlert
    }
}

