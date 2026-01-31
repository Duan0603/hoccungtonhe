using System.ComponentModel.DataAnnotations;

namespace EduVN.Application.DTOs.Courses;

public record CourseDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    string Subject,
    int Grade,
    string? ThumbnailUrl,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid InstructorId,
    string InstructorName
);

public record CreateCourseDto(
    [Required][MaxLength(255)] string Title,
    string Description,
    [Range(0, 100000000)] decimal Price,
    [Required][MaxLength(100)] string Subject,
    [Range(10, 12)] int Grade
);

public record UpdateCourseDto(
    [MaxLength(255)] string? Title,
    string? Description,
    [Range(0, 100000000)] decimal? Price,
    [MaxLength(100)] string? Subject,
    [Range(10, 12)] int? Grade,
    bool? IsPublished
);

public record CourseFilterDto(
    string? Search,
    string? Subject,
    int? Grade,
    decimal? MinPrice,
    decimal? MaxPrice,
    int Page = 1,
    int PageSize = 10
);
