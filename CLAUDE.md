# GiapTech.BindingDocx — Claude Context

## Mục đích dự án

Hệ thống web binding dữ liệu vào template Word/Excel. Người dùng import dữ liệu hồ sơ (Excel/CSV/JSON), mapping với template, hệ thống tự động điền vào file docx/xlsx rồi xuất ZIP. Có hệ thống token tính phí: 1 hồ sơ = 1 token.

Roadmap sau: phát triển thành SaaS đa khách hàng (multi-tenant).

---

## Tech Stack

### Backend — .NET 10, Clean Architecture
- **ORM:** Dapper + raw SQL (KHÔNG dùng EF Core)
- **CQRS:** MediatR + FluentValidation
- **Auth:** JWT Bearer + Refresh Token
- **Logging:** Serilog
- **Document:** OpenXML SDK + ClosedXML
- **Storage:** Provider pattern — Local (default) hoặc MinIO
- **Database:** SQL Server (migration tự viết, không dùng EF migrations)

### Frontend — React + TypeScript (Vite)
- **UI:** Ant Design
- **Data fetching:** TanStack Query
- **Table:** TanStack Table
- **Router:** React Router v6
- **HTTP:** Axios
- **State:** Zustand

### Infrastructure
- Docker + Docker Compose
- Nginx reverse proxy
- SQL Server 2022

---

## Cấu trúc dự án

```
GiapTech.BindingDocx/
├── backend/src/
│   ├── GiapTech.BindingDocx.Domain/        # Entities, Interfaces (không có dependency)
│   ├── GiapTech.BindingDocx.Application/   # Commands, Queries, DTOs, Validators
│   ├── GiapTech.BindingDocx.Infrastructure/# Repositories, DbConnectionFactory, Storage, Auth
│   └── GiapTech.BindingDocx.Api/           # Controllers, Middleware, Program.cs
├── frontend/src/
│   ├── pages/                              # LoginPage, WorkspacePage, ProfilePage, Admin/*
│   ├── components/                         # Sidebar, Toolbar, SettingsDrawer, TokenDrawer
│   ├── layouts/                            # MainLayout, AdminLayout
│   ├── services/                           # API calls (axios)
│   ├── stores/                             # Zustand stores (authStore, appStore)
│   ├── hooks/
│   ├── types/
│   └── router/
├── storage/                                # Local file storage
├── docker-compose.yml
└── PROMPT.md                               # Business requirements gốc
```

---

## Conventions — BẮT BUỘC TUÂN THỦ

### Backend

1. **Clean Architecture nghiêm ngặt** — Domain không import bất kỳ layer nào khác. Application chỉ import Domain.
2. **Dapper only** — KHÔNG dùng EF Core, KHÔNG dùng LINQ to SQL.
3. **CQRS với MediatR** — Mỗi use case là một Command hoặc Query riêng biệt.
4. **Repository pattern** — Controller không gọi trực tiếp DB, phải qua Repository.
5. **Migration tự viết** — Thêm vào `DatabaseMigrator.cs`, đặt tên `00X_TenMigration`.
6. **API Versioning** — Tất cả endpoint prefix `/api/v{version:apiVersion}/`.
7. **Response chuẩn** — Luôn trả `ApiResponse<T>` hoặc `ApiResponse`.
8. **Validation** — FluentValidation cho mọi Command có input từ user.
9. **Role-based auth** — User thường: `[Authorize]`. Admin: `[Authorize(Roles = "admin")]`.
10. **Async/await** — Tất cả DB call phải async, truyền `CancellationToken`.

### Frontend

1. **Ant Design** cho mọi UI component — không mix với shadcn/MUI.
2. **TanStack Query** cho mọi server state — không dùng useState + useEffect để fetch.
3. **Zustand** cho global client state (auth, app settings).
4. **Service layer** — Component không gọi axios trực tiếp, phải qua service file.
5. **TypeScript strict** — Không dùng `any`, phải có type cho mọi API response.

### Chung

- **KHÔNG hardcode** dữ liệu nghiệp vụ — tất cả đọc từ DB.
- **KHÔNG để TODO** hoặc pseudo code trong code.
- **KHÔNG tạo file comment** hay README trừ khi được yêu cầu.
- **KHÔNG thêm feature** ngoài scope được yêu cầu.
- Mọi thay đổi DB phải có migration tương ứng.

---

## Placeholders Template Engine

Template sử dụng cú pháp Mustache-like:

```
{{FieldName}}          — text thường
{{Avatar}}             — image (giữ tỷ lệ, không méo)
{{#ListField}}...{{/ListField}}    — repeater (tabular data)
{{#BoolField}}...{{/BoolField}}    — conditional
```

---

## Business Rules quan trọng

- **Token:** 1 hồ sơ xử lý = 1 token bị trừ
- **Validation trước Generate:** kiểm tra đủ token, nếu không đủ thì block
- **Output:** mỗi hồ sơ ra 1 folder, mỗi template ra 1 file trong folder đó
- **ZIP:** sau generate tự động tạo ZIP (tên theo người/batch)
- **ProfileGroup:** 6 nhóm, mỗi nhóm có template riêng, output riêng

---

## Lệnh chạy dev

### Backend
```powershell
cd backend
dotnet run --project src/GiapTech.BindingDocx.Api
# Swagger: http://localhost:5261/swagger
```

### Frontend
```powershell
cd frontend
npm run dev
# App: http://localhost:5173
```

### Docker (toàn bộ stack)
```powershell
docker-compose up -d
# App: http://localhost:80
# API: http://localhost:5000
```

### Database (Docker only)
```powershell
docker-compose up -d sqlserver
# SQL Server: localhost:1433, user: sa, pass: YourStrong@Passw0rd
```

---

## Tài khoản mặc định

| Field    | Value              |
|----------|--------------------|
| Username | admin              |
| Password | Admin@123          |
| Role     | admin              |
| Token    | 100                |

---

## Tiến độ hiện tại (2026-06-19)

### Hoàn thành
- [x] Clean Architecture scaffold (4 layers)
- [x] Database schema + migration 001, 002, 003
- [x] Auth: Login, RefreshToken, Logout
- [x] JWT + Refresh Token
- [x] Admin: CRUD Users, toggle active, adjust tokens
- [x] Admin: CRUD TokenPackages, toggle active
- [x] Admin: xem tất cả transactions
- [x] User: xem balance, packages, history token
- [x] ProfileGroups: GET list
- [x] Frontend: Login, MainLayout, AdminLayout
- [x] Frontend: WorkspacePage (scaffold 3 tab)
- [x] Frontend: ProfilePage
- [x] Frontend: Admin Users, Token Packages, Transactions pages
- [x] Frontend: Sidebar, Toolbar, TokenDrawer, SettingsDrawer
- [x] Docker + docker-compose
- [x] Nginx config

### Chưa làm (Core features)
- [ ] **TemplateFile API** — upload, list, delete template per group
- [ ] **ImportBatch API** — tạo batch, upload Excel/CSV/JSON
- [ ] **ImportRecord API** — list records trong batch
- [ ] **Binding Engine** — điền dữ liệu vào docx/xlsx (OpenXML + ClosedXML)
- [ ] **Generate API** — trigger binding, trừ token, tạo output files
- [ ] **ZIP Export API** — đóng gói output thành ZIP
- [ ] **Payment flow** — MockPaymentProvider, sau đó VNPay/MoMo
- [ ] **Frontend DataTab** — TanStack Table hiển thị imported records
- [ ] **Frontend MappingTab** — mapping UI (dropdown + drag-drop)
- [ ] **Frontend PreviewTab** — preview docx/xlsx/pdf (Mammoth.js, PDF.js, SheetJS)
- [ ] **Frontend Import Modal** — 4 step wizard (type → upload → mapping → preview)
- [ ] **Frontend Generate button** — gọi API, trừ token, download ZIP

---

## Ports

| Service    | Port  |
|------------|-------|
| Frontend   | 5173  |
| API        | 5261 (dev) / 5000 (docker) |
| SQL Server | 1433  |
| MinIO      | 9000  |
| Nginx      | 80    |
