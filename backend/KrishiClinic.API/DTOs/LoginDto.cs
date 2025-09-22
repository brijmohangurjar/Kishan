using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class LoginDto
    {
        [Required]
        [StringLength(10)]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        public string Otp { get; set; } = string.Empty;
    }

    public class AdminLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class OtpRequestDto
    {
        [Required]
        [StringLength(10)]
        public string Mobile { get; set; } = string.Empty;
    }
}
