using System.Security.Claims;
using EduVN.Application.DTOs.Lessons;
using EduVN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduVN.API.Controllers;

[ApiController]
[Route("api")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet("courses/{courseId}/lessons")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByCourse(Guid courseId)
    {
        var dtos = await _lessonService.GetLessonsByCourseIdAsync(courseId);
        return Ok(dtos);
    }

    [HttpGet("lessons/{id}")]
    public async Task<ActionResult<LessonDto>> GetLesson(Guid id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);

        if (lesson == null)
        {
            return NotFound(new { message = "Lesson not found" });
        }

        return Ok(lesson);
    }

    [HttpPost("lessons")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<LessonDto>> CreateLesson(CreateLessonDto request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

        try
        {
            var lesson = await _lessonService.CreateLessonAsync(request, userId, role);
            return CreatedAtAction(nameof(GetLesson), new { id = lesson.Id }, lesson);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("lessons/{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> UpdateLesson(Guid id, UpdateLessonDto request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

        try
        {
            var success = await _lessonService.UpdateLessonAsync(id, request, userId, role);
            if (!success) return NotFound(new { message = "Lesson not found" });

            return Ok(new { message = "Lesson updated successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("lessons/{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> DeleteLesson(Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

        try
        {
            var success = await _lessonService.DeleteLessonAsync(id, userId, role);
            if (!success) return NotFound(new { message = "Lesson not found" });

            return Ok(new { message = "Lesson deleted successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
