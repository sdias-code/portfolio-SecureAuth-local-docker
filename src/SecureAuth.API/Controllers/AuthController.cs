using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SecureAuth.Application.Auth.Commands.Login;
using SecureAuth.Application.Auth.Commands.Logout;
using SecureAuth.Application.Auth.Commands.Refresh;
using SecureAuth.Application.Auth.Commands.Register;
using SecureAuth.Application.Auth.DTOs;
using SecureAuth.Application.Auth.Queries.GetCurrentUser;
using SecureAuth.Domain.Exceptions;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // 🔹 Consulta do usuário atual
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var result = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(result);
    }

    // 🔹 Registro de novo usuário
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;

        _logger.LogInformation(
            "Registro iniciado | Email: {Email} | CorrelationId: {CorrelationId}",
            request.Email,
            correlationId);

        await _mediator.Send(new RegisterCommand(request.Email, request.Password));

        _logger.LogInformation(
            "Usuário registrado com sucesso | Email: {Email} | CorrelationId: {CorrelationId}",
            request.Email,
            correlationId);

        return StatusCode(StatusCodes.Status201Created);
    }

    // 🔹 Login
    [EnableRateLimiting("fixed")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;

        _logger.LogInformation(
            "Tentativa de login | Email: {Email} | CorrelationId: {CorrelationId}",
            request.Email,
            correlationId);

        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password));

        _logger.LogInformation(
            "Login realizado com sucesso | Email: {Email} | CorrelationId: {CorrelationId}",
            request.Email,
            correlationId);

        return Ok(result);
    }

    // 🔹 Refresh Token
    [EnableRateLimiting("fixed")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;

        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));

        _logger.LogInformation(
            "Token atualizado | CorrelationId: {CorrelationId}",
            correlationId);

        return Ok(result);
    }

    // 🔹 Logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;

        // Logout individual ou global (LogoutAll)
        await _mediator.Send(new LogoutCommand(request.RefreshToken, request.LogoutAll));

        _logger.LogInformation(
            "Logout realizado | CorrelationId: {CorrelationId}",
            correlationId);

        return Ok(new { message = "Logout realizado" });
    }

    // 🔹 Rota de teste de erro
    [HttpGet("error")]
    public IActionResult Error()
    {
        throw new BusinessException("Erro de teste");
    }
}