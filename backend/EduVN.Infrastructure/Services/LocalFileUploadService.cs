using EduVN.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace EduVN.Infrastructure.Services;

public class LocalFileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File is empty");
        }

        // Validate folder name to prevent directory traversal
        if (folderName.Any(c => Path.GetInvalidFileNameChars().Contains(c)) || folderName.Contains(".."))
        {
             throw new ArgumentException("Invalid folder name");
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folderName);
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName.Replace(" ", "_")}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var file = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(file);
        }

        // Return URL relative to server root
        return $"/uploads/{folderName}/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return Task.CompletedTask;

        // Convert URL to file path
        var relativePath = fileUrl.TrimStart('/');
        var filePath = Path.Combine(_environment.WebRootPath, relativePath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
