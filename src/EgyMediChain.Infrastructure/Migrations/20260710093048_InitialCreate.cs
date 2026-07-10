using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyMediChain.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityLicenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<int>(type: "int", nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    LicenseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityLicenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Factories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FactoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialFactoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DosageFormsProduced = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Governorate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistrictArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FactoryLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechnicalOperatingLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommercialRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasQualityControlLab = table.Column<bool>(type: "bit", nullable: true),
                    HasFinishedGoodsStore = table.Column<bool>(type: "bit", nullable: true),
                    HasColdStorage = table.Column<bool>(type: "bit", nullable: true),
                    HasQuarantineArea = table.Column<bool>(type: "bit", nullable: true),
                    EstablishedYear = table.Column<int>(type: "int", nullable: true),
                    TotalProductionLines = table.Column<int>(type: "int", nullable: true),
                    MainProductionTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualityCertificates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationRequestNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationSubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FactoryStatus = table.Column<int>(type: "int", nullable: true),
                    TotalBatches = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicineProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GTIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DosageForm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiresColdChain = table.Column<bool>(type: "bit", nullable: true),
                    ProductStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: true),
                    EntityType = table.Column<int>(type: "int", nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Governorate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistrictArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasColdStorage = table.Column<bool>(type: "bit", nullable: true),
                    HasQuarantineArea = table.Column<bool>(type: "bit", nullable: true),
                    HasDeliveryService = table.Column<bool>(type: "bit", nullable: true),
                    WarehouseStatus = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicineProductId = table.Column<int>(type: "int", nullable: true),
                    FactoryId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<long>(type: "bigint", nullable: true),
                    ManufacturingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BatchStatus = table.Column<int>(type: "int", nullable: true),
                    SupplyChainStage = table.Column<int>(type: "int", nullable: true),
                    CurrentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalUnitCodes = table.Column<long>(type: "bigint", nullable: true),
                    GeneratedUnitCodes = table.Column<long>(type: "bigint", nullable: true),
                    InWarehouseUnitCodes = table.Column<long>(type: "bigint", nullable: true),
                    InPharmacyUnitCodes = table.Column<long>(type: "bigint", nullable: true),
                    SuspiciousUnitCodes = table.Column<long>(type: "bigint", nullable: true),
                    BlockedUnitCodes = table.Column<long>(type: "bigint", nullable: true),
                    RecalledUnitCodes = table.Column<long>(type: "bigint", nullable: true),
                    ScanCountTotal = table.Column<long>(type: "bigint", nullable: true),
                    OpenAlertsCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batches_Factories_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Batches_MedicineProducts_MedicineProductId",
                        column: x => x.MedicineProductId,
                        principalTable: "MedicineProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AuthRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemUserId = table.Column<int>(type: "int", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthRefreshTokens_SystemUsers_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialPharmacyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PharmacyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Governorate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistrictArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultWarehouseId = table.Column<int>(type: "int", nullable: true),
                    HasColdStorage = table.Column<bool>(type: "bit", nullable: true),
                    PharmacyLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PharmacistSyndicateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PharmacyStatus = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pharmacies_Warehouses_DefaultWarehouseId",
                        column: x => x.DefaultWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UnitCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitCodeValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GTIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeValueHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    UnitStatus = table.Column<int>(type: "int", nullable: true),
                    CurrentHolderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentHolderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScanCount = table.Column<int>(type: "int", nullable: true),
                    FirstScannedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitCodes_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SourceFactoryId = table.Column<int>(type: "int", nullable: true),
                    SourceWarehouseId = table.Column<int>(type: "int", nullable: true),
                    DestinationWarehouseId = table.Column<int>(type: "int", nullable: true),
                    DestinationPharmacyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Factories_SourceFactoryId",
                        column: x => x.SourceFactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLogs_Pharmacies_DestinationPharmacyId",
                        column: x => x.DestinationPharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLogs_SystemUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "SystemUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Warehouses_DestinationWarehouseId",
                        column: x => x.DestinationWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLogs_Warehouses_SourceWarehouseId",
                        column: x => x.SourceWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    HolderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HolderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    PharmacyId = table.Column<int>(type: "int", nullable: true),
                    TotalReceivedQuantity = table.Column<long>(type: "bigint", nullable: true),
                    AvailableQuantity = table.Column<long>(type: "bigint", nullable: true),
                    ReservedQuantity = table.Column<long>(type: "bigint", nullable: true),
                    QuarantinedQuantity = table.Column<long>(type: "bigint", nullable: true),
                    InventoryStatus = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryStocks_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InventoryStocks_Pharmacies_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InventoryStocks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityType = table.Column<int>(type: "int", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepresentativeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    DocumentsOverallStatus = table.Column<int>(type: "int", nullable: true),
                    RegistrationStatus = table.Column<int>(type: "int", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FactoryId = table.Column<int>(type: "int", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    PharmacyId = table.Column<int>(type: "int", nullable: true),
                    SystemUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationRequests_Factories_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrationRequests_Pharmacies_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrationRequests_SystemUsers_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrationRequests_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    ShipmentType = table.Column<int>(type: "int", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceFactoryId = table.Column<int>(type: "int", nullable: true),
                    SourceWarehouseId = table.Column<int>(type: "int", nullable: true),
                    DestinationWarehouseId = table.Column<int>(type: "int", nullable: true),
                    DestinationPharmacyId = table.Column<int>(type: "int", nullable: true),
                    ExpectedQuantity = table.Column<long>(type: "bigint", nullable: true),
                    ReceivedQuantity = table.Column<long>(type: "bigint", nullable: true),
                    ShipmentStatus = table.Column<int>(type: "int", nullable: true),
                    RequiresColdChain = table.Column<bool>(type: "bit", nullable: true),
                    DispatchedByUserId = table.Column<int>(type: "int", nullable: true),
                    ReceivedByUserId = table.Column<int>(type: "int", nullable: true),
                    InspectionResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_Factories_SourceFactoryId",
                        column: x => x.SourceFactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_Pharmacies_DestinationPharmacyId",
                        column: x => x.DestinationPharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_Warehouses_DestinationWarehouseId",
                        column: x => x.DestinationWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_Warehouses_SourceWarehouseId",
                        column: x => x.SourceWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PublicVerificationScans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScannedGTIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScannedSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScannedBatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitCodeId = table.Column<int>(type: "int", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationResult = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Governorate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScannedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicVerificationScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicVerificationScans_UnitCodes_UnitCodeId",
                        column: x => x.UnitCodeId,
                        principalTable: "UnitCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EntityDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentStatus = table.Column<int>(type: "int", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityDocuments_RegistrationRequests_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlertType = table.Column<int>(type: "int", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: true),
                    EntityType = table.Column<int>(type: "int", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    ShipmentId = table.Column<int>(type: "int", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlertStatus = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Alerts_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_BatchId",
                table: "Alerts",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ShipmentId",
                table: "Alerts",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_DestinationPharmacyId",
                table: "AuditLogs",
                column: "DestinationPharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_DestinationWarehouseId",
                table: "AuditLogs",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_SourceFactoryId",
                table: "AuditLogs",
                column: "SourceFactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_SourceWarehouseId",
                table: "AuditLogs",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRefreshTokens_SystemUserId",
                table: "AuthRefreshTokens",
                column: "SystemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_FactoryId",
                table: "Batches",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_MedicineProductId",
                table: "Batches",
                column: "MedicineProductId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityDocuments_RegistrationRequestId",
                table: "EntityDocuments",
                column: "RegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_BatchId",
                table: "InventoryStocks",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_PharmacyId",
                table: "InventoryStocks",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_WarehouseId",
                table: "InventoryStocks",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_DefaultWarehouseId",
                table: "Pharmacies",
                column: "DefaultWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicVerificationScans_UnitCodeId",
                table: "PublicVerificationScans",
                column: "UnitCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequests_FactoryId",
                table: "RegistrationRequests",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequests_PharmacyId",
                table: "RegistrationRequests",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequests_SystemUserId",
                table: "RegistrationRequests",
                column: "SystemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequests_WarehouseId",
                table: "RegistrationRequests",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_BatchId",
                table: "Shipments",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DestinationPharmacyId",
                table: "Shipments",
                column: "DestinationPharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DestinationWarehouseId",
                table: "Shipments",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_SourceFactoryId",
                table: "Shipments",
                column: "SourceFactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_SourceWarehouseId",
                table: "Shipments",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitCodes_BatchId",
                table: "UnitCodes",
                column: "BatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "AuthRefreshTokens");

            migrationBuilder.DropTable(
                name: "EntityDocuments");

            migrationBuilder.DropTable(
                name: "EntityLicenses");

            migrationBuilder.DropTable(
                name: "InventoryStocks");

            migrationBuilder.DropTable(
                name: "PublicVerificationScans");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "RegistrationRequests");

            migrationBuilder.DropTable(
                name: "UnitCodes");

            migrationBuilder.DropTable(
                name: "Pharmacies");

            migrationBuilder.DropTable(
                name: "SystemUsers");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Factories");

            migrationBuilder.DropTable(
                name: "MedicineProducts");
        }
    }
}
