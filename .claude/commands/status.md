# /status — Tiến độ dự án

Hiển thị tổng quan tiến độ dự án GiapTech.BindingDocx dựa trên code thực tế.

## Instructions

Thực hiện theo thứ tự:

1. Đọc `CLAUDE.md` phần "Tiến độ hiện tại" để lấy danh sách checklist.

2. Kiểm tra nhanh trạng thái code thực tế:
   - Glob `backend/src/**/Controllers/**/*.cs` → đếm controllers và endpoints
   - Glob `frontend/src/pages/**/*` → đếm pages
   - Chạy `git log --oneline -10` → xem 10 commit gần nhất

3. So sánh checklist trong CLAUDE.md với code thực tế. Nếu phát hiện mục nào đã làm nhưng chưa check (hoặc ngược lại), nêu rõ.

4. Trả về báo cáo theo format:

---

## Báo cáo tiến độ — GiapTech.BindingDocx

**Ngày:** [hôm nay]

### Backend API

| Nhóm | Endpoint | Trạng thái |
|------|----------|-----------|
| Auth | POST /login, /refresh-token, /logout | ✅ Done |
| ... | ... | ... |

### Frontend Pages

| Route | Component | Trạng thái |
|-------|-----------|-----------|
| /login | LoginPage | ✅ Done |
| ... | ... | ... |

### Core Features (Business Logic)

| Feature | Backend | Frontend | Ghi chú |
|---------|---------|----------|---------|
| Template upload | ❌ | ❌ | Chưa bắt đầu |
| Binding Engine | ❌ | ❌ | Core feature |
| ... | ... | ... | ... |

### Commits gần nhất
[10 commits gần nhất]

### Tóm tắt
- Hoàn thành: X/Y tính năng
- Ưu tiên tiếp theo: [feature quan trọng nhất chưa làm]
