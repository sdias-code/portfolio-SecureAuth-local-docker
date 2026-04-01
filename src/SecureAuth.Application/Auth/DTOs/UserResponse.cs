namespace SecureAuth.Application.Auth.DTOs
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}