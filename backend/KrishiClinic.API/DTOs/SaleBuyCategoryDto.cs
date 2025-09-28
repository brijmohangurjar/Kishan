using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class CreateSaleBuyCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public IFormFile? ImageFile { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSaleBuyCategoryDto
    {
        [Required]
        public int SaleBuyCategoryId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public IFormFile? ImageFile { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class SaleBuyCategoryResponseDto
    {
        public int SaleBuyCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
