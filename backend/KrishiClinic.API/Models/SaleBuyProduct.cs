using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrishiClinic.API.Models
{
    public class SaleBuyProduct
    {
        [Key]
        public int SaleBuyProductId { get; set; }
        
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
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? ImageUrls { get; set; } // JSON array of image URLs
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Foreign key
        [Required]
        public int SaleBuyCategoryId { get; set; }
        
        // Navigation property
        [ForeignKey("SaleBuyCategoryId")]
        public virtual SaleBuyCategory SaleBuyCategory { get; set; } = null!;
        
        // User who created this product (for edit/delete permissions)
        [Required]
        public int CreatedByUserId { get; set; }
        
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; } = null!;
    }
}
