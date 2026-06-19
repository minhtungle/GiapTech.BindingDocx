# /add-feature — Thêm feature mới đúng pattern

Thêm một feature hoàn chỉnh theo Clean Architecture của dự án.

## Cách dùng

```
/add-feature <mô tả feature>
```

Ví dụ:
- `/add-feature Upload template file cho ProfileGroup`
- `/add-feature API Generate binding docx`
- `/add-feature Frontend Import Modal 4 bước`

## Instructions

Trước khi code, đọc các file mẫu để nắm pattern:
- `backend/src/GiapTech.BindingDocx.Application/Admin/Users/Commands/CreateUser/` — mẫu Command + Handler + Validator
- `backend/src/GiapTech.BindingDocx.Infrastructure/Repositories/UserRepository.cs` — mẫu Repository
- `backend/src/GiapTech.BindingDocx.Api/Controllers/v1/Admin/AdminUsersController.cs` — mẫu Controller

Implement theo thứ tự sau:

### Nếu feature cần backend

**Bước 1 — Domain layer** (chỉ khi cần entity hoặc interface mới):
- Entity mới: `Domain/Entities/TenEntity.cs` kế thừa `BaseEntity`
- Interface repository mới: `Domain/Interfaces/ITenRepository.cs`

**Bước 2 — Application layer:**
- DTO: `Application/{Feature}/DTOs/TenDto.cs`
- Command hoặc Query + Handler: `Application/{Feature}/Commands/TenAction/`
- Validator (nếu có user input): cùng thư mục với Command

**Bước 3 — Infrastructure layer:**
- Repository: `Infrastructure/Repositories/TenRepository.cs`
- Đăng ký DI trong `Infrastructure/DependencyInjection.cs`

**Bước 4 — Migration** (nếu thêm bảng hoặc cột):
- Thêm migration vào `DatabaseMigrator.cs` theo pattern hiện có
- Số thứ tự tiếp nối: xem `GetMigrations()` để lấy số tiếp theo

**Bước 5 — API layer:**
- Controller: `Api/Controllers/v1/TenController.cs`
- Route pattern: `api/v{version:apiVersion}/ten-endpoint`
- Response: luôn wrap bằng `ApiResponse<T>` hoặc `ApiResponse`
- Admin endpoint: `[Authorize(Roles = "admin")]`

### Nếu feature cần frontend

1. Type definition: `frontend/src/types/`
2. Service (axios calls): `frontend/src/services/tenService.ts`
3. TanStack Query hooks nếu cần fetch/mutate
4. Component hoặc Page: `frontend/src/pages/` hoặc `components/`
5. Route mới (nếu là page): thêm vào `frontend/src/router/index.tsx`

### Sau khi code xong

- Build backend để xác nhận không lỗi compile
- Cập nhật `CLAUDE.md` phần "Tiến độ hiện tại" — đánh dấu `[x]` cho mục vừa làm xong

## Quy tắc bắt buộc

- Không để TODO, không để pseudo code, không bỏ implementation
- Không dùng EF Core — chỉ Dapper + raw SQL
- Mọi DB call phải async và nhận CancellationToken
- Không thêm feature ngoài scope được yêu cầu
- Không dùng `any` trong TypeScript
