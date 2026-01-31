using EduVN.Domain.Enums;

namespace EduVN.Domain.Entities;

public class Assignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public required string Title { get; set; }
    public AssignmentType Type { get; set; }
    public required string Content { get; set; } // JSON string for questions
    public string? Answers { get; set; } // JSON string for correct answers
    public decimal MaxScore { get; set; } = 10;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Course Course { get; set; } = null!;
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
