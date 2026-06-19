# Phase 1 - Hướng dẫn chạy

## Yêu cầu
- .NET 9 SDK (hoặc 10)
- Node.js 18+
- SQL Server 2019+ (hoặc Docker)

---

## Chạy local (không Docker)

### Bước 1: Chuẩn bị SQL Server

Có thể dùng SQL Server local hoặc Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### Bước 2: Cấu hình backend

Kiểm tra `backend/src/GiapTech.BindingDocx.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=GiapTechBindingDocx;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

### Bước 3: Chạy backend

```bash
cd backend
dotnet run --project src/GiapTech.BindingDocx.Api
```

API sẽ chạy tại: http://localhost:5000

Swagger UI: http://localhost:5000/swagger

> **Migration tự động:** Khi backend khởi động lần đầu, schema và seed data tự động được tạo.

### Bước 4: Chạy frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend sẽ chạy tại: http://localhost:5173

### Bước 5: Đăng nhập

| Trường | Giá trị |
|--------|---------|
| Username | `admin` |
| Password | `Admin@123456` |

---

## Chạy với Docker Compose

```bash
docker-compose up -d
```

Ứng dụng sẽ chạy tại: http://localhost

---

## Tài khoản mặc định

| Thông tin | Giá trị |
|-----------|---------|
| Username | admin |
| Password | Admin@123456 |
| Email | admin@giaptech.vn |
| Token ban đầu | 100 token |

---

## API Endpoints (Phase 1)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/v1/auth/login` | Đăng nhập |
| POST | `/api/v1/auth/refresh` | Refresh token |
| POST | `/api/v1/auth/logout` | Đăng xuất |
| GET | `/api/v1/profilegroups` | Danh sách nhóm hồ sơ |
| GET | `/api/v1/tokens/balance` | Số dư token |
| GET | `/api/v1/tokens/packages` | Gói token |
| GET | `/api/v1/tokens/history` | Lịch sử giao dịch |
| GET | `/api/v1/health` | Health check |

---

## Commit message đề xuất

```
feat: Phase 1 - Project foundation with authentication and core architecture

- Clean Architecture (Domain/Application/Infrastructure/Api)
- JWT Authentication + Refresh Token
- Dapper + SQL Server with auto-migration and seed data  
- Profile Group management (6 groups seeded)
- Token system (balance, packages, transactions)
- React + TypeScript + Vite + Ant Design frontend
- Login page + collapsible sidebar + main workspace
- Token drawer + Settings drawer
- Docker Compose + Nginx
```
