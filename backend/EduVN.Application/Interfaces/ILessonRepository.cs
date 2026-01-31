using EduVN.Domain.Entities;

namespace EduVN.Application.Interfaces;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(Guid id);
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task AddAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(Lesson lesson);
}
