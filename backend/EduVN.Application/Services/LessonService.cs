using EduVN.Application.DTOs.Lessons;
using EduVN.Application.Interfaces;
using EduVN.Domain.Entities;

namespace EduVN.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IFileUploadService _fileUploadService;

    public LessonService(
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IFileUploadService fileUploadService)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _fileUploadService = fileUploadService;
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        
        return lessons.Select(l => new LessonDto(
            l.Id,
            l.CourseId,
            l.Title,
            l.VideoUrl,
            l.DocumentUrl,
            l.OrderIndex,
            l.Duration,
            l.IsPublished,
            l.CreatedAt,
            l.UpdatedAt
        ));
    }

    public async Task<LessonDto?> GetLessonByIdAsync(Guid id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null) return null;

        return new LessonDto(
            lesson.Id,
            lesson.CourseId,
            lesson.Title,
            lesson.VideoUrl,
            lesson.DocumentUrl,
            lesson.OrderIndex,
            lesson.Duration,
            lesson.IsPublished,
            lesson.CreatedAt,
            lesson.UpdatedAt
        );
    }

    public async Task<LessonDto> CreateLessonAsync(CreateLessonDto request, Guid userId, string userRole)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            throw new KeyNotFoundException("Course not found");
        }

        // Check ownership
        if (userRole != "Admin" && course.InstructorId != userId)
        {
             throw new UnauthorizedAccessException("You do not have permission to add lessons to this course.");
        }

        var lesson = new Lesson
        {
            CourseId = request.CourseId,
            Title = request.Title,
            VideoUrl = request.VideoUrl,
            DocumentUrl = request.DocumentUrl,
            OrderIndex = request.OrderIndex,
            Duration = request.Duration,
            IsPublished = true // Default publish
        };

        await _lessonRepository.AddAsync(lesson);

        return new LessonDto(
            lesson.Id,
            lesson.CourseId,
            lesson.Title,
            lesson.VideoUrl,
            lesson.DocumentUrl,
            lesson.OrderIndex,
            lesson.Duration,
            lesson.IsPublished,
            lesson.CreatedAt,
            lesson.UpdatedAt
        );
    }

    public async Task<bool> UpdateLessonAsync(Guid id, UpdateLessonDto request, Guid userId, string userRole)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null) return false;

        // Check ownership via Course
        if (userRole != "Admin" && (lesson.Course == null || lesson.Course.InstructorId != userId))
        {
            throw new UnauthorizedAccessException("You do not have permission to update this lesson.");
        }

        if (request.Title != null) lesson.Title = request.Title;
        if (request.VideoUrl != null) lesson.VideoUrl = request.VideoUrl;
        if (request.DocumentUrl != null) lesson.DocumentUrl = request.DocumentUrl;
        if (request.OrderIndex.HasValue) lesson.OrderIndex = request.OrderIndex.Value;
        if (request.Duration.HasValue) lesson.Duration = request.Duration.Value;
        if (request.IsPublished.HasValue) lesson.IsPublished = request.IsPublished.Value;

        lesson.UpdatedAt = DateTime.UtcNow;

        await _lessonRepository.UpdateAsync(lesson);
        return true;
    }

    public async Task<bool> DeleteLessonAsync(Guid id, Guid userId, string userRole)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null) return false;

        // Check ownership via Course
        if (userRole != "Admin" && (lesson.Course == null || lesson.Course.InstructorId != userId))
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this lesson.");
        }

        // Delete files from Cloudinary before deleting lesson from DB
        if (!string.IsNullOrEmpty(lesson.VideoUrl))
        {
            await _fileUploadService.DeleteFileAsync(lesson.VideoUrl);
        }
        if (!string.IsNullOrEmpty(lesson.DocumentUrl))
        {
            await _fileUploadService.DeleteFileAsync(lesson.DocumentUrl);
        }

        await _lessonRepository.DeleteAsync(lesson);
        return true;
    }
}
