using EduVN.Domain.Enums;

namespace EduVN.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? PayOSOrderId { get; set; }
    public string? PayOSTransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
