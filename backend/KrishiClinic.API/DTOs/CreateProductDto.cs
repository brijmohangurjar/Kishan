using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace KrishiClinic.API.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? AdditionalImageUrls { get; set; } // JSON array of additional image URLs

        [StringLength(100)]
        public string? Category { get; set; }

        public int StockQuantity { get; set; } = 0;

        // Helper method to get additional image URLs as array
        public string[] GetAdditionalImageUrlsArray()
        {
            if (string.IsNullOrEmpty(AdditionalImageUrls))
                return new string[0];
            
            try
            {
                return JsonSerializer.Deserialize<string[]>(AdditionalImageUrls) ?? new string[0];
            }
            catch
            {
                return new string[0];
            }
        }

        // Helper method to set additional image URLs from array
        public void SetAdditionalImageUrlsArray(string[] urls)
        {
            AdditionalImageUrls = urls.Length > 0 ? JsonSerializer.Serialize(urls) : null;
        }
    }
}

