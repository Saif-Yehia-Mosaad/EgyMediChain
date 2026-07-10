using EgyMediChain.Api.Dtos;
using EgyMediChain.Domain.Entities;
using EgyMediChain.Domain.Enums;
using EgyMediChain.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgyMediChain.Api.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AlertsController(AppDbContext db) => _db = db;

    [HttpGet("counts")]
    public async Task<ActionResult<object>> GetCounts()
    {
        return Ok(new
        {
            OpenAlerts = await _db.Alerts.CountAsync(a => a.AlertStatus == AlertStatus.Open),
            PublicScanLogs = await _db.PublicVerificationScans.CountAsync(),
            RecallAlerts = await _db.Alerts.CountAsync(a => a.AlertType == AlertType.Recall)
        });
    }

    // ---------------- Open Alerts ----------------
    [HttpGet]
    public async Task<ActionResult<PagedResult<AlertListItemDto>>> GetAll(
        [FromQuery] string? status, [FromQuery] string? severity, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var query = _db.Alerts.Include(a => a.Batch).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(a => a.AlertStatus != null && a.AlertStatus.ToString() == status);
        if (!string.IsNullOrWhiteSpace(severity))
            query = query.Where(a => a.Severity != null && a.Severity.ToString() == severity);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.CreatedAt)
            .Skip(Math.Max(0, (page - 1) * pageSize)).Take(pageSize <= 0 ? 5 : pageSize)
            .Select(a => new AlertListItemDto
            {
                Id = a.Id,
                AlertCode = a.AlertCode,
                AlertType = a.AlertType.ToString(),
                Severity = a.Severity.ToString(),
                EntityType = a.EntityType.ToString(),
                EntityName = a.EntityName,
                BatchNumber = a.Batch != null ? a.Batch.BatchNumber : null,
                Message = a.Message,
                AlertStatus = a.AlertStatus.ToString(),
                CreatedAt = a.CreatedAt
            }).ToListAsync();

        return Ok(new PagedResult<AlertListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total });
    }

    [HttpGet("recalls")]
    public async Task<ActionResult<PagedResult<AlertListItemDto>>> GetRecalls([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var query = _db.Alerts.Include(a => a.Batch).Where(a => a.AlertType == AlertType.Recall);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.CreatedAt)
            .Skip(Math.Max(0, (page - 1) * pageSize)).Take(pageSize <= 0 ? 5 : pageSize)
            .Select(a => new AlertListItemDto
            {
                Id = a.Id,
                AlertCode = a.AlertCode,
                AlertType = a.AlertType.ToString(),
                Severity = a.Severity.ToString(),
                EntityType = a.EntityType.ToString(),
                EntityName = a.EntityName,
                BatchNumber = a.Batch != null ? a.Batch.BatchNumber : null,
                Message = a.Message,
                AlertStatus = a.AlertStatus.ToString(),
                CreatedAt = a.CreatedAt
            }).ToListAsync();
        return Ok(new PagedResult<AlertListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlertListItemDto>> GetById(int id)
    {
        var a = await _db.Alerts.Include(x => x.Batch).FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound(new { message = "Alert not found." });
        return Ok(new AlertListItemDto
        {
            Id = a.Id,
            AlertCode = a.AlertCode,
            AlertType = a.AlertType?.ToString(),
            Severity = a.Severity?.ToString(),
            EntityType = a.EntityType?.ToString(),
            EntityName = a.EntityName,
            BatchNumber = a.Batch?.BatchNumber,
            Message = a.Message,
            AlertStatus = a.AlertStatus?.ToString(),
            CreatedAt = a.CreatedAt
        });
    }

    [HttpPost("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAlertStatusDto? dto)
    {
        var a = await _db.Alerts.FindAsync(id);
        if (a == null) return NotFound(new { message = "Alert not found." });

        var old = a.AlertStatus?.ToString();
        var newStatus = (dto?.Status ?? "UnderReview") switch
        {
            "Resolved" => AlertStatus.Resolved,
            "Dismissed" => AlertStatus.Dismissed,
            "Open" => AlertStatus.Open,
            _ => AlertStatus.UnderReview
        };
        a.AlertStatus = newStatus;
        if (newStatus is AlertStatus.Resolved or AlertStatus.Dismissed) a.ResolvedAt = DateTime.UtcNow;

        _db.AuditLogs.Add(new AuditLog
        {
            LogCode = $"LOG-{DateTime.UtcNow:yyyyMMddHHmmss}",
            UserDisplayName = "Dr. Saif",
            Role = SystemRole.SuperAdmin,
            Action = newStatus == AlertStatus.Resolved ? AuditAction.ResolveAlert : AuditAction.DismissAlert,
            ResourceType = "Alert",
            ResourceId = a.AlertCode,
            OldValue = old,
            NewValue = newStatus.ToString(),
            IpAddress = "127.0.0.1",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return Ok(new { message = "Alert status updated.", status = newStatus.ToString() });
    }

    // ---------------- Public Scan Logs ----------------
    [HttpGet("public-scans")]
    public async Task<ActionResult<PagedResult<ScanListItemDto>>> GetPublicScans(
        [FromQuery] string? result, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var query = _db.PublicVerificationScans.AsQueryable();
        if (!string.IsNullOrWhiteSpace(result))
            query = query.Where(s => s.VerificationResult != null && s.VerificationResult.ToString() == result);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(s => s.ScannedAt)
            .Skip(Math.Max(0, (page - 1) * pageSize)).Take(pageSize <= 0 ? 5 : pageSize)
            .Select(s => new ScanListItemDto
            {
                Id = s.Id,
                ScanCode = s.ScanCode,
                ScannedGTIN = s.ScannedGTIN,
                ScannedSerialNumber = s.ScannedSerialNumber,
                ScannedBatchNumber = s.ScannedBatchNumber,
                ProductName = s.ProductName,
                VerificationResult = s.VerificationResult.ToString(),
                Reason = s.Reason,
                Governorate = s.Governorate,
                City = s.City,
                ScannedAt = s.ScannedAt
            }).ToListAsync();

        return Ok(new PagedResult<ScanListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total });
    }

    [HttpGet("public-scans/{id:int}")]
    public async Task<ActionResult<ScanDetailsDto>> GetScanDetails(int id)
    {
        var s = await _db.PublicVerificationScans
            .Include(x => x.UnitCode).ThenInclude(u => u!.Batch).ThenInclude(b => b!.MedicineProduct)
            .Include(x => x.UnitCode).ThenInclude(u => u!.Batch).ThenInclude(b => b!.Factory)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (s == null) return NotFound(new { message = "Scan not found." });

        var dto = new ScanDetailsDto
        {
            Id = s.Id,
            ScannedGTIN = s.ScannedGTIN,
            ScannedSerialNumber = s.ScannedSerialNumber,
            ScannedBatchNumber = s.ScannedBatchNumber,
            VerificationResult = s.VerificationResult?.ToString(),
            Reason = s.Reason,
            Governorate = s.Governorate,
            City = s.City,
            ScannedAt = s.ScannedAt,
            UnitCodeId = s.UnitCodeId
        };

        if (s.UnitCode != null)
        {
            dto.ProductName = s.UnitCode.Batch?.MedicineProduct?.ProductName;
            dto.BatchNumber = s.UnitCode.Batch?.BatchNumber;
            dto.FactoryName = s.UnitCode.Batch?.Factory?.OfficialFactoryName;
            dto.UnitStatus = s.UnitCode.UnitStatus?.ToString();
            dto.ScanCount = s.UnitCode.ScanCount;
            dto.FirstScannedAt = s.UnitCode.FirstScannedAt;
        }

        return Ok(dto);
    }

    [HttpPost("public-scans/{id:int}/create-alert")]
    public async Task<IActionResult> CreateAlertFromScan(int id, [FromBody] CreateAlertFromScanDto? dto)
    {
        var s = await _db.PublicVerificationScans.FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound(new { message = "Scan not found." });

        var severity = (dto?.Severity ?? "High") switch
        {
            "Critical" => AlertSeverity.Critical,
            "Medium" => AlertSeverity.Medium,
            "Low" => AlertSeverity.Low,
            _ => AlertSeverity.High
        };

        var alert = new Alert
        {
            AlertCode = $"ALERT-{DateTime.UtcNow:yyyyMMddHHmmss}",
            AlertType = AlertType.SuspiciousScan,
            Severity = severity,
            EntityType = EntityKind.Pharmacy,
            EntityName = "Public Scan",
            Message = dto?.Message ?? $"Suspicious scan reported for batch {s.ScannedBatchNumber}.",
            AlertStatus = AlertStatus.Open,
            CreatedAt = DateTime.UtcNow
        };
        _db.Alerts.Add(alert);

        _db.AuditLogs.Add(new AuditLog
        {
            LogCode = $"LOG-{DateTime.UtcNow:yyyyMMddHHmmss}",
            UserDisplayName = "Dr. Saif",
            Role = SystemRole.SuperAdmin,
            Action = AuditAction.CreateAlert,
            ResourceType = "PublicVerificationScan",
            ResourceId = s.ScanCode,
            OldValue = null,
            NewValue = "AlertCreated",
            IpAddress = "127.0.0.1",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return Ok(new { message = "Alert created from scan.", alertCode = alert.AlertCode });
    }
}
