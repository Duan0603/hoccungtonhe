using EduVN.Application.Interfaces;
using EduVN.Domain.Entities;
using EduVN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduVN.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Lessons
            .Where(x => x.CourseId == courseId)
            .OrderBy(x => x.OrderIndex)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Lesson lesson)
    {
        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Lesson lesson)
    {
        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
    }
}
