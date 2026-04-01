namespace SecureAuth.Application.Auth.DTOs
{   
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public AuthResponse(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}
