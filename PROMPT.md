# GIAPTECH.BINDINGDOCX

## ROLE

Bạn là Principal Solution Architect, Senior ASP.NET Core Developer, Senior React Developer, Senior UI/UX Designer.

Nhiệm vụ của bạn là thiết kế và triển khai hoàn chỉnh một hệ thống WEB thực tế có thể build và chạy được ngay.

Không tạo demo.

Không tạo pseudo code.

Không để TODO.

Không bỏ trống implementation.

Mọi lớp, service, API, UI component phải có code hoàn chỉnh.

Nếu có nhiều lựa chọn kỹ thuật, hãy chọn giải pháp phù hợp nhất cho hệ thống SaaS có khả năng mở rộng.

---

# PROJECT INFORMATION

## Project Name

```text
GiapTech.BindingDocx
```

## Root Folder

```text
E:\GIAPTECH\GiapTech.BindingDocx
```

---

# BUSINESS GOAL

Xây dựng hệ thống WEB cho phép:

1. Upload và quản lý template Word/Excel.
2. Import dữ liệu hồ sơ.
3. Mapping dữ liệu với template.
4. Binding dữ liệu vào template.
5. Preview kết quả.
6. Xuất hồ sơ hoàn chỉnh.
7. Xuất ZIP.
8. Quản lý token sử dụng.
9. Quản lý nhóm hồ sơ.
10. Hỗ trợ dữ liệu đơn và dữ liệu bảng.

Hệ thống phải được thiết kế để sau này phát triển thành SaaS đa khách hàng.

---

# TECHNOLOGY STACK

## Backend

```text
ASP.NET Core 9 Web API

Clean Architecture

Dapper

FluentValidation

JWT Authentication

Refresh Token

MediatR

Serilog

AutoMapper
```

---

## Database

Ưu tiên:

```text
SQL Server
```

Thiết kế abstraction để có thể thay thế bằng:

```text
PostgreSQL
```

---

## Frontend

```text
React

TypeScript

Vite

Ant Design

TanStack Query

TanStack Table

React Router

Axios

Zustand
```

---

## Document Processing

```text
OpenXML SDK

ClosedXML
```

---

## Preview

```text
PDF.js

Mammoth.js

SheetJS
```

---

## Storage

Thiết kế Provider Pattern.

Hỗ trợ:

```text
Local Storage

MinIO
```

Mặc định dùng Local Storage.

---

## Deployment

```text
Docker

Docker Compose

Nginx
```

---

# SYSTEM ARCHITECTURE

```text
GiapTech.BindingDocx

├── backend

│   ├── src

│   │   ├── GiapTech.BindingDocx.Api
│   │   ├── GiapTech.BindingDocx.Application
│   │   ├── GiapTech.BindingDocx.Domain
│   │   ├── GiapTech.BindingDocx.Infrastructure

│   └── tests

├── frontend

│   ├── src

│   ├── components

│   ├── pages

│   ├── layouts

│   ├── services

│   └── stores

├── storage

├── templates

├── output

├── database

└── docs
```

---

# CORE CONCEPT

Hệ thống phải hỗ trợ đồng thời:

## 1. Single Record

Ví dụ:

```json
{
  "fullName": "Nguyễn Văn A",
  "birthday": "1998-01-01",
  "address": "Hà Nội"
}
```

---

## 2. Tabular Data

Ví dụ:

```json
{
  "employees": [
    {
      "fullName": "A",
      "address": "HN"
    },
    {
      "fullName": "B",
      "address": "HCM"
    }
  ]
}
```

---

## 3. Mixed Data

```json
{
  "companyName": "ABC",

  "employees": [
    {
      "name": "A"
    },
    {
      "name": "B"
    }
  ]
}
```

---

# TEMPLATE ENGINE

Hệ thống phải hoạt động tương tự:

```text
Docxtemplater

Mustache
```

---

# SUPPORTED PLACEHOLDER

## Text

```text
{{FullName}}

{{Birthday}}

{{Address}}

{{Phone}}
```

---

## Image

```text
{{Avatar}}
```

Yêu cầu:

* Giữ tỷ lệ ảnh
* Không méo ảnh
* Không giảm chất lượng

---

## Repeater

```text
{{#Employees}}

{{FullName}}

{{Address}}

{{/Employees}}
```

---

## Conditional

```text
{{#HasMarried}}

Đã kết hôn

{{/HasMarried}}
```

---

# PROFILE GROUP SYSTEM

Hệ thống có:

```text
Nhóm hồ sơ 1
Nhóm hồ sơ 2
Nhóm hồ sơ 3
Nhóm hồ sơ 4
Nhóm hồ sơ 5
Nhóm hồ sơ 6
```

Mỗi nhóm có:

```text
Template riêng

Output riêng

Cấu hình riêng
```

Không được hardcode.

Toàn bộ phải đọc từ database.

---

# IMPORT SYSTEM

## IMPORT BUTTON

Toolbar chỉ có:

```text
Import
```

---

## IMPORT MODAL

Khi click Import mở Modal.

Kích thước:

```text
90vw

85vh
```

---

## STEP 1

Chọn loại dữ liệu.

```text
○ Hồ sơ đơn

○ Danh sách hồ sơ
```

---

## STEP 2

Upload.

Hỗ trợ:

```text
Excel

CSV

JSON
```

Drag Drop.

---

## STEP 3

Mapping dữ liệu.

Ví dụ:

```text
Excel Column A

↓

FullName
```

Người dùng được chỉnh sửa mapping.

---

## STEP 4

Preview dữ liệu.

DataGrid.

Hỗ trợ:

```text
Search

Sort

Filter

Paging
```

Sau xác nhận dữ liệu được đưa vào Workspace chính.

---

# TOKEN SYSTEM

## BUSINESS RULE

```text
1 hồ sơ xử lý = 1 token
```

Ví dụ:

```text
100 hồ sơ = 100 token
```

---

## VALIDATION

Trước khi Generate:

```text
Nếu token không đủ

=> Chặn xử lý

=> Hiển thị thông báo
```

---

# TOKEN PACKAGE

## Gói 1

```text
1 Token

10.000 VNĐ
```

---

## Gói 2

```text
500 Token

7.000 VNĐ / Token

Tổng 3.500.000 VNĐ
```

---

## Gói 3

```text
1000 Token

5.000 VNĐ / Token

Tổng 5.000.000 VNĐ
```

---

# PAYMENT DESIGN

Thiết kế abstraction.

Tương lai tích hợp:

```text
VNPay

MoMo

Banking QR
```

Hiện tại tạo MockPaymentProvider.

---

# OUTPUT SYSTEM

Ví dụ:

```text
Output

└── Nguyen Van A

    ├── DonDangKy.docx

    ├── HopDong.docx

    ├── ThongTin.xlsx

    └── Avatar.jpg
```

---

# ZIP EXPORT

Sau Generate.

Tự động tạo:

```text
NguyenVanA.zip
```

hoặc

```text
Batch_yyyyMMddHHmmss.zip
```

---

# UI/UX REQUIREMENTS

## DESIGN PHILOSOPHY

Thiết kế theo:

```text
Notion

Linear

GitHub

Google Drive
```

Ưu tiên:

```text
Content First

Single Workspace

Minimal UI

Fast Workflow
```

---

# MAIN LAYOUT

```text
┌────────────────────────────────────────────────────┐
│ Toolbar                                             │
├──────┬─────────────────────────────────────────────┤
│      │                                             │
│      │                                             │
│Tree  │               Main Workspace                │
│View  │                                             │
│      │                                             │
│      │                                             │
└──────┴─────────────────────────────────────────────┘
```

---

# TOOLBAR

Chỉ gồm:

```text
[Nhóm hồ sơ ▼]

[Import]

[Generate]

[Export ZIP]

[Token]

[Settings]
```

---

# COLLAPSIBLE SIDEBAR

Mặc định:

```text
260px
```

Thu gọn:

```text
60px
```

Yêu cầu:

```text
Expand

Collapse

Remember State
```

---

# SIDEBAR CONTENT

Hiển thị:

```text
📁 Hồ sơ A

   📄 Đơn đăng ký

   📄 Hợp đồng

📁 Hồ sơ B

   📄 Đơn đăng ký
```

TreeView.

Yêu cầu:

```text
Expand

Collapse

Refresh

Search

Double Click
```

---

# MAIN WORKSPACE

Chiếm phần lớn màn hình.

Không hiển thị form nhập liệu cố định.

Chỉ tập trung:

```text
Xem dữ liệu

Mapping

Preview
```

---

# TAB SYSTEM

```text
[Dữ liệu]

[Preview]

[Mapping]
```

---

# TAB 1 - DỮ LIỆU

Hiển thị:

```text
TanStack Table
```

Yêu cầu:

```text
Search

Filter

Sort

Pagination

Virtual Scroll

Multi Select
```

---

# TAB 2 - PREVIEW

Hiển thị tài liệu sau binding.

Hỗ trợ:

```text
DOCX

PDF

XLSX
```

Yêu cầu:

```text
Zoom

Fit Width

Fit Page

Download
```

---

# TAB 3 - MAPPING

Hiển thị:

```text
Template Field

↔

Source Field
```

Cho phép:

```text
Dropdown Mapping

Drag Drop Mapping
```

---

# TOKEN DRAWER

Không tạo trang riêng.

Khi click:

```text
Token
```

Mở Drawer bên phải.

Hiển thị:

```text
Token hiện tại

Lịch sử sử dụng

Lịch sử nạp

Danh sách gói token
```

---

# SETTINGS DRAWER

Không tạo trang riêng.

Mở Drawer.

Bao gồm:

```text
Template Folder

Output Folder

Storage

Theme

Language
```

---

# DATABASE TABLES

Bắt buộc tạo migration và seed data.

## ProfileGroup

```sql
Id
Name
Description
TemplatePath
CreatedAt
```

---

## TemplateFile

```sql
Id
GroupId
Name
FilePath
FileType
CreatedAt
```

---

## ImportBatch

```sql
Id
Name
Type
TotalRecords
CreatedAt
```

---

## ImportRecord

```sql
Id
BatchId
JsonData
CreatedAt
```

---

## UserToken

```sql
Id
CurrentToken
CreatedAt
UpdatedAt
```

---

## TokenTransaction

```sql
Id
Type
Amount
Description
CreatedAt
```

---

# SEED DATA

Tự động seed:

```text
6 nhóm hồ sơ

3 gói token

1 user mặc định
```

---

# NON FUNCTIONAL REQUIREMENTS

Bắt buộc:

* Clean Architecture
* SOLID
* Repository Pattern
* Dependency Injection
* Async Await
* Serilog
* Global Exception Middleware
* Validation Middleware
* API Versioning
* Swagger
* Docker Support
* Docker Compose
* Unit Test Friendly
* Không hardcode dữ liệu nghiệp vụ

---

# DELIVERABLE

Hãy triển khai theo từng phase.

Mỗi phase phải:

1. Build thành công.
2. Chạy được.
3. Có hướng dẫn chạy.
4. Có migration tương ứng.
5. Có commit message đề xuất.

Sau khi hoàn thành một phase phải dừng lại và chờ tôi trả lời:

```text
continue
```

mới được triển khai phase tiếp theo.

Không được tạo pseudo code.

Không được bỏ TODO.

Không được bỏ implementation.

Toàn bộ source code phải sẵn sàng để build và chạy thực tế.
