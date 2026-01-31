using EduVN.Domain.Entities;

namespace EduVN.Application.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Course> Items, int TotalCount)> GetAsync(
        string? search, 
        string? subject, 
        int? grade, 
        decimal? minPrice, 
        decimal? maxPrice, 
        int page, 
        int pageSize);
    Task<IEnumerable<Course>> GetByInstructorAsync(Guid instructorId);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(Course course);
    Task<bool> ExistsAsync(Guid id);
}
