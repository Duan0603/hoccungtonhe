using System.Security.Claims;
using EduVN.Application.DTOs.Lessons;
using EduVN.Application.Interfaces;
using EduVN.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduVN.API.Controllers;

[ApiController]
[Route("api")]
public class LessonsController : ControllerBase
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IFileUploadService _fileUploadService;

    public LessonsController(
        ILessonRepository lessonRepository, 
        ICourseRepository courseRepository,
        IFileUploadService fileUploadService)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _fileUploadService = fileUploadService;
    }

    [HttpGet("courses/{courseId}/lessons")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByCourse(Guid courseId)
    {
        // Publicly viewable lessons? Or minimal info?
        // Usually full info is fine, video/doc access might be restricted to enrolled users later.
        // For now, allow viewing list.
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        
        var dtos = lessons.Select(l => new LessonDto(
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

        return Ok(dtos);
    }

    [HttpGet("lessons/{id}")]
    public async Task<ActionResult<LessonDto>> GetLesson(Guid id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson == null)
        {
            return NotFound(new { message = "Lesson not found" });
        }

        return Ok(new LessonDto(
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
        ));
    }

    [HttpPost("lessons")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<LessonDto>> CreateLesson(CreateLessonDto request)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return NotFound(new { message = "Course not found" });
        }

        // Check ownership
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role != "Admin" && course.InstructorId.ToString() != userIdString)
        {
            return Forbid();
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

        return CreatedAtAction(nameof(GetLesson), new { id = lesson.Id }, new LessonDto(
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
        ));
    }

    [HttpPut("lessons/{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> UpdateLesson(Guid id, UpdateLessonDto request)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson == null)
        {
            return NotFound(new { message = "Lesson not found" });
        }

        // Check ownership via Course
        // Repo GetByIdAsync includes Course
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role != "Admin" && lesson.Course.InstructorId.ToString() != userIdString)
        {
            return Forbid();
        }

        if (request.Title != null) lesson.Title = request.Title;
        if (request.VideoUrl != null) lesson.VideoUrl = request.VideoUrl;
        if (request.DocumentUrl != null) lesson.DocumentUrl = request.DocumentUrl;
        if (request.OrderIndex.HasValue) lesson.OrderIndex = request.OrderIndex.Value;
        if (request.Duration.HasValue) lesson.Duration = request.Duration.Value;
        if (request.IsPublished.HasValue) lesson.IsPublished = request.IsPublished.Value;

        lesson.UpdatedAt = DateTime.UtcNow;

        await _lessonRepository.UpdateAsync(lesson);

        return Ok(new { message = "Lesson updated successfully" });
    }

    [HttpDelete("lessons/{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> DeleteLesson(Guid id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson == null)
        {
            return NotFound(new { message = "Lesson not found" });
        }

        // Check ownership via Course
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role != "Admin" && (lesson.Course == null || lesson.Course.InstructorId.ToString() != userIdString))
        {
            if (lesson.Course != null) 
                 return Forbid();
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

        return Ok(new { message = "Lesson deleted successfully" });
    }
}
