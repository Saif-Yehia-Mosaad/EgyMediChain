namespace EgyMediChain.Api.Dtos;

// New file - put it next to the other Dto classes (same folder/namespace as
// RegistrationRequestListItemDto etc.). Used by the public registration wizard submission.
public class SubmitRegistrationRequestDto
{
    // Account (Step 1)
    public string? RepresentativeName { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? NationalId { get; set; }
    public string? Password { get; set; }

    // "Factory" | "Warehouse" | "Pharmacy"
    public string? EntityType { get; set; }

    // Address (common)
    public string? Governorate { get; set; }
    public string? City { get; set; }
    public string? DistrictArea { get; set; }
    public string? FullAddress { get; set; }

    // Factory
    public string? OfficialFactoryName { get; set; }
    public string? LegalCompanyName { get; set; }
    public string? DosageFormsProduced { get; set; }
    public string? FactoryLicenseNumber { get; set; }
    public string? TechnicalOperatingLicenseNumber { get; set; }
    public string? CommercialRegistrationNumber { get; set; }
    public string? TaxCardNumber { get; set; }
    public bool? HasQualityControlLab { get; set; }
    public bool? HasFinishedGoodsStore { get; set; }

    // Warehouse
    public string? OfficialWarehouseName { get; set; }
    public string? WarehouseType { get; set; }
    public string? WarehouseLicenseNumber { get; set; }
    public bool? HasDeliveryService { get; set; }

    // Pharmacy
    public string? OfficialPharmacyName { get; set; }
    public string? PharmacyType { get; set; }
    public int? DefaultWarehouseId { get; set; }
    public string? PharmacyLicenseNumber { get; set; }
    public string? PharmacistSyndicateId { get; set; }

    // Shared licensing / capabilities
    public DateTime? LicenseIssueDate { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public bool? HasColdStorage { get; set; }
    public bool? HasQuarantineArea { get; set; }

    // Declaration checkboxes (Step 5)
    public bool ConfirmInfoCorrect { get; set; }
    public bool AgreeToInspection { get; set; }
}
