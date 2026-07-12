using EgyMediChain.Api.Common;
using EgyMediChain.Api.Dtos;
using EgyMediChain.Domain.Entities;
using EgyMediChain.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgyMediChain.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _jwt;

    public AuthController(AppDbContext db, JwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto? dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        var user = await _db.SystemUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || string.IsNullOrEmpty(user.PasswordHash) ||
            !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        if (user.IsActive != true)
            return Unauthorized(new { message = "This account is not active yet. Wait for Ministry approval." });

        user.LastLoginAt = DateTime.UtcNow;
        var response = await IssueTokensAsync(user);
        await _db.SaveChangesAsync();

        return Ok(response);
    }

    // Body: { "refreshToken": "..." }
    // Rotates the refresh token: the old one is revoked and a brand new pair (access + refresh) is issued.
    // This replaces the old /refresh, which just handed back a SuperAdmin token to anyone who called it.
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> Refresh([FromBody] RefreshRequestDto? dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest(new { message = "Refresh token is required." });

        var stored = await _db.AuthRefreshTokens
            .Include(t => t.SystemUser)
            .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);

        if (stored == null || stored.RevokedAt != null || stored.ExpiresAt == null || stored.ExpiresAt <= DateTime.UtcNow)
            return Unauthorized(new { message = "Refresh token is invalid or expired. Please log in again." });

        if (stored.SystemUser == null || stored.SystemUser.IsActive != true)
            return Unauthorized(new { message = "Account is not active." });

        stored.RevokedAt = DateTime.UtcNow;
        var response = await IssueTokensAsync(stored.SystemUser);
        await _db.SaveChangesAsync();

        return Ok(response);
    }

    // Body: { "refreshToken": "..." }
    // Logs the user out on this device by revoking that one refresh token (their access token
    // keeps working until it naturally expires - that's normal for stateless JWTs).
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequestDto? dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest(new { message = "Refresh token is required." });

        var stored = await _db.AuthRefreshTokens.FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);
        if (stored != null && stored.RevokedAt == null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return Ok(new { message = "Logged out." });
    }

    private async Task<LoginResponseDto> IssueTokensAsync(SystemUser user)
    {
        var refreshToken = _jwt.GenerateRefreshToken();

        _db.AuthRefreshTokens.Add(new AuthRefreshToken
        {
            SystemUserId = user.Id,
            Token = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return new LoginResponseDto
        {
            Token = _jwt.GenerateAccessToken(user),
            RefreshToken = refreshToken,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role?.ToString(),
            EntityType = user.EntityType?.ToString(),
            EntityId = user.EntityId
        };
    }
}
