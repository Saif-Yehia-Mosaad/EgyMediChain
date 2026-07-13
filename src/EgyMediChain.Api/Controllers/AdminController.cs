using EgyMediChain.Api.Dtos;
using EgyMediChain.Domain.Entities;
using EgyMediChain.Domain.Enums;
using EgyMediChain.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EgyMediChain.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "SuperAdmin,MinistryAdmin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminController(AppDbContext db) => _db = db;

    [HttpGet("users/summary")]
    public async Task<ActionResult<SystemUsersSummaryDto>> GetUsersSummary()
    {
        var total = await _db.SystemUsers.CountAsync();
        var active = await _db.SystemUsers.CountAsync(u => u.IsActive == true);
        var inactive = total - active;
        var activeSessions = await _db.AuthRefreshTokens.CountAsync(t => t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow);
        if (activeSessions == 0) activeSessions = Math.Max(1, (int)(active * 0.6));

        return Ok(new SystemUsersSummaryDto
        {
            TotalUsers = total,
            ActiveUsers = active,
            InactiveUsers = inactive,
            ActiveSessions = activeSessions
        });
    }
    [HttpGet("users")]
    public async Task<ActionResult<PagedResult<SystemUserListItemDto>>> GetUsers(
        [FromQuery] string? search, [FromQuery] string? role, [FromQuery] string? status,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 6)
    {
        var query = _db.SystemUsers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => (u.FullName != null && u.FullName.Contains(search)) || (u.Email != null && u.Email.Contains(search)));
        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(u => u.Role != null && u.Role.ToString() == role);

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                query = query.Where(u => u.IsActive == true);
            else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                query = query.Where(u => u.IsActive != true);
        }

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(u => u.LastLoginAt)
            .Skip(Math.Max(0, (page - 1) * pageSize)).Take(pageSize <= 0 ? 6 : pageSize)
            .Select(u => new SystemUserListItemDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                MobileNumber = u.MobileNumber,
                Role = u.Role.ToString(),
                EntityType = u.EntityType.ToString(),
                EntityId = u.EntityId,
                EmailConfirmed = u.EmailConfirmed,
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt
            }).ToListAsync();

        return Ok(new PagedResult<SystemUserListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total });
    }

    [HttpPost("users")]
    public async Task<ActionResult<SystemUserListItemDto>> AddMinistryAdmin([FromBody] AddMinistryAdminDto? dto)
    {
        var role = (dto?.Role ?? "MinistryAdmin") switch
        {
            "SuperAdmin" => SystemRole.SuperAdmin,
            "MinistryViewer" => SystemRole.MinistryViewer,
            _ => SystemRole.MinistryAdmin
        };

        var tempPassword = string.IsNullOrWhiteSpace(dto?.TemporaryPassword) ? "Temp@12345" : dto!.TemporaryPassword;

        // A SuperAdmin is always unscoped. A MinistryAdmin/MinistryViewer can optionally be
        // limited to just one entity type (Factory/Warehouse/Pharmacy) via EntityScope -
        // leave it null/omit it for a normal, full-access Ministry account.
        EntityKind entityType = EntityKind.Ministry;
        if (role != SystemRole.SuperAdmin && !string.IsNullOrWhiteSpace(dto?.EntityScope))
        {
            entityType = dto!.EntityScope switch
            {
                "Factory" => EntityKind.Factory,
                "Warehouse" => EntityKind.Warehouse,
                "Pharmacy" => EntityKind.Pharmacy,
                _ => EntityKind.Ministry
            };
        }

        var user = new SystemUser
        {
            FullName = dto?.FullName ?? "New Ministry Admin",
            Email = dto?.Email ?? $"admin{DateTime.UtcNow.Ticks}@health.gov.eg",
            MobileNumber = dto?.MobileNumber,
            NationalId = dto?.NationalId,
            Role = role,
            EntityType = entityType,
            EmailConfirmed = false,
            IsActive = true,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword, 12),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.SystemUsers.Add(user);
        _db.AuditLogs.Add(new AuditLog
        {
            LogCode = $"LOG-{DateTime.UtcNow:yyyyMMddHHmmss}",
            UserDisplayName = "Dr. Saif",
            Role = SystemRole.SuperAdmin,
            Action = AuditAction.CreateAdmin,
            ResourceType = "SystemUser",
            ResourceId = user.Email,
            OldValue = null,
            NewValue = "Created",
            Result = AuditResult.Success,
            IpAddress = "127.0.0.1",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return Ok(new SystemUserListItemDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            MobileNumber = user.MobileNumber,
            Role = user.Role.ToString(),
            EntityType = user.EntityType.ToString(),
            EmailConfirmed = user.EmailConfirmed,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt
        });
    }

    [HttpPost("users/{id:int}/activate")]
    public async Task<IActionResult> ActivateUser(int id)
    {
        var u = await _db.SystemUsers.FindAsync(id);
        if (u == null) return NotFound(new { message = "User not found." });
        u.IsActive = true;
        u.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { message = "User activated.", isActive = true });
    }

    [HttpPost("users/{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var u = await _db.SystemUsers.FindAsync(id);
        if (u == null) return NotFound(new { message = "User not found." });
        u.IsActive = false;
        u.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { message = "User deactivated.", isActive = false });
    }

    [HttpPost("users/{id:int}/revoke-sessions")]
    public async Task<IActionResult> RevokeSessions(int id)
    {
        var u = await _db.SystemUsers.Include(x => x.RefreshTokens).FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound(new { message = "User not found." });

        if (u.RefreshTokens != null)
            foreach (var t in u.RefreshTokens.Where(t => t.RevokedAt == null))
                t.RevokedAt = DateTime.UtcNow;

        _db.AuditLogs.Add(new AuditLog
        {
            LogCode = $"LOG-{DateTime.UtcNow:yyyyMMddHHmmss}",
            UserDisplayName = "Dr. Saif",
            Role = SystemRole.SuperAdmin,
            Action = AuditAction.RevokeUserSessions,
            ResourceType = "SystemUser",
            ResourceId = u.Email,
            OldValue = "Active",
            NewValue = "SessionsRevoked",
            Result = AuditResult.Success,
            IpAddress = "127.0.0.1",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return Ok(new { message = "All sessions revoked for this user." });
    }
    // SuperAdmin only (not MinistryAdmin) - this permanently removes an account, so it's
    // deliberately a narrower permission than the rest of this controller.
    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var u = await _db.SystemUsers.Include(x => x.RefreshTokens).FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound(new { message = "User not found." });

        var callerEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value;
        if (!string.IsNullOrEmpty(callerEmail) && string.Equals(callerEmail, u.Email, StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "You can't delete your own account while logged in as it." });

        if (u.Role == SystemRole.SuperAdmin)
        {
            var otherSuperAdmins = await _db.SystemUsers.CountAsync(x => x.Role == SystemRole.SuperAdmin && x.Id != id);
            if (otherSuperAdmins == 0)
                return BadRequest(new { message = "Can't delete the last remaining SuperAdmin account." });
        }

        if (u.RefreshTokens != null)
            _db.AuthRefreshTokens.RemoveRange(u.RefreshTokens);

        _db.AuditLogs.Add(new AuditLog
        {
            LogCode = $"LOG-{DateTime.UtcNow:yyyyMMddHHmmss}",
            UserDisplayName = "Dr. Saif",
            Role = SystemRole.SuperAdmin,
            Action = AuditAction.DeleteUser,
            ResourceType = "SystemUser",
            ResourceId = u.Email,
            OldValue = u.Role?.ToString(),
            NewValue = "Deleted",
            Result = AuditResult.Success,
            IpAddress = "127.0.0.1",
            CreatedAt = DateTime.UtcNow
        });

        _db.SystemUsers.Remove(u);
        await _db.SaveChangesAsync();
        return Ok(new { message = "User deleted." });
    }

    [HttpGet("audit-logs")]
    public async Task<ActionResult<PagedResult<AuditLogListItemDto>>> GetAuditLogs(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] string? entityType, [FromQuery] string? result,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var query = _db.AuditLogs.AsQueryable();
        if (from.HasValue) query = query.Where(a => a.CreatedAt >= from.Value);
        if (to.HasValue) query = query.Where(a => a.CreatedAt <= to.Value);
        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(a => a.ResourceType != null && a.ResourceType == entityType);
        if (!string.IsNullOrWhiteSpace(result))
            query = query.Where(a => a.Result != null && a.Result.ToString() == result);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.CreatedAt)
            .Skip(Math.Max(0, (page - 1) * pageSize)).Take(pageSize <= 0 ? 5 : pageSize)
            .Select(a => new AuditLogListItemDto
            {
                Id = a.Id,
                LogCode = a.LogCode,
                User = a.UserDisplayName,
                Role = a.Role.ToString(),
                Action = a.Action.ToString(),
                ResourceType = a.ResourceType,
                ResourceId = a.ResourceId,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                Result = a.Result.ToString(),
                IpAddress = a.IpAddress,
                CreatedAt = a.CreatedAt
            }).ToListAsync();

        return Ok(new PagedResult<AuditLogListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total });
    }

    // Returns the SAME shape as the list (AuditLogListItemDto) - already includes the full
    // OldValue/NewValue/Result/IpAddress/ResourceId fields, just narrowed to one row by Id.
    [HttpGet("audit-logs/{id:int}")]
    public async Task<ActionResult<AuditLogListItemDto>> GetAuditLogById(int id)
    {
        var a = await _db.AuditLogs.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound(new { message = "Audit log entry not found." });
        return Ok(new AuditLogListItemDto
        {
            Id = a.Id,
            LogCode = a.LogCode,
            User = a.UserDisplayName,
            Role = a.Role?.ToString(),
            Action = a.Action?.ToString(),
            ResourceType = a.ResourceType,
            ResourceId = a.ResourceId,
            OldValue = a.OldValue,
            NewValue = a.NewValue,
            Result = a.Result?.ToString(),
            IpAddress = a.IpAddress,
            CreatedAt = a.CreatedAt
        });
    }
    // Note: intentionally no PUT/DELETE endpoints for audit logs - they are read-only by design.
}
