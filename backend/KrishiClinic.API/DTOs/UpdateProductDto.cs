using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class UpdateProductDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        public int? StockQuantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
