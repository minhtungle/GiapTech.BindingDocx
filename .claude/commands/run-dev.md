# /run-dev — Khởi động môi trường dev

Kiểm tra trạng thái và in ra hướng dẫn khởi động environment development.

## Instructions

1. Chạy lệnh `docker ps` để kiểm tra SQL Server container đang chạy chưa.

2. Nếu SQL Server chưa chạy, nhắc người dùng chạy: `docker-compose up -d sqlserver` và đợi khoảng 30 giây.

3. Thử build backend để kiểm tra không có lỗi compile: `dotnet build backend/`

4. In hướng dẫn khởi động:

**Terminal 1 — Backend:**
- Lệnh: `cd backend && dotnet run --project src/GiapTech.BindingDocx.Api`
- API URL: http://localhost:5261
- Swagger: http://localhost:5261/swagger
- Migration tự chạy khi khởi động

**Terminal 2 — Frontend:**
- Lệnh: `cd frontend && npm run dev`
- App URL: http://localhost:5173

**Tài khoản mặc định:**
- Username: admin / Password: Admin@123 / Role: admin / Token: 100

**Database kết nối:**
- Server: localhost,1433 | DB: GiapTechBindingDocx | User: sa | Pass: YourStrong@Passw0rd

**Chạy toàn bộ Docker:**
- Lệnh: `docker-compose up -d`
- App: http://localhost:80
