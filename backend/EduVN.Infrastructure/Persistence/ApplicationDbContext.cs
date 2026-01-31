using System;
using EduVN.Domain.Entities;
using EduVN.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduVN.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(UserStatus.Pending);
            entity.Property(e => e.School).HasMaxLength(255);
            entity.Property(e => e.GoogleId).HasMaxLength(255);
            entity.HasIndex(e => e.GoogleId).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        });

        // Course Configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("courses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Subject).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Instructor)
                .WithMany(u => u.CoursesAsInstructor)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.Subject, e.Grade });
        });

        // Lesson Configuration
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.ToTable("lessons");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
            entity.Property(e => e.DocumentUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Assignment Configuration
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("assignments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(e => e.Content).HasColumnType("jsonb").IsRequired();
            entity.Property(e => e.Answers).HasColumnType("jsonb");
            entity.Property(e => e.MaxScore).HasColumnType("decimal(5,2)").HasDefaultValue(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Submission Configuration
        modelBuilder.Entity<Submission>(entity =>
        {
            entity.ToTable("submissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Answers).HasColumnType("jsonb").IsRequired();
            entity.Property(e => e.Score).HasColumnType("decimal(5,2)");
            entity.Property(e => e.AIFeedback).HasColumnType("jsonb");
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Student)
                .WithMany(u => u.Submissions)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.AssignmentId);
        });

        // Enrollment Configuration
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("enrollments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EnrollmentType).HasConversion<string>().HasMaxLength(20).HasDefaultValue(EnrollmentType.Paid);
            entity.Property(e => e.EnrolledAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
        });

        // Order Configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(OrderStatus.Pending);
            entity.Property(e => e.PayOSOrderId).HasMaxLength(255);
            entity.HasIndex(e => e.PayOSOrderId).IsUnique();
            entity.Property(e => e.PayOSTransactionId).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Student)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Status);
        });

        // RefreshToken Configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).HasMaxLength(500).IsRequired();
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.IsRevoked });
        });



        // Seed Initial Users
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var instructorId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var studentId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Passwords: Admin@123, Teacher@123, Student@123
        // Hashes generated via tool to ensure stability across migrations
        
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminId,
                FullName = "Admin User",
                Email = "admin@hoccungtonhe.com",
                PasswordHash = "$2a$11$eIEAErt55HWnnDvJE1DOP.9MKV70fzW.96e7sFvv6D6Wmi7T3EblK",
                Role = UserRole.Admin,
                Status = UserStatus.Approved,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = instructorId,
                FullName = "Instructor User",
                Email = "teacher@hoccungtonhe.com",
                PasswordHash = "$2a$11$SkF1AimECPvz0goq3S/tR.d1s/TDTDM6pvJlraZC8KHcGQN2YhqZm",
                Role = UserRole.Instructor,
                Status = UserStatus.Approved,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = studentId,
                FullName = "Student User",
                Email = "student@hoccungtonhe.com",
                PasswordHash = "$2a$11$LQhhIwaxOp6hp9wkuYFTi.ZjFCS5Q/JftH9SudOKje7SK7TRAuGxu",
                Role = UserRole.Student,
                Status = UserStatus.Approved,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
