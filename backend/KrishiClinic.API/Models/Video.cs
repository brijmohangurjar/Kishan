using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrishiClinic.API.Models
{
    public class Video
    {
        [Key]
        public int VideoId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(500)]
        public string VideoUrl { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "General";

        public int Duration { get; set; } // Duration in seconds

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; } = 0; // For ordering in slider

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; } // Admin ID who created the video

        // Navigation property
        [ForeignKey("CreatedBy")]
        public virtual Admin? CreatedByAdmin { get; set; }
    }
}
