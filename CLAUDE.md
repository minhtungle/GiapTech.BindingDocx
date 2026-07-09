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
# API: http://localhost:5001 (port 5000 bị macOS Control Center chiếm)
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

## Tiến độ hiện tại (2026-06-20)

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
- [x] Frontend: ProfilePage
- [x] Frontend: Admin Users, Token Packages, Transactions pages
- [x] Frontend: Sidebar, Toolbar, TokenDrawer, SettingsDrawer
- [x] Docker + docker-compose
- [x] Nginx config
- [x] **Workspace — Binding Engine** (2026-06-19)
  - Backend: `WorkspaceController` — 5 endpoints (groups, keys, export-template, import-data, generate)
  - Backend: `TemplateKeyExtractor` — đọc key từ docx (merge runs) + xlsx (phân loại single/table)
  - Backend: `ExcelTemplateGenerator` — tạo Excel mẫu multi-sheet
  - Backend: `ExcelDataParser` — parse Excel import → single fields + table rows
  - Backend: `TemplateRenderer` — fill docx/xlsx, tạo ZIP output
  - Backend: Token deduction khi generate (min 1, tính theo số row bảng)
  - Frontend: Sidebar đổi thành Select nhóm + hiển thị file list
  - Frontend: WorkspacePage 2 tab (Nhập dữ liệu / Preview) + footer export
  - Frontend: KeysTab — form single fields + import bảng Excel
  - Frontend: PreviewTab — xem lại dữ liệu trước khi xuất
  - Fix: password hash admin (`Admin@123`)
  - Config: Docker volume mount `export_file_temp`, port API đổi 5000→5001
- [x] **Workspace — Nâng cấp nhập liệu & preview** (2026-06-20)
  - Backend: `TemplateKeyExtractor` — per-file `Keys` list; nhiều bảng/xlsx → `TableKey = fileBase_bang_N.xlsx`
  - Backend: `ITemplateRenderer.RenderXlsx` — nhận `tableDataBySheet: Dictionary<string, List<...>>`
  - Backend: `ExcelTemplateGenerator` — sheet name lấy từ `TableKey` base
  - Backend: `GenerateFilesCommand` — thêm `SyncMode`, `SingleFieldsByFile`, map tableKey→sheetName qua extractor
  - Backend: `PreviewRenderedQuery` + Handler + endpoint `POST .../files/{fileName}/preview-rendered`
  - Frontend: `TemplateFileInfo.keys`, `TableFileInfo.tableKey` (types)
  - Frontend: `appStore` — `syncEnabled` (default ON), `setSingleField(fileName, key, value)` sync propagation, `getMergedSingleFields()`
  - Frontend: `KeysTab` — Collapse per-file, sync toggle + Modal confirm khi tắt
  - Frontend: `PreviewTab` — Select file → gọi preview-rendered API → render docx (mammoth) / xlsx (SheetJS)
  - Frontend: `WorkspacePage.css` — fix Ant Design Tabs scroll (`.ant-tabs-content-holder` flex override)
- [x] **Sidebar UX** (2026-06-20)
  - Bỏ Select chọn nhóm trùng lặp ở Toolbar, chỉ giữ 1 Select ở Sidebar
  - Auto-select nhóm 1 khi load trang (xử lý cả trường hợp refresh khi `selectedGroupId` đã persist nhưng `groupKeys` null)
  - Options hiển thị tên nhóm thay vì ID (bỏ `optionRender`, embed `🔒` vào label)

### Chưa làm
- [ ] **Payment flow** — MockPaymentProvider, sau đó VNPay/MoMo
- [ ] **Workspace group_2, group_3** — mở khóa khi template sẵn sàng
- [ ] **TemplateFile quản lý qua UI** — upload/delete template từ admin panel
- [ ] **Dọn dẹp** — xóa `DataTab.tsx`, `MappingTab.tsx` (tab cũ không còn dùng)

---

## Ports

| Service    | Port  |
|------------|-------|
| Frontend   | 5173  |
| API        | 5261 (dev) / 5001 (docker) |
| SQL Server | 1433  |
| MinIO      | 9000  |
| Nginx      | 80    |
