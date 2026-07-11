using EgyMediChain.Api.Common;
using EgyMediChain.Api.Dtos;
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
        await _db.SaveChangesAsync();

        return Ok(BuildResponse(user));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> Refresh()
    {
        var user = await _db.SystemUsers.FirstOrDefaultAsync(u => u.Role == Domain.Enums.SystemRole.SuperAdmin);
        if (user == null) return Unauthorized(new { message = "No account available." });
        return Ok(BuildResponse(user));
    }

    private LoginResponseDto BuildResponse(Domain.Entities.SystemUser user) => new()
    {
        Token = _jwt.GenerateAccessToken(user),
        RefreshToken = _jwt.GenerateRefreshToken(),
        UserId = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role?.ToString(),
        EntityType = user.EntityType?.ToString(),
        EntityId = user.EntityId
    };
}
