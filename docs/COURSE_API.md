# 📚 Course Management API Documentation

## 🔄 Status: Implemented & Verified

## 🛠️ Endpoints

Base URL: `http://localhost:5104/api/courses`

### 1. Get Courses (Public)
**GET** `/api/courses`

**Query Parameters:**
- `search`: string (checks title and description)
- `subject`: string (exact match)
- `grade`: int (10, 11, 12)
- `minPrice`: decimal
- `maxPrice`: decimal
- `page`: int (default 1)
- `pageSize`: int (default 10)

**Response:**
```json
{
  "data": [
    {
      "id": "uuid",
      "title": "Toán 12 - Chuyên đề Hàm Số",
      "price": 500000,
      "instructorName": "Nguyễn Văn A",
      "thumbnailUrl": "url..."
    }
  ],
  "totalCount": 50,
  "totalPages": 5
}
```

### 2. Get Course Detail (Public)
**GET** `/api/courses/{id}`

**Response:**
```json
{
  "id": "uuid",
  "title": "Toán 12 - Chuyên đề Hàm Số",
  "description": "Full course content...",
  "price": 500000,
  "subject": "Toán",
  "grade": 12,
  "thumbnailUrl": "...",
  "instructorName": "Nguyễn Văn A",
  "createdAt": "2026-01-30T..."
}
```

### 3. Create Course (Instructor/Admin)
**POST** `/api/courses`
**Auth**: Bearer Token required (Role: Instructor or Admin)

**Body:**
```json
{
  "title": "Lý 11 - Điện Học",
  "description": "Khóa học cơ bản...",
  "price": 300000,
  "subject": "Lý",
  "grade": 11
}
```

### 4. Update Course (Instructor/Admin)
**PUT** `/api/courses/{id}`
**Auth**: Bearer Token required (Only Owner or Admin)

**Body:** (Partial updates supported)
```json
{
  "title": "New Title",
  "price": 350000,
  "isPublished": true
}
```

### 5. Delete Course (Instructor/Admin)
**DELETE** `/api/courses/{id}`
**Auth**: Bearer Token required (Only Owner or Admin)

### 6. Get My Courses (Instructor)
**GET** `/api/courses/my-courses`
**Auth**: Bearer Token required (Role: Instructor)

**Response:** List of courses created by the current user.

---

## 🔒 Authorization Rules

- **Public**: Anyone can view course list and details.
- **Instructor**: Can create courses. Can update/delete ONLY their own courses.
- **Admin**: Can create, update, delete ANY course.

---

## 🧪 Testing with Swagger

1. **Login** as Instructor to get Token.
   - User: `instructor@eduvn.com` / `Instructor@123`
2. **Authorize** in Swagger (Paste `Bearer <token>`).
3. **Try POST /api/courses**.
4. **Try GET /api/courses**.

---

## 📝 Next Steps (Task 3.2: File Upload)

Need to implement `IFileUploadService` to handle:
- Course Thumbnails
- Lesson Videos
- Document PDFs
