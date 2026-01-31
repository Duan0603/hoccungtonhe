# 🎉 EduVN Database & Services Running Successfully!

## ✅ Services Status

| Service | Status | URL | Port |
|---------|--------|-----|------|
| **PostgreSQL** | ✅ Running | localhost | 5432 |
| **pgAdmin** | ✅ Running | http://localhost:5050 | 5050 |
| **Backend API** | ✅ Running | http://localhost:5104 | 5104 |
| **Swagger UI** | ✅ Running | http://localhost:5104/swagger | 5104 |

---

## 🗄️ Access pgAdmin

### 1. Open pgAdmin
Navigate to: http://localhost:5050

### 2. Login Credentials
- **Email**: `admin@eduvn.com`
- **Password**: `admin`

### 3. Add PostgreSQL Server
1. Click "Add New Server"
2. **General Tab**:
   - Name: `EduVN Local`
3. **Connection Tab**:
   - Host: `eduvn-postgres` (container name)
   - Port: `5432`
   - Database: `eduvn`
   - Username: `hoangducduan`
   - Password: `hoangduan0603`
4. Click "Save"

### 4. Browse Database
- Servers → EduVN Local → Databases → eduvn → Schemas → public → Tables

---

## 👥 Demo User Accounts

Database has been seeded with **3 demo accounts**:

### Admin Account
- **Email**: `admin@eduvn.com`
- **Password**: `Admin@123`
- **Role**: Administrator
- **Status**: Approved

### Instructor Account
- **Email**: `instructor@eduvn.com`
- **Password**: `Instructor@123`
- **Role**: Instructor
- **Fullname**: Nguyễn Văn A - Giáo viên Toán
- **Status**: Approved

### Student Account
- **Email**: `student@eduvn.com`
- **Password**: `Student@123`
- **Role**: Student
- **Grade**: 12
- **School**: THPT Nguyễn Huệ
- **Status**: Approved

---

## 🔍 Verify Demo Users

### Using pgAdmin
```sql
-- View all users
SELECT id, email, full_name, role, status, grade, school, created_at 
FROM users 
ORDER BY created_at;

-- View by role
SELECT email, full_name, role, status 
FROM users 
WHERE role = 'Admin';
```

### Using API (After building authentication endpoint)
```bash
# Test login endpoint
curl -X POST http://localhost:5104/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@eduvn.com","password":"Admin@123"}'
```

---

## 📊 Database Tables

All 7 tables successfully created:

| Table | Records | Description |
|-------|---------|-------------|
| `users` | 3 | Admin, Instructor, Student |
| `courses` | 0 | Course catalog (empty) |
| `lessons` | 0 | Video lessons (empty) |
| `assignments` | 0 | Assignments (empty) |
| `submissions` | 0 | Student submissions (empty) |
| `enrollments` | 0 | Course enrollments (empty) |
| `orders` | 0 | PayOS orders (empty) |
| `__EFMigrationsHistory` | 1 | Migration tracking |

---

## 🧪 Test Swagger UI

### 1. Open Swagger
Navigate to: http://localhost:5104/swagger

### 2. Current Endpoints
- Currently only shows default endpoints
- After Milestone 2, you'll see:
  - `POST /api/auth/register`
  - `POST /api/auth/login`
  - `POST /api/auth/google`
  - etc.

---

## 🐳 Docker Commands

### View Running Containers
```bash
docker ps
```

### View Logs
```bash
# PostgreSQL logs
docker logs eduvn-postgres

# pgAdmin logs
docker logs eduvn-pgadmin
```

### Stop Services
```bash
cd d:\hoconhanho
docker-compose down
```

### Restart Services
```bash
docker-compose up -d
```

### Stop & Remove Data (⚠️ Caution: Deletes database!)
```bash
docker-compose down -v
```

---

## 🚀 Next Steps (Milestone 2)

Now that database is ready, we'll implement:

1. ✅ **JWT Authentication Service**
   - Token generation & validation
   - Refresh token rotation
   
2. ✅ **Auth Controllers**
   - Register (Email + Password)
   - Login (JWT tokens)
   - Google OAuth
   
3. ✅ **Frontend Login/Register UI**
   - Form validation with Zod
   - Protected routes
   - Auth state management (Zustand)

---

## 🔧 Troubleshooting

### Backend not starting?
```bash
# Check if port 5104 is available
netstat -ano | findstr :5104

# Kill process if needed (replace PID)
taskkill /PID <process_id> /F
```

### Can't connect to PostgreSQL from pgAdmin?
- Use hostname: `eduvn-postgres` (NOT `localhost`)
- Containers are in same Docker network

### Want to reset database?
```bash
# Stop containers
docker-compose down -v

# Restart
docker-compose up -d

# Run migrations again
cd backend
dotnet ef database update --project EduVN.Infrastructure --startup-project EduVN.API

# Restart backend to seed data
cd EduVN.API
dotnet run
```

---

*Last Updated: 2026-01-30 20:50*
