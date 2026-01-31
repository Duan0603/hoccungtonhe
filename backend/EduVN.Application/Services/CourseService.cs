using EduVN.Application.DTOs.Courses;
using EduVN.Application.Interfaces;
using EduVN.Domain.Entities;

namespace EduVN.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IFileUploadService _fileUploadService;

    public CourseService(
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        IFileUploadService fileUploadService)
    {
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
        _fileUploadService = fileUploadService;
    }

    public async Task<(IEnumerable<CourseDto> Items, int TotalCount)> GetCoursesAsync(CourseFilterDto filter)
    {
        var (items, totalCount) = await _courseRepository.GetAsync(
            filter.Search,
            filter.Subject,
            filter.Grade,
            filter.MinPrice,
            filter.MaxPrice,
            filter.Page,
            filter.PageSize
        );

        var dtos = items.Select(c => new CourseDto(
            c.Id,
            c.Title,
            c.Description,
            c.Price,
            c.Subject,
            c.Grade,
            c.ThumbnailUrl,
            c.IsPublished,
            c.CreatedAt,
            c.UpdatedAt,
            c.InstructorId,
            c.Instructor?.FullName ?? "Unknown"
        ));

        return (dtos, totalCount);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) return null;

        return new CourseDto(
            course.Id,
            course.Title,
            course.Description,
            course.Price,
            course.Subject,
            course.Grade,
            course.ThumbnailUrl,
            course.IsPublished,
            course.CreatedAt,
            course.UpdatedAt,
            course.InstructorId,
            course.Instructor?.FullName ?? "Unknown"
        );
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto request, Guid userId, string userName)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Subject = request.Subject,
            Grade = request.Grade,
            InstructorId = userId,
            IsPublished = false // Default draft
        };

        await _courseRepository.AddAsync(course);

        return new CourseDto(
            course.Id,
            course.Title,
            course.Description,
            course.Price,
            course.Subject,
            course.Grade,
            course.ThumbnailUrl,
            course.IsPublished,
            course.CreatedAt,
            course.UpdatedAt,
            course.InstructorId,
            userName
        );
    }

    public async Task<bool> UpdateCourseAsync(Guid id, UpdateCourseDto request, Guid userId, string userRole)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) return false;

        // Check permission
        if (userRole != "Admin" && course.InstructorId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this course.");
        }

        // Update fields if provided
        if (request.Title != null) course.Title = request.Title;
        if (request.Description != null) course.Description = request.Description;
        if (request.Price.HasValue) course.Price = request.Price.Value;
        if (request.Subject != null) course.Subject = request.Subject;
        if (request.Grade.HasValue) course.Grade = request.Grade.Value;
        if (request.IsPublished.HasValue) course.IsPublished = request.IsPublished.Value;

        course.UpdatedAt = DateTime.UtcNow;

        await _courseRepository.UpdateAsync(course);
        return true;
    }

    public async Task<bool> DeleteCourseAsync(Guid id, Guid userId, string userRole)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) return false;

        // Check permission
        if (userRole != "Admin" && course.InstructorId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this course.");
        }

        // Get all lessons for this course and delete their files from Cloudinary
        var lessons = await _lessonRepository.GetByCourseIdAsync(id);
        foreach (var lesson in lessons)
        {
            if (!string.IsNullOrEmpty(lesson.VideoUrl))
            {
                await _fileUploadService.DeleteFileAsync(lesson.VideoUrl);
            }
            if (!string.IsNullOrEmpty(lesson.DocumentUrl))
            {
                await _fileUploadService.DeleteFileAsync(lesson.DocumentUrl);
            }
        }

        // Delete course thumbnail if exists
        if (!string.IsNullOrEmpty(course.ThumbnailUrl))
        {
            await _fileUploadService.DeleteFileAsync(course.ThumbnailUrl);
        }

        // Delete course (lessons will be cascade deleted by database)
        await _courseRepository.DeleteAsync(course);
        return true;
    }

    public async Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(Guid instructorId)
    {
        var courses = await _courseRepository.GetByInstructorAsync(instructorId);

        return courses.Select(c => new CourseDto(
            c.Id,
            c.Title,
            c.Description,
            c.Price,
            c.Subject,
            c.Grade,
            c.ThumbnailUrl,
            c.IsPublished,
            c.CreatedAt,
            c.UpdatedAt,
            c.InstructorId,
            c.Instructor?.FullName ?? "Me"
        ));
    }
}
