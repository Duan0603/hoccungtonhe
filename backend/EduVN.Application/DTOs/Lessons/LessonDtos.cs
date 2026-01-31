using System.ComponentModel.DataAnnotations;

namespace EduVN.Application.DTOs.Lessons;

public record LessonDto(
    Guid Id,
    Guid CourseId,
    string Title,
    string? VideoUrl,
    string? DocumentUrl,
    int OrderIndex,
    int Duration,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateLessonDto(
    [Required] Guid CourseId,
    [Required][MaxLength(255)] string Title,
    string? VideoUrl,
    string? DocumentUrl,
    int OrderIndex = 0,
    int Duration = 0
);

public record UpdateLessonDto(
    [MaxLength(255)] string? Title,
    string? VideoUrl,
    string? DocumentUrl,
    int? OrderIndex,
    int? Duration,
    bool? IsPublished
);

public record ReorderLessonDto(
    Guid Id,
    int NewIndex
);
