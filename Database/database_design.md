# THIẾT KẾ CƠ SỞ DỮ LIỆU
## Hệ thống Chấm công bằng Nhận diện Khuôn mặt

---

## Sơ đồ quan hệ (ERD)

```
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│ departments  │      │  positions   │      │ work_shifts  │
├──────────────┤      ├──────────────┤      ├──────────────┤
│ id       PK  │      │ id       PK  │      │ id       PK  │
│ code         │      │ code         │      │ code         │
│ name         │      │ name         │      │ name         │
│ manager_id FK│──┐   │ base_salary  │      │ start_time   │
└──────┬───────┘  │   └──────┬───────┘      │ end_time     │
       │          │          │              └──────┬───────┘
       │          │          │                     │
       ▼          │          ▼                     │
┌─────────────────┴──────────────────┐             │
│            employees               │             │
├────────────────────────────────────┤             │
│ id                             PK  │             │
│ code                               │             │
│ full_name                          │             │
│ department_id                  FK  │◄────────────┘
│ position_id                    FK  │
│ is_face_registered                 │
└────────┬──────────────┬────────────┘
         │              │
    ┌────┘              └────┐
    ▼                        ▼
┌──────────────┐      ┌────────────────────┐
│  face_data   │      │ attendance_records │
├──────────────┤      ├────────────────────┤
│ id       PK  │      │ id             PK  │
│ employee_id  │      │ employee_id    FK  │
│ encoding     │      │ shift_id       FK  │
│ image_path   │      │ date               │
│ quality      │      │ check_in           │
└──────────────┘      │ check_out          │
                      │ status             │
┌──────────────┐      └────────────────────┘
│    users     │
├──────────────┤      ┌────────────────────┐
│ id       PK  │◄─────│    audit_logs      │
│ employee_id  │      ├────────────────────┤
│ username     │      │ id             PK  │
│ role         │      │ user_id        FK  │
└──────────────┘      │ action             │
                      │ table_name         │
                      └────────────────────┘
```

---

## Bảng 1: departments (Phòng ban)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã phòng ban |
| 2 | code | VARCHAR(20) | NOT NULL, UNIQUE | Mã code (VD: HR, IT) |
| 3 | name | VARCHAR(100) | NOT NULL | Tên phòng ban |
| 4 | description | TEXT | | Mô tả |
| 5 | manager_id | INT | FK → employees(id) | Trưởng phòng |
| 6 | is_active | BOOLEAN | DEFAULT TRUE | Đang hoạt động |
| 7 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 8 | updated_at | TIMESTAMP | | Ngày cập nhật |

---

## Bảng 2: positions (Chức vụ)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã chức vụ |
| 2 | code | VARCHAR(20) | NOT NULL, UNIQUE | Mã code (VD: MGR) |
| 3 | name | VARCHAR(100) | NOT NULL | Tên chức vụ |
| 4 | description | TEXT | | Mô tả |
| 5 | base_salary | DECIMAL(15,0) | | Lương cơ bản |
| 6 | is_active | BOOLEAN | DEFAULT TRUE | Đang áp dụng |
| 7 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 8 | updated_at | TIMESTAMP | | Ngày cập nhật |

---

## Bảng 3: employees (Nhân viên)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã nhân viên |
| 2 | code | VARCHAR(20) | NOT NULL, UNIQUE | Mã NV (VD: NV001) |
| 3 | full_name | VARCHAR(100) | NOT NULL | Họ và tên |
| 4 | gender | VARCHAR(10) | CHECK (Nam/Nữ/Khác) | Giới tính |
| 5 | date_of_birth | DATE | | Ngày sinh |
| 6 | phone | VARCHAR(15) | | Số điện thoại |
| 7 | email | VARCHAR(100) | UNIQUE | Email |
| 8 | address | TEXT | | Địa chỉ |
| 9 | identity_card | VARCHAR(20) | UNIQUE | CMND/CCCD |
| 10 | department_id | INT | FK → departments(id) | Phòng ban |
| 11 | position_id | INT | FK → positions(id) | Chức vụ |
| 12 | hire_date | DATE | DEFAULT TODAY | Ngày vào làm |
| 13 | avatar | TEXT | | Ảnh đại diện |
| 14 | is_active | BOOLEAN | DEFAULT TRUE | Còn làm việc |
| 15 | is_face_registered | BOOLEAN | DEFAULT FALSE | Đã đăng ký Face ID |
| 16 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 17 | updated_at | TIMESTAMP | | Ngày cập nhật |

---

## Bảng 4: users (Tài khoản)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã user |
| 2 | username | VARCHAR(50) | NOT NULL, UNIQUE | Tên đăng nhập |
| 3 | password_hash | VARCHAR(255) | NOT NULL | Mật khẩu (hash) |
| 4 | employee_id | INT | FK → employees(id) | Nhân viên liên kết |
| 5 | role | VARCHAR(20) | CHECK (Admin/Manager/User) | Quyền hạn |
| 6 | is_active | BOOLEAN | DEFAULT TRUE | Đang hoạt động |
| 7 | last_login | TIMESTAMP | | Đăng nhập cuối |
| 8 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 9 | updated_at | TIMESTAMP | | Ngày cập nhật |

---

## Bảng 5: face_data (Dữ liệu khuôn mặt)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã Face ID |
| 2 | employee_id | INT | NOT NULL, FK, ON DELETE CASCADE | Nhân viên |
| 3 | encoding | BYTEA | NOT NULL | Vector encoding (128D) |
| 4 | image_path | TEXT | | Đường dẫn ảnh |
| 5 | image_index | SMALLINT | CHECK (1-5) | Số thứ tự ảnh |
| 6 | quality | DECIMAL(5,2) | | Chất lượng (%) |
| 7 | is_active | BOOLEAN | DEFAULT TRUE | Đang sử dụng |
| 8 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 9 | updated_at | TIMESTAMP | | Ngày cập nhật |

---

## Bảng 6: work_shifts (Ca làm việc)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã ca |
| 2 | code | VARCHAR(20) | NOT NULL, UNIQUE | Mã code (VD: MAIN) |
| 3 | name | VARCHAR(100) | NOT NULL | Tên ca |
| 4 | start_time | TIME | NOT NULL | Giờ bắt đầu |
| 5 | end_time | TIME | NOT NULL | Giờ kết thúc |
| 6 | break_minutes | SMALLINT | DEFAULT 0 | Nghỉ giữa ca (phút) |
| 7 | late_threshold | SMALLINT | DEFAULT 15 | Ngưỡng đi muộn (phút) |
| 8 | early_threshold | SMALLINT | DEFAULT 15 | Ngưỡng về sớm (phút) |
| 9 | is_active | BOOLEAN | DEFAULT TRUE | Đang áp dụng |
| 10 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 11 | updated_at | TIMESTAMP | | Ngày cập nhật |

---

## Bảng 7: attendance_records (Chấm công)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | BIGSERIAL | PK | Mã bản ghi |
| 2 | employee_id | INT | NOT NULL, FK | Nhân viên |
| 3 | date | DATE | NOT NULL, DEFAULT TODAY | Ngày chấm công |
| 4 | shift_id | INT | FK → work_shifts(id) | Ca làm việc |
| 5 | check_in | TIMESTAMP | | Giờ vào |
| 6 | check_out | TIMESTAMP | | Giờ ra |
| 7 | check_in_image | TEXT | | Ảnh Check-in |
| 8 | check_out_image | TEXT | | Ảnh Check-out |
| 9 | check_in_confidence | DECIMAL(5,2) | | Độ tin cậy vào (%) |
| 10 | check_out_confidence | DECIMAL(5,2) | | Độ tin cậy ra (%) |
| 11 | status | VARCHAR(20) | CHECK (...) | Trạng thái |
| 12 | late_minutes | SMALLINT | DEFAULT 0 | Phút đi muộn |
| 13 | early_minutes | SMALLINT | DEFAULT 0 | Phút về sớm |
| 14 | working_hours | DECIMAL(4,2) | | Giờ làm việc |
| 15 | overtime_hours | DECIMAL(4,2) | DEFAULT 0 | Giờ tăng ca |
| 16 | note | TEXT | | Ghi chú |
| 17 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 18 | updated_at | TIMESTAMP | | Ngày cập nhật |

**Ràng buộc:** UNIQUE (employee_id, date) - Mỗi NV chỉ 1 bản ghi/ngày

**Status values:** Present, Late, Early, Absent, Leave, Holiday

---

## Bảng 8: leave_requests (Đơn nghỉ phép)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã đơn |
| 2 | employee_id | INT | NOT NULL, FK | Nhân viên |
| 3 | leave_type | VARCHAR(20) | CHECK (...) | Loại nghỉ |
| 4 | start_date | DATE | NOT NULL | Từ ngày |
| 5 | end_date | DATE | NOT NULL | Đến ngày |
| 6 | total_days | DECIMAL(4,1) | NOT NULL | Số ngày nghỉ |
| 7 | reason | TEXT | | Lý do |
| 8 | status | VARCHAR(20) | CHECK (...) | Trạng thái |
| 9 | approved_by | INT | FK → users(id) | Người duyệt |
| 10 | approved_at | TIMESTAMP | | Ngày duyệt |
| 11 | reject_reason | TEXT | | Lý do từ chối |
| 12 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 13 | updated_at | TIMESTAMP | | Ngày cập nhật |

**Leave types:** Annual, Sick, Personal, Maternity, Unpaid, Other

**Status values:** Pending, Approved, Rejected, Cancelled

---

## Bảng 9: holidays (Ngày lễ)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã ngày lễ |
| 2 | date | DATE | NOT NULL, UNIQUE | Ngày |
| 3 | name | VARCHAR(100) | NOT NULL | Tên ngày lễ |
| 4 | description | TEXT | | Mô tả |
| 5 | is_recurring | BOOLEAN | DEFAULT FALSE | Lặp hàng năm |
| 6 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |

---

## Bảng 10: system_settings (Cài đặt)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | SERIAL | PK | Mã cài đặt |
| 2 | key | VARCHAR(100) | NOT NULL, UNIQUE | Khóa |
| 3 | value | TEXT | | Giá trị |
| 4 | type | VARCHAR(20) | CHECK (...) | Kiểu dữ liệu |
| 5 | description | TEXT | | Mô tả |
| 6 | is_editable | BOOLEAN | DEFAULT TRUE | Cho phép sửa |
| 7 | created_at | TIMESTAMP | DEFAULT NOW | Ngày tạo |
| 8 | updated_at | TIMESTAMP | | Ngày cập nhật |

**Types:** String, Int, Decimal, Boolean, Json

---

## Bảng 11: audit_logs (Nhật ký)

| STT | Tên cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
|:---:|---------|--------------|-----------|-------|
| 1 | id | BIGSERIAL | PK | Mã log |
| 2 | user_id | INT | FK → users(id) | Người thao tác |
| 3 | action | VARCHAR(50) | NOT NULL | Hành động |
| 4 | table_name | VARCHAR(50) | | Bảng tác động |
| 5 | record_id | VARCHAR(20) | | ID bản ghi |
| 6 | old_data | JSONB | | Dữ liệu cũ |
| 7 | new_data | JSONB | | Dữ liệu mới |
| 8 | ip_address | VARCHAR(45) | | Địa chỉ IP |
| 9 | created_at | TIMESTAMP | DEFAULT NOW | Thời điểm |

---

## Indexes

| Bảng | Index | Cột |
|------|-------|-----|
| employees | idx_employees_dept | department_id |
| employees | idx_employees_position | position_id |
| employees | idx_employees_active | is_active |
| face_data | idx_face_data_employee | employee_id |
| attendance_records | idx_attendance_employee | employee_id |
| attendance_records | idx_attendance_date | date |
| attendance_records | idx_attendance_status | status |
| attendance_records | idx_attendance_emp_date | employee_id, date |
| leave_requests | idx_leave_employee | employee_id |
| leave_requests | idx_leave_status | status |
| audit_logs | idx_audit_user | user_id |
| audit_logs | idx_audit_created | created_at |
| audit_logs | idx_audit_table | table_name |

---

## Views

| View | Mô tả |
|------|-------|
| v_today_attendance | Chấm công hôm nay |
| v_monthly_summary | Tổng hợp theo tháng |
| v_employees_no_face | NV chưa đăng ký Face ID |

---

## Triggers

| Trigger | Bảng | Chức năng |
|---------|------|-----------|
| trg_*_updated | Tất cả | Tự động cập nhật updated_at |
| trg_face_status | face_data | Cập nhật is_face_registered |
