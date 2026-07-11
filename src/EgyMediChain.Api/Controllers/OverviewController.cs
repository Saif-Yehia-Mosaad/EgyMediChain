using EgyMediChain.Api.Dtos;
using EgyMediChain.Domain.Enums;
using EgyMediChain.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EgyMediChain.Api.Controllers;

[ApiController]
[Route("api/overview")]
[Authorize(Roles = "SuperAdmin,MinistryAdmin,MinistryViewer")]
public class OverviewController : ControllerBase
{
    private readonly AppDbContext _db;
    public OverviewController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<OverviewDto>> Get()
    {
        var cards = new OverviewCardsDto
        {
            PendingRequests = await _db.RegistrationRequests.CountAsync(r => r.RegistrationStatus == RegistrationStatus.Pending),
            ActiveFactories = await _db.Factories.CountAsync(f => f.FactoryStatus == FacilityStatus.Active),
            ActiveWarehouses = await _db.Warehouses.CountAsync(w => w.WarehouseStatus == FacilityStatus.Active),
            ActivePharmacies = await _db.Pharmacies.CountAsync(p => p.PharmacyStatus == FacilityStatus.Active),
            ActiveBatches = await _db.Batches.CountAsync(b => b.BatchStatus != BatchStatus.Recalled),
            ShipmentsInTransit = await _db.Shipments.CountAsync(s => s.ShipmentStatus == ShipmentStatus.InTransit),
            OpenAlerts = await _db.Alerts.CountAsync(a => a.AlertStatus == AlertStatus.Open),
            SuspiciousPublicScans = await _db.PublicVerificationScans.CountAsync(s => s.VerificationResult == VerificationResult.Suspicious)
        };

        var recentRequests = await _db.RegistrationRequests
            .OrderByDescending(r => r.SubmittedAt)
            .Take(5)
            .Select(r => new RecentRegistrationRequestDto
            {
                Id = r.Id,
                RequestCode = r.RequestCode,
                EntityType = r.EntityType.ToString(),
                EntityName = r.EntityName,
                SubmittedBy = r.RepresentativeName,
                SubmittedAt = r.SubmittedAt,
                RegistrationStatus = r.RegistrationStatus.ToString()
            }).ToListAsync();

        var recentAlerts = await _db.Alerts
            .Include(a => a.Batch)
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .Select(a => new RecentAlertDto
            {
                Id = a.Id,
                AlertCode = a.AlertCode,
                AlertType = a.AlertType.ToString(),
                Severity = a.Severity.ToString(),
                EntityType = a.EntityType.ToString(),
                BatchNumber = a.Batch != null ? a.Batch.BatchNumber : null,
                CreatedAt = a.CreatedAt,
                AlertStatus = a.AlertStatus.ToString()
            }).ToListAsync();

        var recentBatches = await _db.Batches
            .Include(b => b.MedicineProduct)
            .Include(b => b.Factory)
            .OrderByDescending(b => b.UpdatedAt)
            .Take(5)
            .Select(b => new RecentBatchActivityDto
            {
                Id = b.Id,
                ProductName = b.MedicineProduct != null ? b.MedicineProduct.ProductName : null,
                BatchNumber = b.BatchNumber,
                FactoryName = b.Factory != null ? b.Factory.OfficialFactoryName : null,
                BatchStatus = b.BatchStatus.ToString(),
                SupplyChainStage = b.SupplyChainStage.ToString(),
                LastUpdated = b.UpdatedAt
            }).ToListAsync();

        return Ok(new OverviewDto
        {
            Cards = cards,
            RecentRegistrationRequests = recentRequests,
            RecentAlerts = recentAlerts,
            RecentBatchActivity = recentBatches
        });
    }
}

