using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EventSphere.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(50)]
        public string? EnrollmentNumber { get; set; }

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Participant;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<EventReview> Reviews { get; set; } = new List<EventReview>();
        public ICollection<SavedMedia> SavedMedia { get; set; } = new List<SavedMedia>();
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
    }

    public enum UserRole
    {
        Visitor,
        Participant,
        Organizer,
        Admin
    }
}

