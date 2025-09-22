using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Village { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Mobile { get; set; } = string.Empty;
    }
}

