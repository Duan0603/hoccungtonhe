namespace EduVN.Application.Interfaces;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName);
    Task DeleteFileAsync(string filePath);
}
