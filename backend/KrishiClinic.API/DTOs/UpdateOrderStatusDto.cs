using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}

