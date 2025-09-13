using System.ComponentModel.DataAnnotations;

namespace EventSphere.Models
{
    public class Registration
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Registered;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public DateTime? CancellationDate { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        public bool AttendanceMarked { get; set; } = false;

        public DateTime? AttendanceDate { get; set; }

        [StringLength(500)]
        public string? QRCode { get; set; }

        public bool CertificateRequested { get; set; } = false;

        public bool CertificateFeePaid { get; set; } = false;

        public DateTime? CertificateGeneratedDate { get; set; }

        [StringLength(500)]
        public string? CertificateUrl { get; set; }

        // Navigation properties
        public Event Event { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }

    public enum RegistrationStatus
    {
        Registered,
        Cancelled,
        Attended,
        NoShow
    }
}

