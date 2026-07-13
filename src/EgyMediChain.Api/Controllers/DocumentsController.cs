using EgyMediChain.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace EgyMediChain.Api.Controllers;

// Documents can contain National ID copies etc., so downloads go through an authenticated
// endpoint rather than only the raw /uploads/... static URL (that static URL still works too,
// since Program.cs has UseStaticFiles(), but this is the one the frontend should actually use).
[ApiController]
[Route("api/documents")]
[Authorize(Roles = "SuperAdmin,MinistryAdmin,MinistryViewer")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public DocumentsController(AppDbContext db) => _db = db;

    [HttpGet("{documentId:int}/download")]
    public async Task<IActionResult> Download(int documentId)
    {
        var doc = await _db.EntityDocuments.FirstOrDefaultAsync(d => d.Id == documentId);
        if (doc == null || string.IsNullOrWhiteSpace(doc.FileUrl))
            return NotFound(new { message = "Document not found." });

        // FileUrl looks like "/uploads/REQ-2026-1234/xxx.pdf" - map it back to the physical path
        // under wwwroot (see RegistrationRequestsController.Submit, where it's written).
        var relativePath = doc.FileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (!System.IO.File.Exists(fullPath))
            return NotFound(new { message = "File is missing on disk." });

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullPath, out var contentType))
            contentType = "application/octet-stream";

        var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        var downloadName = doc.FileName ?? Path.GetFileName(fullPath);
        return File(bytes, contentType, downloadName);
    }
}
