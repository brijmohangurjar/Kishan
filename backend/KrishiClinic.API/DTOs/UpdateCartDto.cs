using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class UpdateCartDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}

