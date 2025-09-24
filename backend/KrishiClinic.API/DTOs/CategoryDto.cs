using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ProductCount { get; set; }
    }
}
