using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class CreateVideoDto
    {
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

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int Duration { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateVideoDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? VideoUrl { get; set; }

        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int? Duration { get; set; }

        public int? DisplayOrder { get; set; }

        public bool? IsActive { get; set; }
    }
}
