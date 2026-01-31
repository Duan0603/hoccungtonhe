using EduVN.Application.DTOs.Courses;

namespace EduVN.Application.Interfaces;

public interface ICourseService
{
    Task<(IEnumerable<CourseDto> Items, int TotalCount)> GetCoursesAsync(CourseFilterDto filter);
    Task<CourseDto?> GetCourseByIdAsync(Guid id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto request, Guid userId, string userName);
    Task<bool> UpdateCourseAsync(Guid id, UpdateCourseDto request, Guid userId, string userRole);
    Task<bool> DeleteCourseAsync(Guid id, Guid userId, string userRole);
    Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(Guid instructorId);
}
