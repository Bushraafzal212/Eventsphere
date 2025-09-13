using System.ComponentModel.DataAnnotations;

namespace EventSphere.Models
{
    public class EventReview
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int OverallRating { get; set; }

        [Range(1, 5)]
        public int VenueRating { get; set; }

        [Range(1, 5)]
        public int CoordinationRating { get; set; }

        [Range(1, 5)]
        public int TechnicalRating { get; set; }

        [Range(1, 5)]
        public int HospitalityRating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        [StringLength(1000)]
        public string? Suggestions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;

        // Navigation properties
        public Event Event { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}

