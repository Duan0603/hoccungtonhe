using EduVN.Domain.Entities;
using EduVN.Domain.Enums;
using EduVN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduVN.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed Admin User if not exists
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            var adminUser = new User
            {
                Email = "admin@eduvn.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "Administrator",
                Role = UserRole.Admin,
                Status = UserStatus.Approved,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Admin user created:");
            Console.WriteLine($"   Email: {adminUser.Email}");
            Console.WriteLine($"   Password: Admin@123");
        }

        // Seed Demo Instructor (Approved)
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Instructor))
        {
            var instructor = new User
            {
                Email = "instructor@eduvn.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor@123"),
                FullName = "Nguyễn Văn A - Giáo viên Toán",
                Role = UserRole.Instructor,
                Status = UserStatus.Approved,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(instructor);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Demo instructor created:");
            Console.WriteLine($"   Email: {instructor.Email}");
            Console.WriteLine($"   Password: Instructor@123");
        }

        // Seed Demo Student
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Student))
        {
            var student = new User
            {
                Email = "student@eduvn.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                FullName = "Trần Thị B - Học sinh",
                Role = UserRole.Student,
                Status = UserStatus.Approved,
                Grade = 12,
                School = "THPT Nguyễn Huệ",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(student);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Demo student created:");
            Console.WriteLine($"   Email: {student.Email}");
            Console.WriteLine($"   Password: Student@123");
            Console.WriteLine($"   Grade: {student.Grade}");
            Console.WriteLine($"   School: {student.School}");
        }

        Console.WriteLine("\n🎉 Database seeded successfully!");
    }
}
