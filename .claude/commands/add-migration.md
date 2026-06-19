# /add-migration — Thêm database migration

Thêm migration mới vào hệ thống migration tự viết của dự án.

## Cách dùng

```
/add-migration <mô tả migration>
```

Ví dụ:
```
/add-migration Thêm bảng TemplateFiles
/add-migration Thêm cột OutputPath vào ProfileGroups
```

## Instructions

1. Đọc file `backend/src/GiapTech.BindingDocx.Infrastructure/Data/DatabaseMigrator.cs`

2. Xác định số thứ tự migration tiếp theo (xem các `yield return` trong `GetMigrations()`)

3. Thêm migration mới:

```csharp
// Trong GetMigrations():
yield return ("00X_TenMigration", Migration00X_TenMigration);

// Thêm const mới:
private const string Migration00X_TenMigration = @"
-- SQL ở đây
-- Dùng GO để phân tách batch
";
```

4. Quy tắc SQL:
   - Dùng `IF NOT EXISTS` trước khi tạo bảng hoặc thêm cột (idempotent)
   - Luôn dùng `NEWSEQUENTIALID()` cho primary key GUID
   - Timestamps: `DATETIME2 NOT NULL DEFAULT GETUTCDATE()`
   - GUID columns: `UNIQUEIDENTIFIER`
   - Phân tách batch bằng `GO` trên dòng riêng

5. Cập nhật Entity tương ứng trong `Domain/Entities/` nếu có thay đổi schema

6. Cập nhật Repository Interface trong `Domain/Interfaces/` nếu cần method mới

7. Cập nhật Repository Implementation trong `Infrastructure/Repositories/`

## Lưu ý

Migration chạy tự động khi API khởi động. Không cần chạy lệnh riêng.
Migration được track trong bảng `__Migrations` — một migration chỉ chạy một lần.
