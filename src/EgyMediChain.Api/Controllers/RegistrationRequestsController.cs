using EgyMediChain.Api.Dtos;
using EgyMediChain.Domain.Entities;
using EgyMediChain.Domain.Enums;
using EgyMediChain.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgyMediChain.Api.Controllers;

[ApiController]
[Route("api/registration-requests")]
public class RegistrationRequestsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RegistrationRequestsController(AppDbContext db) => _db = db;

    // status: pending | under-review | needs-more-documents | approved | rejected | cancelled | (empty = all)
    [HttpGet]
    public async Task<ActionResult<PagedResult<RegistrationRequestListItemDto>>> GetAll(
        [FromQuery] string? status, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = _db.RegistrationRequests.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalized = status.Replace("-", "").ToLower();
            query = query.Where(r => r.RegistrationStatus != null &&
                r.RegistrationStatus.ToString()!.ToLower() == normalized);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r =>
                (r.EntityName != null && r.EntityName.Contains(search)) ||
                (r.Email != null && r.Email.Contains(search)) ||
                (r.RequestCode != null && r.RequestCode.Contains(search)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.SubmittedAt)
            .Skip(Math.Max(0, (page - 1) * pageSize))
            .Take(pageSize <= 0 ? 10 : pageSize)
            .Select(r => new RegistrationRequestListItemDto
            {
                Id = r.Id,
                RequestCode = r.RequestCode,
                EntityType = r.EntityType.ToString(),
                EntityName = r.EntityName,
                RepresentativeName = r.RepresentativeName,
                Email = r.Email,
                SubmittedAt = r.SubmittedAt,
                EmailConfirmed = r.EmailConfirmed,
                DocumentsOverallStatus = r.DocumentsOverallStatus.ToString(),
                RegistrationStatus = r.RegistrationStatus.ToString()
            }).ToListAsync();

        return Ok(new PagedResult<RegistrationRequestListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total });
    }

    [HttpGet("counts")]
    public async Task<ActionResult<object>> GetCounts()
    {
        return Ok(new
        {
            PendingReview = await _db.RegistrationRequests.CountAsync(r => r.RegistrationStatus == RegistrationStatus.Pending || r.RegistrationStatus == RegistrationStatus.UnderReview),
            NeedsMoreDocuments = await _db.RegistrationRequests.CountAsync(r => r.RegistrationStatus == RegistrationStatus.NeedsMoreDocuments),
            Approved = await _db.RegistrationRequests.CountAsync(r => r.RegistrationStatus == RegistrationStatus.Approved),
            Rejected = await _db.RegistrationRequests.CountAsync(r => r.RegistrationStatus == RegistrationStatus.Rejected),
            Cancelled = await _db.RegistrationRequests.CountAsync(r => r.RegistrationStatus == RegistrationStatus.Cancelled)
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegistrationRequestDetailsDto>> GetById(int id)
    {
        var r = await _db.RegistrationRequests
            .Include(x => x.Factory)
            .Include(x => x.Warehouse)
            .Include(x => x.Pharmacy).ThenInclude(p => p!.DefaultWarehouse)
            .Include(x => x.SystemUser)
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (r == null) return NotFound(new { message = "Registration request not found." });

        var entity = new EntityInfoDto();
        if (r.Factory != null)
        {
            var f = r.Factory;
            entity.OfficialFactoryName = f.OfficialFactoryName;
            entity.LegalCompanyName = f.LegalCompanyName;
            entity.DosageFormsProduced = f.DosageFormsProduced;
            entity.Governorate = f.Governorate; entity.City = f.City; entity.DistrictArea = f.DistrictArea; entity.FullAddress = f.FullAddress;
            entity.FactoryLicenseNumber = f.FactoryLicenseNumber;
            entity.TechnicalOperatingLicenseNumber = f.TechnicalOperatingLicenseNumber;
            entity.CommercialRegistrationNumber = f.CommercialRegistrationNumber;
            entity.TaxCardNumber = f.TaxCardNumber;
            entity.LicenseIssueDate = f.LicenseIssueDate; entity.LicenseExpiryDate = f.LicenseExpiryDate;
            entity.HasQualityControlLab = f.HasQualityControlLab; entity.HasFinishedGoodsStore = f.HasFinishedGoodsStore;
            entity.HasColdStorage = f.HasColdStorage; entity.HasQuarantineArea = f.HasQuarantineArea;
            entity.Status = f.FactoryStatus?.ToString();
        }
        else if (r.Warehouse != null)
        {
            var w = r.Warehouse;
            entity.OfficialWarehouseName = w.OfficialWarehouseName;
            entity.WarehouseType = w.WarehouseType;
            entity.Governorate = w.Governorate; entity.City = w.City; entity.DistrictArea = w.DistrictArea; entity.FullAddress = w.FullAddress;
            entity.WarehouseLicenseNumber = w.WarehouseLicenseNumber;
            entity.LicenseIssueDate = w.LicenseIssueDate; entity.LicenseExpiryDate = w.LicenseExpiryDate;
            entity.HasColdStorage = w.HasColdStorage; entity.HasQuarantineArea = w.HasQuarantineArea; entity.HasDeliveryService = w.HasDeliveryService;
            entity.Status = w.WarehouseStatus?.ToString();
        }
        else if (r.Pharmacy != null)
        {
            var p = r.Pharmacy;
            entity.OfficialPharmacyName = p.OfficialPharmacyName;
            entity.PharmacyType = p.PharmacyType;
            entity.Governorate = p.Governorate; entity.City = p.City; entity.DistrictArea = p.DistrictArea; entity.FullAddress = p.FullAddress;
            entity.DefaultWarehouseName = p.DefaultWarehouse?.OfficialWarehouseName;
            entity.HasColdStorage = p.HasColdStorage;
            entity.PharmacyLicenseNumber = p.PharmacyLicenseNumber;
            entity.LicenseIssueDate = p.LicenseIssueDate; entity.LicenseExpiryDate = p.LicenseExpiryDate;
            entity.PharmacistSyndicateId = p.PharmacistSyndicateId;
            entity.Status = p.PharmacyStatus?.ToString();
        }

        var dto = new RegistrationRequestDetailsDto
        {
            Id = r.Id,
            RequestCode = r.RequestCode,
            EntityType = r.EntityType?.ToString(),
            SubmittedAt = r.SubmittedAt,
            RegistrationStatus = r.RegistrationStatus?.ToString(),
            AdminNotes = r.AdminNotes,
            RejectionReason = r.RejectionReason,
            Account = r.SystemUser == null ? null : new AccountInfoDto
            {
                FullName = r.SystemUser.FullName,
                Email = r.SystemUser.Email,
                MobileNumber = r.SystemUser.MobileNumber,
                NationalIdMasked = MaskNationalId(r.SystemUser.NationalId),
                EmailConfirmed = r.SystemUser.EmailConfirmed,
                IsActive = r.SystemUser.IsActive
            },
            Entity = entity,
            Documents = r.Documents?.Select(d => new DocumentItemDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                FileName = d.FileName,
                FileUrl = d.FileUrl,
                UploadedAt = d.UploadedAt,
                DocumentStatus = d.DocumentStatus?.ToString(),
                ReviewedBy = d.ReviewedBy,
                ReviewedAt = d.ReviewedAt,
                RejectionReason = d.RejectionReason
            }).ToList()
        };

        return Ok(dto);
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var r = await _db.RegistrationRequests
            .Include(x => x.Factory).Include(x => x.Warehouse).Include(x => x.Pharmacy).Include(x => x.SystemUser)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return NotFound(new { message = "Registration request not found." });

        r.RegistrationStatus = RegistrationStatus.Approved;
        if (r.Factory != null) r.Factory.FactoryStatus = FacilityStatus.Active;
        if (r.Warehouse != null) r.Warehouse.WarehouseStatus = FacilityStatus.Active;
        if (r.Pharmacy != null) r.Pharmacy.PharmacyStatus = FacilityStatus.Active;
        if (r.SystemUser != null) r.SystemUser.IsActive = true;

        _db.AuditLogs.Add(NewLog(r.SystemUser, AuditAction.ApproveRegistration, "RegistrationRequest", r.RequestCode, "Pending", "Approved"));
        await _db.SaveChangesAsync();
        return Ok(new { message = "Registration request approved.", status = "Approved" });
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectRequestDto? dto)
    {
        var r = await _db.RegistrationRequests
            .Include(x => x.Factory).Include(x => x.Warehouse).Include(x => x.Pharmacy).Include(x => x.SystemUser)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return NotFound(new { message = "Registration request not found." });

        r.RegistrationStatus = RegistrationStatus.Rejected;
        r.RejectionReason = dto?.RejectionReason ?? "Not specified";
        if (r.Factory != null) r.Factory.FactoryStatus = FacilityStatus.Rejected;
        if (r.Warehouse != null) r.Warehouse.WarehouseStatus = FacilityStatus.Rejected;
        if (r.Pharmacy != null) r.Pharmacy.PharmacyStatus = FacilityStatus.Rejected;
        if (r.SystemUser != null) r.SystemUser.IsActive = false;

        _db.AuditLogs.Add(NewLog(r.SystemUser, AuditAction.RejectRegistration, "RegistrationRequest", r.RequestCode, "Pending", "Rejected"));
        await _db.SaveChangesAsync();
        return Ok(new { message = "Registration request rejected.", status = "Rejected" });
    }

    [HttpPost("{id:int}/request-more-documents")]
    public async Task<IActionResult> RequestMoreDocuments(int id, [FromBody] RequestMoreDocumentsDto? dto)
    {
        var r = await _db.RegistrationRequests.Include(x => x.SystemUser).Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return NotFound(new { message = "Registration request not found." });

        r.RegistrationStatus = RegistrationStatus.NeedsMoreDocuments;
        r.AdminNotes = dto?.AdminNotes;

        if (dto?.DocumentIdsNeedingReplacement != null && r.Documents != null)
        {
            foreach (var docId in dto.DocumentIdsNeedingReplacement)
            {
                var doc = r.Documents.FirstOrDefault(d => d.Id == docId);
                if (doc != null) doc.DocumentStatus = DocumentStatus.NeedsReplacement;
            }
        }

        _db.AuditLogs.Add(NewLog(r.SystemUser, AuditAction.UpdateDocumentStatus, "EntityDocument", r.RequestCode, "UnderReview", "NeedsReplacement"));
        await _db.SaveChangesAsync();
        return Ok(new { message = "More documents requested.", status = "NeedsMoreDocuments" });
    }

    [HttpPost("documents/{documentId:int}/status")]
    public async Task<IActionResult> UpdateDocumentStatus(int documentId, [FromBody] DocumentStatusUpdateDto? dto)
    {
        var doc = await _db.EntityDocuments.FirstOrDefaultAsync(d => d.Id == documentId);
        if (doc == null) return NotFound(new { message = "Document not found." });

        doc.DocumentStatus = (dto?.Status ?? "Complete") switch
        {
            "Complete" => DocumentStatus.Complete,
            "NeedsReplacement" => DocumentStatus.NeedsReplacement,
            "Rejected" => DocumentStatus.Rejected,
            _ => DocumentStatus.UnderReview
        };
        doc.RejectionReason = dto?.RejectionReason;
        doc.ReviewedAt = DateTime.UtcNow;
        doc.ReviewedBy = "Dr. Saif";

        await _db.SaveChangesAsync();
        return Ok(new { message = "Document status updated.", status = doc.DocumentStatus.ToString() });
    }

    private static string? MaskNationalId(string? nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length < 4) return nationalId;
        return $"**** **** {nationalId[^4..]}";
    }

    private static AuditLog NewLog(SystemUser? user, AuditAction action, string resourceType, string? resourceId, string? oldVal, string? newVal) => new()
    {
        LogCode = $"LOG-{DateTime.UtcNow:yyyyMMddHHmmss}",
        UserId = user?.Id,
        UserDisplayName = "Dr. Saif",
        Role = SystemRole.SuperAdmin,
        Action = action,
        ResourceType = resourceType,
        ResourceId = resourceId,
        OldValue = oldVal,
        NewValue = newVal,
        IpAddress = "127.0.0.1",
        CreatedAt = DateTime.UtcNow
    };
}
