using System.ComponentModel.DataAnnotations;

namespace EventSphere.Models
{
    public class SavedMedia
    {
        public int Id { get; set; }

        [Required]
        public int MediaId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime SavedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public EventMedia Media { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}

