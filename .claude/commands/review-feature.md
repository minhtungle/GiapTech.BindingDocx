# /review-feature — Review code feature vừa làm

Kiểm tra một feature vừa implement có đúng convention và đầy đủ không.

## Cách dùng

```
/review-feature <tên feature hoặc file path>
```

Ví dụ:
- `/review-feature TemplateFile upload`
- `/review-feature backend/src/.../TemplateFilesController.cs`

## Instructions

Đọc code của feature cần review, sau đó kiểm tra theo checklist sau:

### Backend Checklist

**Clean Architecture:**
- [ ] Domain không import Application hoặc Infrastructure
- [ ] Application chỉ import Domain
- [ ] Controller không gọi Repository trực tiếp (phải qua MediatR)

**Command/Query:**
- [ ] Có Validator nếu command nhận input từ user
- [ ] Handler không chứa business logic phức tạp inlined — nên delegate xuống Repository
- [ ] Trả đúng kiểu dữ liệu (không return entity trực tiếp, phải map sang DTO)

**Repository:**
- [ ] Dùng Dapper, không dùng EF Core
- [ ] Tất cả method đều async và nhận CancellationToken
- [ ] Không có SQL injection (dùng parameterized queries)

**Controller:**
- [ ] Route đúng pattern: `api/v{version:apiVersion}/...`
- [ ] Response wrap bằng `ApiResponse<T>`
- [ ] Có `[ProducesResponseType]` attribute
- [ ] Admin endpoint có `[Authorize(Roles = "admin")]`

**Migration:**
- [ ] Nếu có thêm bảng/cột, có migration tương ứng
- [ ] Migration idempotent (có IF NOT EXISTS)

### Frontend Checklist

**Data fetching:**
- [ ] Dùng TanStack Query, không dùng useState+useEffect để fetch
- [ ] Error state được xử lý
- [ ] Loading state được xử lý

**TypeScript:**
- [ ] Không có `any` type
- [ ] Response type được định nghĩa trong `types/`

**Service layer:**
- [ ] Component không gọi axios trực tiếp
- [ ] Có service file riêng

**UI:**
- [ ] Dùng Ant Design components
- [ ] Không mix component library khác

### Báo cáo

Sau khi review, trả về:
1. Danh sách vấn đề tìm thấy (nếu có), theo thứ tự ưu tiên
2. Danh sách điểm tốt cần giữ
3. Đề xuất fix nếu cần
