-- ============================================
-- HỆ THỐNG CHẤM CÔNG BẰNG NHẬN DIỆN KHUÔN MẶT
-- Face ID Attendance System - PostgreSQL
-- Version: 1.0
-- ============================================

-- ============================================
-- BƯỚC 1: TẠO DATABASE (chạy riêng trong postgres)
-- ============================================
-- CREATE DATABASE face_id_attendance WITH ENCODING = 'UTF8';

-- ============================================
-- BƯỚC 2: KẾT NỐI VÀ TẠO CÁC BẢNG
-- ============================================

-- Xóa các bảng cũ nếu tồn tại (theo thứ tự phụ thuộc)
DROP TABLE IF EXISTS audit_logs CASCADE;
DROP TABLE IF EXISTS leave_requests CASCADE;
DROP TABLE IF EXISTS attendance_records CASCADE;
DROP TABLE IF EXISTS face_data CASCADE;
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS employees CASCADE;
DROP TABLE IF EXISTS work_shifts CASCADE;
DROP TABLE IF EXISTS holidays CASCADE;
DROP TABLE IF EXISTS system_settings CASCADE;
DROP TABLE IF EXISTS positions CASCADE;
DROP TABLE IF EXISTS departments CASCADE;

-- ============================================
-- BẢNG 1: departments (Phòng ban)
-- ============================================
CREATE TABLE departments (
    id              SERIAL          PRIMARY KEY,
    code            VARCHAR(20)     NOT NULL UNIQUE,
    name            VARCHAR(100)    NOT NULL,
    description     TEXT,
    manager_id      INT,
    is_active       BOOLEAN         DEFAULT TRUE,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- ============================================
-- BẢNG 2: positions (Chức vụ)
-- ============================================
CREATE TABLE positions (
    id              SERIAL          PRIMARY KEY,
    code            VARCHAR(20)     NOT NULL UNIQUE,
    name            VARCHAR(100)    NOT NULL,
    description     TEXT,
    base_salary     DECIMAL(15,0),
    is_active       BOOLEAN         DEFAULT TRUE,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- ============================================
-- BẢNG 3: employees (Nhân viên)
-- ============================================
CREATE TABLE employees (
    id              SERIAL          PRIMARY KEY,
    code            VARCHAR(20)     NOT NULL UNIQUE,
    full_name       VARCHAR(100)    NOT NULL,
    gender          VARCHAR(10)     CHECK (gender IN ('Nam', 'Nữ', 'Khác')),
    date_of_birth   DATE,
    phone           VARCHAR(15),
    email           VARCHAR(100)    UNIQUE,
    address         TEXT,
    identity_card   VARCHAR(20)     UNIQUE,
    department_id   INT             REFERENCES departments(id),
    position_id     INT             REFERENCES positions(id),
    hire_date       DATE            DEFAULT CURRENT_DATE,
    avatar          TEXT,
    is_active       BOOLEAN         DEFAULT TRUE,
    is_face_registered BOOLEAN      DEFAULT FALSE,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- Thêm FK cho manager_id
ALTER TABLE departments ADD CONSTRAINT fk_dept_manager 
    FOREIGN KEY (manager_id) REFERENCES employees(id);

-- ============================================
-- BẢNG 4: users (Tài khoản)
-- ============================================
CREATE TABLE users (
    id              SERIAL          PRIMARY KEY,
    username        VARCHAR(50)     NOT NULL UNIQUE,
    password_hash   VARCHAR(255)    NOT NULL,
    employee_id     INT             REFERENCES employees(id),
    role            VARCHAR(20)     DEFAULT 'User' CHECK (role IN ('Admin', 'Manager', 'User')),
    is_active       BOOLEAN         DEFAULT TRUE,
    last_login      TIMESTAMP,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- ============================================
-- BẢNG 5: face_data (Dữ liệu khuôn mặt)
-- ============================================
CREATE TABLE face_data (
    id              SERIAL          PRIMARY KEY,
    employee_id     INT             NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
    encoding        BYTEA           NOT NULL,
    image_path      TEXT,
    image_index     SMALLINT        DEFAULT 1 CHECK (image_index BETWEEN 1 AND 5),
    quality         DECIMAL(5,2),
    is_active       BOOLEAN         DEFAULT TRUE,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- ============================================
-- BẢNG 6: work_shifts (Ca làm việc)
-- ============================================
CREATE TABLE work_shifts (
    id              SERIAL          PRIMARY KEY,
    code            VARCHAR(20)     NOT NULL UNIQUE,
    name            VARCHAR(100)    NOT NULL,
    start_time      TIME            NOT NULL,
    end_time        TIME            NOT NULL,
    break_minutes   SMALLINT        DEFAULT 0,
    late_threshold  SMALLINT        DEFAULT 15,
    early_threshold SMALLINT        DEFAULT 15,
    is_active       BOOLEAN         DEFAULT TRUE,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- ============================================
-- BẢNG 7: attendance_records (Chấm công)
-- ============================================
CREATE TABLE attendance_records (
    id              BIGSERIAL       PRIMARY KEY,
    employee_id     INT             NOT NULL REFERENCES employees(id),
    date            DATE            NOT NULL DEFAULT CURRENT_DATE,
    shift_id        INT             REFERENCES work_shifts(id),
    check_in        TIMESTAMP,
    check_out       TIMESTAMP,
    check_in_image  TEXT,
    check_out_image TEXT,
    check_in_confidence  DECIMAL(5,2),
    check_out_confidence DECIMAL(5,2),
    status          VARCHAR(20)     DEFAULT 'Present' 
                    CHECK (status IN ('Present', 'Late', 'Early', 'Absent', 'Leave', 'Holiday')),
    late_minutes    SMALLINT        DEFAULT 0,
    early_minutes   SMALLINT        DEFAULT 0,
    working_hours   DECIMAL(4,2),
    overtime_hours  DECIMAL(4,2)    DEFAULT 0,
    note            TEXT,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP,
    
    UNIQUE (employee_id, date)
);

-- ============================================
-- BẢNG 8: leave_requests (Đơn nghỉ phép)
-- ============================================
CREATE TABLE leave_requests (
    id              SERIAL          PRIMARY KEY,
    employee_id     INT             NOT NULL REFERENCES employees(id),
    leave_type      VARCHAR(20)     NOT NULL CHECK (leave_type IN ('Annual', 'Sick', 'Personal', 'Maternity', 'Unpaid', 'Other')),
    start_date      DATE            NOT NULL,
    end_date        DATE            NOT NULL,
    total_days      DECIMAL(4,1)    NOT NULL,
    reason          TEXT,
    status          VARCHAR(20)     DEFAULT 'Pending' CHECK (status IN ('Pending', 'Approved', 'Rejected', 'Cancelled')),
    approved_by     INT             REFERENCES users(id),
    approved_at     TIMESTAMP,
    reject_reason   TEXT,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- ============================================
-- BẢNG 9: holidays (Ngày lễ)
-- ============================================
CREATE TABLE holidays (
    id              SERIAL          PRIMARY KEY,
    date            DATE            NOT NULL UNIQUE,
    name            VARCHAR(100)    NOT NULL,
    description     TEXT,
    is_recurring    BOOLEAN         DEFAULT FALSE,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- BẢNG 10: system_settings (Cài đặt)
-- ============================================
CREATE TABLE system_settings (
    id              SERIAL          PRIMARY KEY,
    key             VARCHAR(100)    NOT NULL UNIQUE,
    value           TEXT,
    type            VARCHAR(20)     DEFAULT 'String' CHECK (type IN ('String', 'Int', 'Decimal', 'Boolean', 'Json')),
    description     TEXT,
    is_editable     BOOLEAN         DEFAULT TRUE,
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- ============================================
-- BẢNG 11: audit_logs (Nhật ký)
-- ============================================
CREATE TABLE audit_logs (
    id              BIGSERIAL       PRIMARY KEY,
    user_id         INT             REFERENCES users(id),
    action          VARCHAR(50)     NOT NULL,
    table_name      VARCHAR(50),
    record_id       VARCHAR(20),
    old_data        JSONB,
    new_data        JSONB,
    ip_address      VARCHAR(45),
    created_at      TIMESTAMP       DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- INDEXES
-- ============================================
CREATE INDEX idx_employees_dept ON employees(department_id);
CREATE INDEX idx_employees_position ON employees(position_id);
CREATE INDEX idx_employees_active ON employees(is_active);

CREATE INDEX idx_face_data_employee ON face_data(employee_id);

CREATE INDEX idx_attendance_employee ON attendance_records(employee_id);
CREATE INDEX idx_attendance_date ON attendance_records(date);
CREATE INDEX idx_attendance_status ON attendance_records(status);
CREATE INDEX idx_attendance_emp_date ON attendance_records(employee_id, date);

CREATE INDEX idx_leave_employee ON leave_requests(employee_id);
CREATE INDEX idx_leave_status ON leave_requests(status);

CREATE INDEX idx_audit_user ON audit_logs(user_id);
CREATE INDEX idx_audit_created ON audit_logs(created_at);
CREATE INDEX idx_audit_table ON audit_logs(table_name);

-- ============================================
-- TRIGGER: Auto update updated_at
-- ============================================
CREATE OR REPLACE FUNCTION fn_update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_departments_updated BEFORE UPDATE ON departments FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_positions_updated BEFORE UPDATE ON positions FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_employees_updated BEFORE UPDATE ON employees FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_users_updated BEFORE UPDATE ON users FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_face_data_updated BEFORE UPDATE ON face_data FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_work_shifts_updated BEFORE UPDATE ON work_shifts FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_attendance_updated BEFORE UPDATE ON attendance_records FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_leave_updated BEFORE UPDATE ON leave_requests FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_settings_updated BEFORE UPDATE ON system_settings FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

-- ============================================
-- TRIGGER: Auto update is_face_registered
-- ============================================
CREATE OR REPLACE FUNCTION fn_update_face_status()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE employees SET is_face_registered = TRUE WHERE id = NEW.employee_id;
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE employees SET is_face_registered = EXISTS(
            SELECT 1 FROM face_data WHERE employee_id = OLD.employee_id AND is_active = TRUE
        ) WHERE id = OLD.employee_id;
        RETURN OLD;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_face_status 
AFTER INSERT OR DELETE ON face_data 
FOR EACH ROW EXECUTE FUNCTION fn_update_face_status();

-- ============================================
-- VIEWS
-- ============================================

-- View: Chấm công hôm nay
CREATE VIEW v_today_attendance AS
SELECT 
    e.id,
    e.code,
    e.full_name,
    d.name AS department,
    p.name AS position,
    a.check_in,
    a.check_out,
    a.status,
    a.late_minutes,
    a.working_hours
FROM employees e
LEFT JOIN attendance_records a ON e.id = a.employee_id AND a.date = CURRENT_DATE
LEFT JOIN departments d ON e.department_id = d.id
LEFT JOIN positions p ON e.position_id = p.id
WHERE e.is_active = TRUE;

-- View: Tổng hợp chấm công tháng
CREATE VIEW v_monthly_summary AS
SELECT 
    e.id,
    e.code,
    e.full_name,
    d.name AS department,
    DATE_TRUNC('month', a.date) AS month,
    COUNT(*) FILTER (WHERE a.status = 'Present') AS present_days,
    COUNT(*) FILTER (WHERE a.status = 'Late') AS late_days,
    COUNT(*) FILTER (WHERE a.status = 'Absent') AS absent_days,
    COUNT(*) FILTER (WHERE a.status = 'Leave') AS leave_days,
    SUM(COALESCE(a.late_minutes, 0)) AS total_late_minutes,
    SUM(COALESCE(a.working_hours, 0)) AS total_working_hours,
    SUM(COALESCE(a.overtime_hours, 0)) AS total_overtime_hours
FROM employees e
LEFT JOIN attendance_records a ON e.id = a.employee_id
LEFT JOIN departments d ON e.department_id = d.id
WHERE e.is_active = TRUE
GROUP BY e.id, e.code, e.full_name, d.name, DATE_TRUNC('month', a.date);

-- View: Nhân viên chưa đăng ký Face ID
CREATE VIEW v_employees_no_face AS
SELECT id, code, full_name, department_id
FROM employees 
WHERE is_active = TRUE AND is_face_registered = FALSE;

-- ============================================
-- DỮ LIỆU MẪU
-- ============================================

-- Phòng ban
INSERT INTO departments (code, name, description) VALUES
('HR', 'Phòng Nhân sự', 'Quản lý nhân sự, tuyển dụng'),
('IT', 'Phòng Công nghệ', 'Phát triển và vận hành hệ thống'),
('SALES', 'Phòng Kinh doanh', 'Bán hàng và CSKH'),
('ACCT', 'Phòng Kế toán', 'Tài chính, kế toán'),
('ADMIN', 'Phòng Hành chính', 'Hành chính văn phòng');

-- Chức vụ
INSERT INTO positions (code, name, base_salary) VALUES
('DIR', 'Giám đốc', 50000000),
('MGR', 'Trưởng phòng', 25000000),
('LEAD', 'Trưởng nhóm', 18000000),
('SR', 'Nhân viên cao cấp', 15000000),
('JR', 'Nhân viên', 10000000),
('INT', 'Thực tập sinh', 5000000);

-- Ca làm việc
INSERT INTO work_shifts (code, name, start_time, end_time, break_minutes) VALUES
('MAIN', 'Ca hành chính', '08:00', '17:00', 60),
('MORNING', 'Ca sáng', '06:00', '14:00', 30),
('AFTERNOON', 'Ca chiều', '14:00', '22:00', 30),
('NIGHT', 'Ca đêm', '22:00', '06:00', 30);

-- Ngày lễ 2026
INSERT INTO holidays (date, name, is_recurring) VALUES
('2026-01-01', 'Tết Dương lịch', TRUE),
('2026-02-16', 'Nghỉ Tết Nguyên đán', FALSE),
('2026-02-17', 'Nghỉ Tết Nguyên đán', FALSE),
('2026-02-18', 'Tết Nguyên đán', FALSE),
('2026-02-19', 'Nghỉ Tết Nguyên đán', FALSE),
('2026-02-20', 'Nghỉ Tết Nguyên đán', FALSE),
('2026-04-06', 'Giỗ Tổ Hùng Vương', FALSE),
('2026-04-30', 'Giải phóng miền Nam', TRUE),
('2026-05-01', 'Quốc tế Lao động', TRUE),
('2026-09-02', 'Quốc khánh', TRUE);

-- Cài đặt hệ thống
INSERT INTO system_settings (key, value, type, description) VALUES
('company_name', 'Công ty ABC', 'String', 'Tên công ty'),
('work_start_time', '08:00', 'String', 'Giờ vào làm'),
('work_end_time', '17:00', 'String', 'Giờ tan làm'),
('late_threshold', '15', 'Int', 'Ngưỡng đi muộn (phút)'),
('early_threshold', '15', 'Int', 'Ngưỡng về sớm (phút)'),
('face_threshold', '0.6', 'Decimal', 'Ngưỡng nhận diện (0-1)'),
('max_face_images', '5', 'Int', 'Số ảnh Face ID tối đa'),
('camera_index', '0', 'Int', 'Camera index'),
('auto_checkout', '23:59', 'String', 'Giờ tự động checkout'),
('require_checkout', 'true', 'Boolean', 'Bắt buộc checkout');

-- User Admin (password cần hash thực tế)
INSERT INTO users (username, password_hash, role) VALUES
('admin', '$2a$10$HASH_PASSWORD_HERE', 'Admin');

-- Nhân viên mẫu
INSERT INTO employees (code, full_name, gender, phone, email, department_id, position_id) VALUES
('NV001', 'Nguyễn Văn An', 'Nam', '0901234567', 'an.nv@company.com', 2, 2),
('NV002', 'Trần Thị Bình', 'Nữ', '0901234568', 'binh.tt@company.com', 1, 3),
('NV003', 'Lê Văn Cường', 'Nam', '0901234569', 'cuong.lv@company.com', 2, 5),
('NV004', 'Phạm Thị Dung', 'Nữ', '0901234570', 'dung.pt@company.com', 3, 5),
('NV005', 'Hoàng Văn Em', 'Nam', '0901234571', 'em.hv@company.com', 4, 4);
