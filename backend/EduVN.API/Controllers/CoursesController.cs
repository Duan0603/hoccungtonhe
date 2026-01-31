using System.Security.Claims;
using EduVN.Application.DTOs.Courses;
using EduVN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduVN.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult> GetCourses([FromQuery] CourseFilterDto filter)
    {
        var (items, totalCount) = await _courseService.GetCoursesAsync(filter);

        return Ok(new
        {
            Data = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);

        if (course == null)
        {
            return NotFound(new { message = "Course not found" });
        }

        return Ok(course);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Me";
        var course = await _courseService.CreateCourseAsync(request, userId, userName);

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseDto request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

        try
        {
            var success = await _courseService.UpdateCourseAsync(id, request, userId, role);
            if (!success) return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course updated successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

        try
        {
            var success = await _courseService.DeleteCourseAsync(id, userId, role);
            if (!success) return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course and all associated files deleted successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
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

        var courses = await _courseService.GetCoursesByInstructorAsync(userId);
        return Ok(courses);
    }
}
