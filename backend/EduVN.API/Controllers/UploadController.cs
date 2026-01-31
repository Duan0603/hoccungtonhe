using EduVN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduVN.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Instructor,Admin")]
public class UploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx", ".mp4", ".mov", ".avi", ".webm", ".mkv" };
    private readonly long _maxFileSize = 500 * 1024 * 1024; // 500MB

    public UploadController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    [HttpPost]
    [RequestSizeLimit(500 * 1024 * 1024)] // 500MB
    [RequestFormLimits(MultipartBodyLengthLimit = 500 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string folder = "common")
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded" });
        }

        if (file.Length > _maxFileSize)
        {
            return BadRequest(new { message = "File size exceeds 500MB limit" });
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            return BadRequest(new { message = $"Invalid file type. Allowed: {string.Join(", ", _allowedExtensions)}" });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var fileUrl = await _fileUploadService.UploadFileAsync(stream, file.FileName, folder);
            
            // Cloudinary returns full URL, local returns relative URL
            // Return both for compatibility
            var isCloudUrl = fileUrl.StartsWith("http");
            
            return Ok(new { 
                url = fileUrl, 
                relativeUrl = isCloudUrl ? fileUrl : fileUrl // For cloud, url = relativeUrl
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Upload failed: {ex.Message}" });
        }
    }
}
