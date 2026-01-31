namespace EduVN.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public required string Answers { get; set; } // JSON string
    public decimal? Score { get; set; }
    public string? AIFeedback { get; set; } // JSON string: {errors: [], solution: "", explanation: ""}
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Assignment Assignment { get; set; } = null!;
    public User Student { get; set; } = null!;
}
