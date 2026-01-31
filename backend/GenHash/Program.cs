using BCrypt.Net;

string adminHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
string teacherHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123");
string studentHash = BCrypt.Net.BCrypt.HashPassword("Student@123");

Console.WriteLine($"ADMIN|{adminHash}");
Console.WriteLine($"TEACHER|{teacherHash}");
Console.WriteLine($"STUDENT|{studentHash}");
