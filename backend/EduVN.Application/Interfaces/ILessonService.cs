using EduVN.Application.DTOs.Lessons;

namespace EduVN.Application.Interfaces;

public interface ILessonService
{
    Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId);
    Task<LessonDto?> GetLessonByIdAsync(Guid id);
    Task<LessonDto> CreateLessonAsync(CreateLessonDto request, Guid userId, string userRole);
    Task<bool> UpdateLessonAsync(Guid id, UpdateLessonDto request, Guid userId, string userRole);
    Task<bool> DeleteLessonAsync(Guid id, Guid userId, string userRole);
}
