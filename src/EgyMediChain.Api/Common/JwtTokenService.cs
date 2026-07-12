using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EgyMediChain.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace EgyMediChain.Api.Common;

public class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(SystemUser user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("fullName", user.FullName ?? string.Empty),
            new(ClaimTypes.Role, user.Role?.ToString() ?? "MinistryViewer"),
            // entityId/entityType are what let every controller tell "this token belongs to
            // Factory #7" from "this token belongs to Factory #12" - required for ownership checks.
            new("entityType", user.EntityType?.ToString() ?? string.Empty),
            new("entityId", user.EntityId?.ToString() ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiryMinutes"] ?? "120")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken() => Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
}
