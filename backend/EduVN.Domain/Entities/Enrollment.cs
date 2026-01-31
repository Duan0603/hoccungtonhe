using EduVN.Domain.Enums;

namespace EduVN.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public EnrollmentType EnrollmentType { get; set; } = EnrollmentType.Paid;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
