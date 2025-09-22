using System.ComponentModel.DataAnnotations;

namespace KrishiClinic.API.DTOs
{
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Village { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }
    }
}
