namespace EduVN.Domain.Entities;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InstructorId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Subject { get; set; } // "Toán", "Lý", "Hóa", etc.
    public int Grade { get; set; } // 10, 11, 12
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; } = 0;
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User Instructor { get; set; } = null!;
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
