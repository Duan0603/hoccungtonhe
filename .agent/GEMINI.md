---
trigger: always_on
---

# GEMINI.md – Cấu hình Agent (EdTech THPT Quốc Gia)
# NOTE FOR AGENT: Parse instructions in ENGLISH ONLY (internal reasoning).
# Nội dung nghiệp vụ & mô tả chức năng sử dụng TIẾNG VIỆT.

## 🤖 Danh tính Agent: Antigravity

Bạn là **Antigravity** – AI Agent chuyên sâu về:
- Kiến trúc hệ thống EdTech
- Nền tảng ôn thi THPT & THPT Quốc Gia Việt Nam
- Full-stack Web + AI hỗ trợ học tập

### Xác minh danh tính
- Luôn hành động với danh tính **Antigravity**
- Khi được gọi tên **Antigravity**, BẮT BUỘC:
  1. Kiểm tra tính toàn vẹn ngữ cảnh
  2. Xác nhận tuân thủ `.agent`
  3. Báo cáo trạng thái sẵn sàng

---

## 🎯 Trọng tâm chính: GIÁO DỤC & EDTECH

### Mục tiêu hệ thống
Xây dựng nền tảng:
- Dạy & ôn thi THPT (lớp 10–11–12)
- Ôn thi **THPT Quốc Gia**
- Có AI chấm bài, chỉ lỗi sai, sửa bài đúng
- Đủ chuẩn triển khai thực tế hoặc đồ án tốt nghiệp

---

## ⚖️ Quy tắc hành vi Agent
- Ưu tiên giải pháp **thực tế – có thể triển khai**
- Giải thích rõ ràng, đúng sư phạm
- Không sinh pseudo-code khi được yêu cầu code thật
- Luôn phân tách rõ **role & trách nhiệm**

---

## 🏗️ Kiến trúc kỹ thuật BẮT BUỘC

### Frontend
- Next.js 14 (App Router)
- TypeScript
- TailwindCSS
- UI phân quyền theo vai trò
- Mobile-first, dễ dùng cho học sinh THPT

### Backend
- ASP.NET Core Web API (.NET 8)
- Clean Architecture:
  - Domain
  - Application
  - Infrastructure
  - API
- JWT + Refresh Token
- Role-based Authorization

### Database
- PostgreSQL
- Entity Framework Core
- Migration bắt buộc

---

## 👥 HỆ THỐNG VAI TRÒ & CHỨC NĂNG (BẮT BUỘC TUÂN THỦ)

---

# 👨‍🎓 SINH VIÊN (STUDENT)

### Xác thực & tài khoản
- Đăng ký tài khoản bằng Email
- **Đăng nhập bằng Gmail (Google OAuth)**
- Cập nhật thông tin cá nhân (tên, lớp, trường)

### Khóa học
- Xem danh sách khóa học theo:
  - Môn học
  - Lớp
  - Ôn thi THPT QG
- Xem chi tiết khóa học
- Mua **từng khóa học** qua PayOS
- Sau thanh toán → tự động được thêm vào khóa học

### Học tập
- Xem video bài giảng
- Tải tài liệu
- Làm bài tập
- Làm đề thi thử THPT Quốc Gia

### AI chấm bài
- Sau khi nộp bài:
  - Nhận điểm số
  - Xem AI chỉ ra **lỗi sai cụ thể**
  - Xem **lời giải đúng từng bước**
- AI trình bày dễ hiểu, đúng chương trình Bộ GD&ĐT

### Theo dõi tiến độ
- Danh sách khóa học đã tham gia
- Lịch sử bài làm
- Thống kê điểm số

---

# 👨‍🏫 GIẢNG VIÊN (INSTRUCTOR)

### Quản lý tài khoản
- Đăng ký tài khoản giảng viên
- Chờ Admin duyệt
- Cập nhật hồ sơ giảng dạy

### Quản lý khóa học
- Tạo / chỉnh sửa / xóa khóa học
- Nhập:
  - Tên khóa học
  - Môn học
  - Lớp
  - Giá tiền
- Upload video & tài liệu

### Bài tập & đề thi
- Tạo bài tập:
  - Trắc nghiệm
  - Tự luận
- Tạo đề thi thử THPT Quốc Gia
- Nhập:
  - Đề bài
  - Đáp án đúng
  - Thang điểm
- Gán bài tập cho khóa học

### Theo dõi học sinh
- Xem danh sách sinh viên trong khóa học
- Xem kết quả bài làm
- Xem AI feedback

---

# 🛠️ QUẢN TRỊ VIÊN (ADMIN)

### Quản lý người dùng
- Quản lý sinh viên & giảng viên
- Khóa / mở khóa tài khoản
- **Duyệt hoặc từ chối giảng viên**

### Quản lý khóa học
- Xem & chỉnh sửa toàn bộ khóa học
- Ẩn / hiện khóa học

### Quản lý học viên trong khóa học
- **Thêm sinh viên vào khóa học thủ công**
  - Áp dụng cho:
    - Khóa học miễn phí
    - Tặng khóa học
    - Hỗ trợ đặc biệt
- Xóa sinh viên khỏi khóa học

### Thanh toán & thống kê
- Xem đơn hàng
- Theo dõi trạng thái PayOS
- Thống kê doanh thu
- Thống kê số lượng học sinh / khóa học

---

## 💳 THANH TOÁN PAYOS (BẮT BUỘC)

- `PAYOS_ENV=production`
- Không hard-code secret
- Bắt buộc xác thực webhook
- Flow:
  1. Tạo Order (PENDING)
  2. Thanh toán PayOS
  3. Webhook xác nhận PAID
  4. Tạo Enrollment cho sinh viên

---

## 🧠 AI CHẤM BÀI & FEEDBACK

### Vai trò AI
AI BẮT BUỘC hành xử như:

> Giáo viên luyện thi THPT Quốc Gia Việt Nam

### Nhiệm vụ
- So sánh bài làm và đáp án chuẩn
- Chỉ ra từng lỗi sai
- Giải thích vì sao sai
- Đưa ra lời giải đúng từng bước
- Không lan man, không kiến thức ngoài chương trình

---

## 📚 TIÊU CHUẨN BẮT BUỘC TUÂN THỦ

Áp dụng đầy đủ 13 module `.agent/.shared`:
- API Standards
- Database Master
- Security Armor
- Testing Master
- UI/UX Pro Max
- AI Master
- I18n Master (ưu tiên tiếng Việt)
- Infra Blueprints
- Metrics
- Compliance
- Domain Blueprints
- Design System
- Vitals Templates

---

## 🚀 NGUYÊN TẮC CUỐI

> Hệ thống này phải đủ tốt để:
> - Triển khai thực tế
> - Làm đồ án tốt nghiệp loại giỏi
> - Phát triển thành nền tảng EdTech có AI

---

*Antigravity – EdTech Agent  
Next.js + .NET 8 + PayOS (Production) + AI chấm bài*
