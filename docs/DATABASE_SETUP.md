# 🗄️ Database Setup Guide

This guide helps you set up PostgreSQL database for EduVN platform.

---

## 📋 Prerequisites

- PostgreSQL 15+ installed
- pgAdmin (optional, for GUI management)

---

## 🚀 Quick Setup

### Option 1: Using psql Command Line

```bash
# 1. Connect to PostgreSQL as postgres user
psql -U postgres

# 2. Create database
CREATE DATABASE eduvn;

# 3. Create user (optional - if you want a dedicated user)
CREATE USER eduvn_user WITH PASSWORD 'your_password';

# 4. Grant privileges
GRANT ALL PRIVILEGES ON DATABASE eduvn TO eduvn_user;

# 5. Exit
\q
```

### Option 2: Using pgAdmin

1. Open pgAdmin
2. Right-click on "Databases" → "Create" → "Database"
3. Name: `eduvn`
4. Owner: `postgres` (or your custom user)
5. Click "Save"

---

## ⚙️ Configure Connection String

Update `backend/EduVN.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=eduvn;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

**Replace `YOUR_PASSWORD` with your PostgreSQL password!**

---

## 🔧 Run Migrations

```bash
# Navigate to backend folder
cd d:\hoconhanho\backend

# Run EF Core migrations
dotnet ef database update --project EduVN.Infrastructure --startup-project EduVN.API
```

This will:
- ✅ Create all tables (users, courses, lessons, assignments, submissions, enrollments, orders)
- ✅ Apply indexes
- ✅ Set up foreign keys

---

## 🌱 Seed Demo Data (Optional)

To add demo users, update `Program.cs`:

```csharp
// Add before app.Run();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await EduVN.Infrastructure.Data.DatabaseSeeder.SeedAsync(context);
}

app.Run();
```

Then run:
```bash
cd EduVN.API
dotnet run
```

**Demo accounts created:**
- **Admin**: `admin@eduvn.com` / `Admin@123`
- **Instructor**: `instructor@eduvn.com` / `Instructor@123`
- **Student**: `student@eduvn.com` / `Student@123`

---

## ✅ Verify Setup

### Check Tables Created

```sql
-- Connect to eduvn database
\c eduvn

-- List all tables
\dt

-- Expected output:
-- users, courses, lessons, assignments, submissions, enrollments, orders, __EFMigrationsHistory
```

### Check Admin User

```sql
SELECT email, full_name, role, status 
FROM users 
WHERE role = 'Admin';

-- Expected: admin@eduvn.com
```

---

## 🔧 Troubleshooting

### Error: "password authentication failed"

**Solution**: Update connection string with correct password in `appsettings.json`

### Error: "database eduvn does not exist"

**Solution**: Create database manually:
```sql
CREATE DATABASE eduvn;
```

### Error: "peer authentication failed"

**Solution**: Update `pg_hba.conf` to use `md5` instead of `peer`:
```
# Change this line:
local   all             all                                     peer

# To:
local   all             all                                     md5
```

Then restart PostgreSQL:
```bash
# Windows
net stop postgresql-x64-15
net start postgresql-x64-15

# Linux/Mac
sudo systemctl restart postgresql
```

### Error: "relation does not exist"

**Solution**: Run migrations again:
```bash
dotnet ef database update --project EduVN.Infrastructure --startup-project EduVN.API
```

---

## 📊 Database Schema

### Tables Created

| Table | Description |
|-------|-------------|
| `users` | Students, Instructors, Admins |
| `courses` | Course catalog |
| `lessons` | Video lessons & documents |
| `assignments` | Multiple choice & essay questions |
| `submissions` | Student answers with AI feedback |
| `enrollments` | Course enrollments |
| `orders` | PayOS payment tracking |

### Indexes

- `users(email)` - Unique
- `users(google_id)` - Unique
- `courses(subject, grade)` - Composite
- `enrollments(student_id, course_id)` - Unique composite
- `orders(payos_order_id)` - Unique
- `orders(status)` - For filtering

---

## 🎯 Next Steps

After database setup:

1. ✅ Verify backend builds: `dotnet build`
2. ✅ Run backend: `cd EduVN.API && dotnet run`
3. ✅ Test Swagger UI: `https://localhost:5000/swagger`
4. ✅ Start frontend: `cd ../../frontend && npm run dev`

---

*Last Updated: 2026-01-30*
