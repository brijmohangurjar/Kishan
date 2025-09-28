using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class CreateSaleBuyProductDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string PlaceName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string ProductDescription { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(15)]
        [RegularExpression(@"^[\+]?[1-9][\d]{0,15}$", ErrorMessage = "Phone number must be valid")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        public int SaleBuyCategoryId { get; set; }
        
        public List<IFormFile>? ImageFiles { get; set; }
        
        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateSaleBuyProductDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string PlaceName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string ProductDescription { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(15)]
        [RegularExpression(@"^[\+]?[1-9][\d]{0,15}$", ErrorMessage = "Phone number must be valid")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        public int SaleBuyCategoryId { get; set; }
        
        public List<IFormFile>? ImageFiles { get; set; }
        
        public List<string>? ImageUrls { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class SaleBuyProductResponseDto
    {
        public int SaleBuyProductId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SaleBuyCategoryId { get; set; }
        public string SaleBuyCategoryName { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public bool CanEdit { get; set; } // True if current user can edit/delete this product
    }
}
