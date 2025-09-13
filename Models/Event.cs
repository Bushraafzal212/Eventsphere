using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSphere.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EventType { get; set; } = string.Empty; // Seminar, Competition, Workshop, etc.

        [Required]
        [StringLength(200)]
        public string Venue { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(100)]
        public string OrganizingDepartment { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string OrganizerName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string OrganizerEmail { get; set; } = string.Empty;

        [Required]
        public int MaxCapacity { get; set; }

        public int CurrentRegistrations { get; set; } = 0;

        [Required]
        public EventStatus Status { get; set; } = EventStatus.Pending;

        [Required]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? BannerImageUrl { get; set; }

        [StringLength(500)]
        public string? RulebookUrl { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CertificateFee { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<EventMedia> Media { get; set; } = new List<EventMedia>();
        public ICollection<EventReview> Reviews { get; set; } = new List<EventReview>();
    }

    public enum EventStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled,
        Completed
    }

    public enum EventCategory
    {
        Cultural,
        Academic,
        Sports,
        Social,
        Entertainment,
        Career
    }
}

