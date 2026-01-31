using EduVN.Application.Interfaces;
using EduVN.Domain.Entities;
using EduVN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduVN.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(IEnumerable<Course> Items, int TotalCount)> GetAsync(
        string? search,
        string? subject,
        int? grade,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize)
    {
        var query = _context.Courses
            .Include(c => c.Instructor)
            .AsNoTracking() // Performance optimization for read-only
            .AsQueryable();

        // Filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(searchLower) || 
                                     c.Description.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(subject))
        {
            query = query.Where(c => c.Subject.ToLower() == subject.ToLower());
        }

        if (grade.HasValue)
        {
            query = query.Where(c => c.Grade == grade.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(c => c.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(c => c.Price <= maxPrice.Value);
        }

        // Only show published courses for public view? 
        // Logic handled in Controller or Service usually, but Repository should just return data matching criteria.
        // Assuming this is a general search, we might want to filter IsPublished = true unless strictly requested otherwise,
        // but to keep Repository generic, I'll assume the caller filters IsPublished if needed, 
        // OR filtering happens here. For now, let's keep it open and let Controller decide.
        
        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Course>> GetByInstructorAsync(Guid instructorId)
    {
        return await _context.Courses
            .Where(c => c.InstructorId == instructorId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course course)
    {
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Courses.AnyAsync(c => c.Id == id);
    }
}
