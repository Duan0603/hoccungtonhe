using EduVN.Domain.Enums;

namespace EduVN.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long OrderCode { get; set; } // PayOS requires safe long
    public Guid StudentId { get; set; }
    public User? Student { get; set; }
    public Guid CourseId { get; set; }
    public Course? Course { get; set; }
    public int Amount { get; set; }
    public string Status { get; set; } = "PENDING"; // PENDING, PAID, CANCELLED
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public string? PayOSOrderId { get; set; }
    public string? PayOSTransactionId { get; set; }
}
