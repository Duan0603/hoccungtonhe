namespace EduVN.Domain.Enums;

public enum UserRole
{
    Student,
    Instructor,
    Admin
}

public enum UserStatus
{
    Pending,
    Approved,
    Rejected,
    Blocked
}

public enum AssignmentType
{
    MultipleChoice,
    Essay
}

public enum OrderStatus
{
    Pending,
    Paid,
    Failed,
    Cancelled
}

public enum EnrollmentType
{
    Paid,
    Free,
    Manual
}
