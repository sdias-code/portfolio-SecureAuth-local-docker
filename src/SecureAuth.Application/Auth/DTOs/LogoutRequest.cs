namespace SecureAuth.Application.Auth.DTOs;
// 🔹 DTO para logout
public record LogoutRequest(string RefreshToken, bool LogoutAll = false);
