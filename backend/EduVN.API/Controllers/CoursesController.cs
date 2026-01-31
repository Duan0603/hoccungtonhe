using System.Security.Claims;
using EduVN.Application.DTOs.Courses;
using EduVN.Application.Interfaces;
using EduVN.Domain.Entities;
using EduVN.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduVN.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IFileUploadService _fileUploadService;

    public CoursesController(
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        IFileUploadService fileUploadService)
    {
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
        _fileUploadService = fileUploadService;
    }

    [HttpGet]
    public async Task<ActionResult> GetCourses([FromQuery] CourseFilterDto filter)
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

        return Ok(new
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound(new { message = "Course not found" });
        }

        return Ok(new CourseDto(
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
        ));
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto request)
    {
        // Get current user ID from JWT
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

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

        // Fetch again to include instructor info if needed, or just return basic
        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, new CourseDto(
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
            User.FindFirst(ClaimTypes.Name)?.Value ?? "Me"
        ));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseDto request)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound(new { message = "Course not found" });
        }

        // Check permission (only owner or admin)
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role != "Admin" && course.InstructorId.ToString() != userIdString)
        {
            return Forbid();
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

        return Ok(new { message = "Course updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound(new { message = "Course not found" });
        }

        // Check permission
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role != "Admin" && course.InstructorId.ToString() != userIdString)
        {
            return Forbid();
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

        return Ok(new { message = "Course and all associated files deleted successfully" });
    }

    [HttpGet("my-courses")]
    [Authorize(Roles = "Instructor")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetMyCourses()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var courses = await _courseRepository.GetByInstructorAsync(userId);
        
        var dtos = courses.Select(c => new CourseDto(
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
            c.Instructor?.FullName ?? "Me" // Optimization: Instructor might be null if not included, but repo includes it? Repo GetByInstructor doesn't include Instructor nav prop in code above, let's fix or assume "Me"
        ));

        return Ok(dtos);
    }
}
