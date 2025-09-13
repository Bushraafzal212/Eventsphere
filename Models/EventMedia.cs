using System.ComponentModel.DataAnnotations;

namespace EventSphere.Models
{
    public class EventMedia
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        [Required]
        public MediaType Type { get; set; }

        [Required]
        [StringLength(50)]
        public string FileExtension { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [Required]
        public string UploadedBy { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;

        public bool IsFeatured { get; set; } = false;

        // Navigation properties
        public Event Event { get; set; } = null!;
        public ApplicationUser Uploader { get; set; } = null!;
        public ICollection<SavedMedia> SavedByUsers { get; set; } = new List<SavedMedia>();
    }

    public enum MediaType
    {
        Image,
        Video,
        Document
    }
}

