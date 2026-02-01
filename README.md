# 📚 hoccungtonhe - Ôn thi THPT Quốc Gia

Nền tảng ôn thi THPT Quốc Gia với AI chấm bài thông minh.

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| **Frontend** | Next.js 15, TypeScript, TailwindCSS |
| **Backend** | ASP.NET Core 8, Entity Framework Core |
| **Database** | PostgreSQL 15 |
| **Auth** | JWT + Refresh Token, Google OAuth |
| **Storage** | Cloudinary (videos, images) |
| **Payment** | PayOS (coming soon) |

---

## ⚡ Quick Start (Dành cho người mới)

Chạy lần lượt các lệnh sau theo thứ tự:

### Bước 1: Khởi động Database (Bắt buộc chạy trước)
```bash
docker-compose up -d
```
*Đợi khoảng 10s để database khởi động xong.*

### Bước 2: Setup Backend & Migration
```bash
cd backend
cp ../backend/EduVN.API/.env.example .env
# (Tùy chọn) Sửa file .env nếu cần

cd backend
./db-update.bat
```
## Cho Mac OS
```bash
cd backend
chmod +x db-update.sh
./db-update.sh
```

### Bước 3: Setup Frontend (Terminal mới)
```bash
cd frontend
cp .env.example .env.local
npm install
npm run dev
```

---

##  Setup cho Collaborators

### Yêu cầu

- [Node.js 18+](https://nodejs.org/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15](https://www.postgresql.org/download/) hoặc Docker
- [Git](https://git-scm.com/)

---

### 1️⃣ Clone repo

```bash
git clone https://github.com/YOUR_USERNAME/hoccungtonhe.git
cd hoccungtonhe
```

---

### 2️⃣ Setup Database (chọn 1 trong 2 cách)

#### Cách A: Dùng Docker (Khuyến nghị)

```bash
docker-compose up -d
```

PostgreSQL sẽ chạy trên `localhost:5432`, pgAdmin trên `localhost:5050`.

#### Cách B: Cài PostgreSQL local

1. Cài PostgreSQL 15
2. Tạo database tên `hoccungtonhe`
3. Ghi nhớ username/password

---

### 3️⃣ Setup Backend

```bash
cd backend/EduVN.API

# Copy file env mẫu
cp .env.example .env

# Sửa file .env với thông tin database của bạn
```

**Nội dung file `.env`:**

```env
# Database
DATABASE_HOST=localhost
DATABASE_PORT=5432
DATABASE_NAME=hoccungtonhe
DATABASE_USER=your_username
DATABASE_PASSWORD=your_password

# JWT (giữ nguyên hoặc tạo mới)
JWT_SECRET_KEY=your_64_character_secret_key_here
JWT_ISSUER=https://hoccungtonhe.com
JWT_AUDIENCE=https://hoccungtonhe.com
JWT_ACCESS_TOKEN_EXPIRY_MINUTES=15
JWT_REFRESH_TOKEN_EXPIRY_DAYS=7

# Cloudinary (xin từ team lead nếu cần)
CLOUDINARY_CLOUD_NAME=your_cloud_name
CLOUDINARY_API_KEY=your_api_key
CLOUDINARY_API_SECRET=your_api_secret

# Google OAuth (optional)
GOOGLE_CLIENT_ID=your_google_client_id.apps.googleusercontent.com
```

**Chạy migrations và start backend:**

```bash
# Cài EF Core tools (lần đầu)
dotnet tool install --global dotnet-ef

# Chạy migrations
dotnet ef database update

# Start backend
dotnet run
```

Backend sẽ chạy trên: `http://localhost:5104`

---

### 4️⃣ Setup Frontend

```bash
cd frontend

# Copy file env mẫu
cp .env.example .env.local

# Cài dependencies
npm install

# Start frontend
npm run dev
```

**Nội dung file `.env.local`:**

```env
NEXT_PUBLIC_API_URL=http://localhost:5104
NEXT_PUBLIC_APP_NAME=hoccungtonhe
NEXT_PUBLIC_GOOGLE_CLIENT_ID=your_google_client_id.apps.googleusercontent.com
```

Frontend sẽ chạy trên: `http://localhost:3000`

---

## � Cấu trúc dự án

```
hoccungtonhe/
├── backend/
│   └── EduVN.API/           # ASP.NET Core API
│       ├── Controllers/
│       ├── Services/
│       ├── Migrations/
│       └── .env.example
├── frontend/
│   └── src/
│       ├── app/             # Next.js App Router
│       ├── components/
│       ├── lib/
│       └── .env.example
├── docker-compose.yml
└── README.md
```

---

## 👥 User Roles

| Role | Permissions |
|------|-------------|
| **Student** | Xem khóa học, mua khóa học, làm bài tập |
| **Instructor** | Tạo/sửa/xóa khóa học, thêm bài học |
| **Admin** | Quản lý users, duyệt giảng viên, xem thống kê |

---

## � Test Accounts

Sau khi chạy migrations, database sẽ có sẵn:

| Email | Password | Role |
|-------|----------|------|
| `admin@hoccungtonhe.com` | `Admin@123` | Admin |
| (Tự đăng ký) | - | Student |

---

## 🧪 Chạy toàn bộ hệ thống

```bash
# Terminal 1: Database (nếu dùng Docker)
docker-compose up -d

# Terminal 2: Backend
cd backend/EduVN.API && dotnet run

# Terminal 3: Frontend
cd frontend && npm run dev
```

Mở browser: http://localhost:3000

---

## 📝 Git Workflow

```bash
# Tạo branch mới từ main
git checkout main
git pull origin main
git checkout -b feature/your-feature-name

# Commit changes
git add .
git commit -m "feat: mô tả ngắn gọn"

# Push và tạo PR
git push origin feature/your-feature-name
```

---

## ❓ Troubleshooting

### Database connection failed

```bash
# Kiểm tra PostgreSQL đang chạy
docker ps  # nếu dùng Docker
# hoặc
pg_isready -h localhost -p 5432
```

### Port already in use

```bash
# Kill process trên port
npx kill-port 3000 5104
```

### Migration failed

### Migration failed or "Failed executing DbCommand"

- Nếu thấy lỗi `Failed executing DbCommand... SELECT "MigrationId"` nhưng cuối cùng vẫn báo `Done.`, thì **ĐÓ LÀ BÌNH THƯỜNG**.
- Đây là do EF Core kiểm tra bảng lịch sử chưa tồn tại (vì database mới tinh). Nó sẽ tự tạo sau đó.

```bash
cd backend/EduVN.API
dotnet ef migrations remove  # Xóa migration lỗi
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

---

## 📞 Liên hệ

- **Team Lead**: [your-email@example.com]
- **Discord**: [link-to-discord]

---

Made with ❤️ by hoccungtonhe team
