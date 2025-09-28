using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.Models
{
    public class SaleBuyCategory
    {
        [Key]
        public int SaleBuyCategoryId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation property
        public virtual ICollection<SaleBuyProduct> SaleBuyProducts { get; set; } = new List<SaleBuyProduct>();
    }
}
