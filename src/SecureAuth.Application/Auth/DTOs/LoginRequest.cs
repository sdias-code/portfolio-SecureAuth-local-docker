using System.ComponentModel.DataAnnotations;

namespace SecureAuth.Application.Auth.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
