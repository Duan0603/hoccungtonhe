using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EduVN.Application.Interfaces;
using EduVN.Application.Settings;
using Microsoft.Extensions.Options;

namespace EduVN.Infrastructure.Services;

public class CloudinaryUploadService : IFileUploadService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryUploadService(IOptions<CloudinarySettings> settings)
    {
        var account = new Account(
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder)
    {
        // Determine if it's a video or image based on extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var isVideo = extension is ".mp4" or ".mov" or ".avi" or ".webm" or ".mkv";

        if (isVideo)
        {
            return await UploadVideoAsync(fileStream, fileName, folder);
        }
        else
        {
            return await UploadImageAsync(fileStream, fileName, folder);
        }
    }

    private async Task<string> UploadVideoAsync(Stream fileStream, string fileName, string folder)
    {
        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = $"eduvn/{folder}",
            PublicId = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString("N")[..8],
            Overwrite = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
        {
            throw new Exception($"Cloudinary upload failed: {result.Error.Message}");
        }

        return result.SecureUrl.ToString();
    }

    private async Task<string> UploadImageAsync(Stream fileStream, string fileName, string folder)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = $"eduvn/{folder}",
            PublicId = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString("N")[..8],
            Overwrite = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
        {
            throw new Exception($"Cloudinary upload failed: {result.Error.Message}");
        }

        return result.SecureUrl.ToString();
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        // Skip if not a Cloudinary URL
        if (!fileUrl.Contains("cloudinary.com")) return;

        try
        {
            // Extract publicId from Cloudinary URL
            // URL format: https://res.cloudinary.com/{cloud}/video/upload/v{version}/{folder}/{publicId}.{ext}
            // or: https://res.cloudinary.com/{cloud}/image/upload/v{version}/{folder}/{publicId}.{ext}
            var uri = new Uri(fileUrl);
            var pathSegments = uri.AbsolutePath.Split('/');
            
            // Find the resource type (video or image)
            var resourceType = ResourceType.Image;
            var uploadIndex = Array.IndexOf(pathSegments, "upload");
            if (uploadIndex > 0 && pathSegments[uploadIndex - 1] == "video")
            {
                resourceType = ResourceType.Video;
            }

            // Get everything after 'upload/v{version}/' as the publicId (without extension)
            // Skip: "", cloud, video/image, upload, v{version}
            if (uploadIndex >= 0 && uploadIndex + 2 < pathSegments.Length)
            {
                var publicIdParts = pathSegments.Skip(uploadIndex + 2).ToArray();
                var publicIdWithExt = string.Join("/", publicIdParts);
                var publicId = Path.ChangeExtension(publicIdWithExt, null); // Remove extension

                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = resourceType
                };

                await _cloudinary.DestroyAsync(deleteParams);
            }
        }
        catch (Exception)
        {
            // Log error but don't throw - deletion failure shouldn't break the flow
            // In production, you'd want proper logging here
        }
    }
}
