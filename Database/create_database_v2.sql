-- ============================================
-- HỆ THỐNG CHẤM CÔNG BẰNG NHẬN DIỆN KHUÔN MẶT
-- Face ID Attendance System - PostgreSQL
-- Version: 2.0 (Optimized)
-- ============================================

-- ============================================
-- BƯỚC 1: TẠO DATABASE (chạy riêng trong postgres)
-- ============================================
-- CREATE DATABASE face_id_attendance
--     WITH ENCODING = 'UTF8'
--     LC_COLLATE = 'vi_VN.UTF-8'
--     LC_CTYPE = 'vi_VN.UTF-8';

-- ============================================
-- BƯỚC 2: THIẾT LẬP TIMEZONE
-- ============================================
SET timezone = 'Asia/Ho_Chi_Minh';

-- ============================================
-- BƯỚC 3: XÓA CÁC BẢNG CŨ (theo thứ tự phụ thuộc)
-- ============================================
DROP TABLE IF EXISTS audit_logs CASCADE;
DROP TABLE IF EXISTS leave_requests CASCADE;
DROP TABLE IF EXISTS attendance_records CASCADE;
DROP TABLE IF EXISTS employee_shifts CASCADE;
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
-- Hỗ trợ phòng ban đa cấp qua parent_id
-- ============================================
CREATE TABLE departments (
    id              SERIAL          PRIMARY KEY,
    code            VARCHAR(20)     NOT NULL UNIQUE,
    name            VARCHAR(100)    NOT NULL,
    description     TEXT,
    parent_id       INT             REFERENCES departments(id) ON DELETE SET NULL,
    manager_id      INT,            -- FK sẽ thêm sau khi tạo bảng employees
    is_active       BOOLEAN         NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMPTZ
);

COMMENT ON TABLE departments IS 'Quản lý phòng ban, hỗ trợ cấu trúc đa cấp';
COMMENT ON COLUMN departments.parent_id IS 'Phòng ban cha (NULL = phòng ban gốc)';
COMMENT ON COLUMN departments.manager_id IS 'Trưởng phòng (FK → employees)';

-- ============================================
-- BẢNG 2: positions (Chức vụ)
-- ============================================
CREATE TABLE positions (
    id              SERIAL          PRIMARY KEY,
    code            VARCHAR(20)     NOT NULL UNIQUE,
    name            VARCHAR(100)    NOT NULL,
    description     TEXT,
    base_salary     NUMERIC(15,2),
    is_active       BOOLEAN         NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMPTZ
);

COMMENT ON TABLE positions IS 'Danh mục chức vụ và mức lương cơ bản';
COMMENT ON COLUMN positions.base_salary IS 'Lương cơ bản (VNĐ), hỗ trợ 2 chữ số thập phân';

-- ============================================
-- BẢNG 3: work_shifts (Ca làm việc)
-- ============================================
CREATE TABLE work_shifts (
    id              SERIAL          PRIMARY KEY,
    code            VARCHAR(20)     NOT NULL UNIQUE,
    name            VARCHAR(100)    NOT NULL,
    start_time      TIME            NOT NULL,
    end_time        TIME            NOT NULL,
    break_minutes   SMALLINT        NOT NULL DEFAULT 0 CHECK (break_minutes >= 0),
    late_threshold  SMALLINT        NOT NULL DEFAULT 15 CHECK (late_threshold >= 0),
    early_threshold SMALLINT        NOT NULL DEFAULT 15 CHECK (early_threshold >= 0),
    is_overnight    BOOLEAN         NOT NULL DEFAULT FALSE,
    is_active       BOOLEAN         NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMPTZ
);

COMMENT ON TABLE work_shifts IS 'Định nghĩa các ca làm việc';
COMMENT ON COLUMN work_shifts.is_overnight IS 'Ca đêm: end_time thuộc ngày tiếp theo';
COMMENT ON COLUMN work_shifts.late_threshold IS 'Ngưỡng đi muộn (phút) trước khi tính là trễ';
COMMENT ON COLUMN work_shifts.early_threshold IS 'Ngưỡng về sớm (phút) trước khi tính là về sớm';

-- ============================================
-- BẢNG 4: employees (Nhân viên)
-- ============================================
CREATE TABLE employees (
    id                  SERIAL          PRIMARY KEY,
    code                VARCHAR(20)     NOT NULL UNIQUE,
    full_name           VARCHAR(100)    NOT NULL,
    gender              CHAR(1)         CHECK (gender IN ('M', 'F', 'O')),
    date_of_birth       DATE,
    phone               VARCHAR(15),
    email               VARCHAR(100)    UNIQUE,
    address             TEXT,
    identity_card       VARCHAR(20)     UNIQUE,
    department_id       INT             REFERENCES departments(id) ON DELETE SET NULL,
    position_id         INT             REFERENCES positions(id) ON DELETE SET NULL,
    default_shift_id    INT             REFERENCES work_shifts(id) ON DELETE SET NULL,
    hire_date           DATE            NOT NULL DEFAULT CURRENT_DATE,
    termination_date    DATE,
    avatar              TEXT,
    is_active           BOOLEAN         NOT NULL DEFAULT TRUE,
    is_face_registered  BOOLEAN         NOT NULL DEFAULT FALSE,
    created_at          TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          TIMESTAMPTZ,

    CONSTRAINT chk_employee_dates CHECK (
        termination_date IS NULL OR termination_date >= hire_date
    )
);

COMMENT ON TABLE employees IS 'Thông tin nhân viên';
COMMENT ON COLUMN employees.gender IS 'M=Male, F=Female, O=Other — hiển thị label ở app layer';
COMMENT ON COLUMN employees.default_shift_id IS 'Ca làm việc mặc định của nhân viên';
COMMENT ON COLUMN employees.termination_date IS 'Ngày nghỉ việc (NULL = đang làm)';
COMMENT ON COLUMN employees.is_face_registered IS 'Auto-updated bởi trigger trên face_data';

-- Thêm FK cho departments.manager_id
ALTER TABLE departments ADD CONSTRAINT fk_dept_manager
    FOREIGN KEY (manager_id) REFERENCES employees(id) ON DELETE SET NULL;

-- ============================================
-- BẢNG 5: users (Tài khoản đăng nhập)
-- ============================================
CREATE TABLE users (
    id                  SERIAL          PRIMARY KEY,
    username            VARCHAR(50)     NOT NULL UNIQUE,
    password_hash       VARCHAR(255)    NOT NULL,
    employee_id         INT             UNIQUE REFERENCES employees(id) ON DELETE SET NULL,
    role                VARCHAR(20)     NOT NULL DEFAULT 'User'
                        CHECK (role IN ('Admin', 'Manager', 'User')),
    is_active           BOOLEAN         NOT NULL DEFAULT TRUE,
    last_login          TIMESTAMPTZ,
    failed_login_count  SMALLINT        NOT NULL DEFAULT 0,
    locked_until        TIMESTAMPTZ,
    created_at          TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          TIMESTAMPTZ
);

COMMENT ON TABLE users IS 'Tài khoản đăng nhập hệ thống';
COMMENT ON COLUMN users.employee_id IS 'UNIQUE: 1 nhân viên = 1 tài khoản (1-1)';
COMMENT ON COLUMN users.failed_login_count IS 'Số lần đăng nhập sai liên tiếp';
COMMENT ON COLUMN users.locked_until IS 'Khóa tài khoản tạm thời đến thời điểm này';

-- ============================================
-- BẢNG 6: face_data (Dữ liệu khuôn mặt)
-- ============================================
CREATE TABLE face_data (
    id              SERIAL          PRIMARY KEY,
    employee_id     INT             NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
    encoding        BYTEA           NOT NULL,
    image_path      TEXT,
    image_index     SMALLINT        NOT NULL DEFAULT 1 CHECK (image_index BETWEEN 1 AND 5),
    quality         REAL            CHECK (quality >= 0 AND quality <= 1),
    is_active       BOOLEAN         NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMPTZ,

    CONSTRAINT uq_face_employee_index UNIQUE (employee_id, image_index)
);

COMMENT ON TABLE face_data IS 'Face embeddings (128D vectors) cho nhận diện khuôn mặt';
COMMENT ON COLUMN face_data.encoding IS 'Vector encoding 128 chiều (dlib/FaceNet)';
COMMENT ON COLUMN face_data.quality IS 'Chất lượng ảnh: 0.0 - 1.0';
COMMENT ON COLUMN face_data.image_index IS 'Vị trí ảnh (1-5), mỗi NV tối đa 5 ảnh';

-- ============================================
-- BẢNG 7: employee_shifts (Lịch ca nhân viên)
-- Hỗ trợ xoay ca và override ca mặc định
-- ============================================
CREATE TABLE employee_shifts (
    id              SERIAL          PRIMARY KEY,
    employee_id     INT             NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
    shift_id        INT             NOT NULL REFERENCES work_shifts(id) ON DELETE CASCADE,
    effective_date  DATE            NOT NULL,
    end_date        DATE,
    created_by      INT             REFERENCES employees(id) ON DELETE SET NULL,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_shift_dates CHECK (
        end_date IS NULL OR end_date >= effective_date
    ),
    CONSTRAINT uq_employee_shift_date UNIQUE (employee_id, effective_date)
);

COMMENT ON TABLE employee_shifts IS 'Phân ca theo ngày cho nhân viên (override default_shift_id)';
COMMENT ON COLUMN employee_shifts.effective_date IS 'Ngày bắt đầu áp dụng ca này';
COMMENT ON COLUMN employee_shifts.end_date IS 'Ngày kết thúc (NULL = vô thời hạn)';

-- ============================================
-- BẢNG 8: attendance_records (Chấm công)
-- PARTITIONED theo tháng để tối ưu query báo cáo
-- ============================================
CREATE TABLE attendance_records (
    id              BIGSERIAL,
    employee_id     INT             NOT NULL REFERENCES employees(id) ON DELETE RESTRICT,
    attendance_date DATE            NOT NULL DEFAULT CURRENT_DATE,
    shift_id        INT             REFERENCES work_shifts(id) ON DELETE SET NULL,
    check_in        TIMESTAMPTZ,
    check_out       TIMESTAMPTZ,
    check_in_image  TEXT,
    check_out_image TEXT,
    check_in_confidence  REAL,
    check_out_confidence REAL,
    status          VARCHAR(20)     NOT NULL DEFAULT 'Present'
                    CHECK (status IN ('Present', 'Late', 'EarlyLeave', 'Absent', 'Leave', 'Holiday')),
    late_minutes    SMALLINT        NOT NULL DEFAULT 0 CHECK (late_minutes >= 0),
    early_minutes   SMALLINT        NOT NULL DEFAULT 0 CHECK (early_minutes >= 0),
    working_hours   DECIMAL(5,2)    CHECK (working_hours >= 0),
    overtime_hours  DECIMAL(5,2)    NOT NULL DEFAULT 0 CHECK (overtime_hours >= 0),
    note            TEXT,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMPTZ,

    PRIMARY KEY (id, attendance_date),

    CONSTRAINT uq_attendance_emp_date UNIQUE (employee_id, attendance_date),
    CONSTRAINT chk_checkout_after_checkin CHECK (
        check_out IS NULL OR check_in IS NULL OR check_out >= check_in
    ),
    CONSTRAINT chk_confidence_range CHECK (
        (check_in_confidence IS NULL OR (check_in_confidence >= 0 AND check_in_confidence <= 1)) AND
        (check_out_confidence IS NULL OR (check_out_confidence >= 0 AND check_out_confidence <= 1))
    )
) PARTITION BY RANGE (attendance_date);

COMMENT ON TABLE attendance_records IS 'Bản ghi chấm công — partitioned theo tháng';
COMMENT ON COLUMN attendance_records.attendance_date IS 'Ngày chấm công (partition key)';
COMMENT ON COLUMN attendance_records.status IS 'Present/Late/EarlyLeave/Absent/Leave/Holiday';
COMMENT ON COLUMN attendance_records.check_in_confidence IS 'Độ tin cậy nhận diện: 0.0 - 1.0';

-- Tạo partitions cho 2026
CREATE TABLE attendance_2026_01 PARTITION OF attendance_records FOR VALUES FROM ('2026-01-01') TO ('2026-02-01');
CREATE TABLE attendance_2026_02 PARTITION OF attendance_records FOR VALUES FROM ('2026-02-01') TO ('2026-03-01');
CREATE TABLE attendance_2026_03 PARTITION OF attendance_records FOR VALUES FROM ('2026-03-01') TO ('2026-04-01');
CREATE TABLE attendance_2026_04 PARTITION OF attendance_records FOR VALUES FROM ('2026-04-01') TO ('2026-05-01');
CREATE TABLE attendance_2026_05 PARTITION OF attendance_records FOR VALUES FROM ('2026-05-01') TO ('2026-06-01');
CREATE TABLE attendance_2026_06 PARTITION OF attendance_records FOR VALUES FROM ('2026-06-01') TO ('2026-07-01');
CREATE TABLE attendance_2026_07 PARTITION OF attendance_records FOR VALUES FROM ('2026-07-01') TO ('2026-08-01');
CREATE TABLE attendance_2026_08 PARTITION OF attendance_records FOR VALUES FROM ('2026-08-01') TO ('2026-09-01');
CREATE TABLE attendance_2026_09 PARTITION OF attendance_records FOR VALUES FROM ('2026-09-01') TO ('2026-10-01');
CREATE TABLE attendance_2026_10 PARTITION OF attendance_records FOR VALUES FROM ('2026-10-01') TO ('2026-11-01');
CREATE TABLE attendance_2026_11 PARTITION OF attendance_records FOR VALUES FROM ('2026-11-01') TO ('2026-12-01');
CREATE TABLE attendance_2026_12 PARTITION OF attendance_records FOR VALUES FROM ('2026-12-01') TO ('2027-01-01');

-- ============================================
-- BẢNG 9: leave_requests (Đơn nghỉ phép)
-- ============================================
CREATE TABLE leave_requests (
    id              SERIAL          PRIMARY KEY,
    employee_id     INT             NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
    leave_type      VARCHAR(20)     NOT NULL
                    CHECK (leave_type IN ('Annual', 'Sick', 'Personal', 'Maternity', 'Unpaid', 'Other')),
    start_date      DATE            NOT NULL,
    end_date        DATE            NOT NULL,
    total_days      DECIMAL(4,1)    NOT NULL CHECK (total_days > 0),
    reason          TEXT,
    status          VARCHAR(20)     NOT NULL DEFAULT 'Pending'
                    CHECK (status IN ('Pending', 'Approved', 'Rejected', 'Cancelled')),
    approved_by     INT             REFERENCES employees(id) ON DELETE SET NULL,
    approved_at     TIMESTAMPTZ,
    reject_reason   TEXT,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMPTZ,

    CONSTRAINT chk_leave_dates CHECK (end_date >= start_date),
    CONSTRAINT chk_leave_approval CHECK (
        (status IN ('Pending', 'Cancelled') AND approved_by IS NULL AND approved_at IS NULL)
        OR (status = 'Approved' AND approved_by IS NOT NULL AND approved_at IS NOT NULL)
        OR (status = 'Rejected' AND approved_by IS NOT NULL AND approved_at IS NOT NULL)
    )
);

COMMENT ON TABLE leave_requests IS 'Quản lý đơn xin nghỉ phép';
COMMENT ON COLUMN leave_requests.approved_by IS 'FK → employees: người duyệt (quản lý)';
COMMENT ON COLUMN leave_requests.total_days IS 'Số ngày nghỉ (0.5 = nửa ngày)';

-- ============================================
-- BẢNG 10: holidays (Ngày lễ)
-- ============================================
CREATE TABLE holidays (
    id              SERIAL          PRIMARY KEY,
    holiday_date    DATE            NOT NULL UNIQUE,
    name            VARCHAR(100)    NOT NULL,
    description     TEXT,
    is_recurring    BOOLEAN         NOT NULL DEFAULT FALSE,
    recurring_month SMALLINT        CHECK (recurring_month BETWEEN 1 AND 12),
    recurring_day   SMALLINT        CHECK (recurring_day BETWEEN 1 AND 31),
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_recurring_fields CHECK (
        (is_recurring = FALSE AND recurring_month IS NULL AND recurring_day IS NULL)
        OR (is_recurring = TRUE AND recurring_month IS NOT NULL AND recurring_day IS NOT NULL)
    )
);

COMMENT ON TABLE holidays IS 'Ngày lễ/nghỉ — recurring auto-generate hàng năm';
COMMENT ON COLUMN holidays.holiday_date IS 'Ngày cụ thể (đổi tên từ "date" tránh nhầm)';
COMMENT ON COLUMN holidays.is_recurring IS 'TRUE = lặp hàng năm (1/1, 30/4, 1/5, 2/9)';
COMMENT ON COLUMN holidays.recurring_month IS 'Tháng lặp (dùng khi is_recurring = TRUE)';
COMMENT ON COLUMN holidays.recurring_day IS 'Ngày lặp (dùng khi is_recurring = TRUE)';

-- ============================================
-- BẢNG 11: system_settings (Cài đặt hệ thống)
-- Đã đổi tên: key → setting_key, type → data_type
-- ============================================
CREATE TABLE system_settings (
    id              SERIAL          PRIMARY KEY,
    setting_key     VARCHAR(100)    NOT NULL UNIQUE,
    setting_value   TEXT,
    data_type       VARCHAR(20)     NOT NULL DEFAULT 'String'
                    CHECK (data_type IN ('String', 'Int', 'Decimal', 'Boolean', 'Json')),
    description     TEXT,
    is_editable     BOOLEAN         NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMPTZ
);

COMMENT ON TABLE system_settings IS 'Cấu hình hệ thống dạng key-value';
COMMENT ON COLUMN system_settings.setting_key IS 'Tên setting (đổi từ "key" tránh reserved word)';
COMMENT ON COLUMN system_settings.data_type IS 'Kiểu dữ liệu (đổi từ "type" tránh reserved word)';

-- ============================================
-- BẢNG 12: audit_logs (Nhật ký thao tác)
-- ============================================
CREATE TABLE audit_logs (
    id              BIGSERIAL       PRIMARY KEY,
    user_id         INT             REFERENCES users(id) ON DELETE SET NULL,
    action          VARCHAR(50)     NOT NULL,
    table_name      VARCHAR(50),
    record_id       TEXT,
    old_data        JSONB,
    new_data        JSONB,
    ip_address      INET,
    user_agent      TEXT,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE audit_logs IS 'Audit trail cho mọi thao tác quan trọng';
COMMENT ON COLUMN audit_logs.ip_address IS 'INET type: tự validate IPv4/IPv6';
COMMENT ON COLUMN audit_logs.record_id IS 'TEXT: linh hoạt cho mọi kiểu PK';

-- ============================================
-- INDEXES
-- ============================================

-- == departments ==
CREATE INDEX idx_dept_parent ON departments(parent_id) WHERE parent_id IS NOT NULL;
CREATE INDEX idx_dept_active ON departments(is_active) WHERE is_active = TRUE;

-- == employees ==
CREATE INDEX idx_emp_department ON employees(department_id);
CREATE INDEX idx_emp_position ON employees(position_id);
CREATE INDEX idx_emp_active ON employees(is_active) WHERE is_active = TRUE;
CREATE INDEX idx_emp_name ON employees(full_name);
CREATE INDEX idx_emp_default_shift ON employees(default_shift_id) WHERE default_shift_id IS NOT NULL;

-- == face_data ==
-- Covering index cho query nhận diện: lấy tất cả encoding active
CREATE INDEX idx_face_active ON face_data(employee_id, is_active) WHERE is_active = TRUE;

-- == employee_shifts ==
CREATE INDEX idx_empshift_employee ON employee_shifts(employee_id, effective_date);
CREATE INDEX idx_empshift_shift ON employee_shifts(shift_id);

-- == attendance_records (tự inherit xuống partitions) ==
-- Composite index chính cho tra cứu chấm công (thay thế 2 index đơn lẻ thừa)
CREATE INDEX idx_att_emp_date ON attendance_records(employee_id, attendance_date);
CREATE INDEX idx_att_date ON attendance_records(attendance_date);
CREATE INDEX idx_att_status ON attendance_records(status) WHERE status != 'Present';

-- == leave_requests ==
CREATE INDEX idx_leave_employee ON leave_requests(employee_id);
CREATE INDEX idx_leave_status ON leave_requests(status) WHERE status = 'Pending';
CREATE INDEX idx_leave_dates ON leave_requests(start_date, end_date);

-- == holidays ==
CREATE INDEX idx_holiday_date ON holidays(holiday_date);
CREATE INDEX idx_holiday_recurring ON holidays(recurring_month, recurring_day) WHERE is_recurring = TRUE;

-- == audit_logs ==
CREATE INDEX idx_audit_user ON audit_logs(user_id) WHERE user_id IS NOT NULL;
CREATE INDEX idx_audit_created ON audit_logs(created_at);
CREATE INDEX idx_audit_table_action ON audit_logs(table_name, action);

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
            SELECT 1 FROM face_data
            WHERE employee_id = OLD.employee_id AND is_active = TRUE
            AND id != OLD.id
        ) WHERE id = OLD.employee_id;
        RETURN OLD;
    ELSIF TG_OP = 'UPDATE' AND OLD.is_active != NEW.is_active THEN
        UPDATE employees SET is_face_registered = EXISTS(
            SELECT 1 FROM face_data
            WHERE employee_id = NEW.employee_id AND is_active = TRUE
        ) WHERE id = NEW.employee_id;
        RETURN NEW;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_face_status
AFTER INSERT OR UPDATE OF is_active OR DELETE ON face_data
FOR EACH ROW EXECUTE FUNCTION fn_update_face_status();

-- ============================================
-- FUNCTION: Tự động tạo partition cho tháng tiếp theo
-- (Chạy scheduled hàng tháng hoặc bằng pg_cron)
-- ============================================
CREATE OR REPLACE FUNCTION fn_create_attendance_partition()
RETURNS void AS $$
DECLARE
    next_month DATE;
    partition_name TEXT;
    start_date DATE;
    end_date DATE;
BEGIN
    next_month := DATE_TRUNC('month', CURRENT_DATE + INTERVAL '1 month');
    partition_name := 'attendance_' || TO_CHAR(next_month, 'YYYY_MM');
    start_date := next_month;
    end_date := next_month + INTERVAL '1 month';

    -- Kiểm tra partition đã tồn tại chưa
    IF NOT EXISTS (
        SELECT 1 FROM pg_class WHERE relname = partition_name
    ) THEN
        EXECUTE format(
            'CREATE TABLE %I PARTITION OF attendance_records FOR VALUES FROM (%L) TO (%L)',
            partition_name, start_date, end_date
        );
        RAISE NOTICE 'Created partition: %', partition_name;
    END IF;
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION fn_create_attendance_partition IS 'Gọi hàng tháng: SELECT fn_create_attendance_partition();';

-- ============================================
-- VIEWS
-- ============================================

-- View: Chấm công hôm nay (với đầy đủ thông tin ca)
CREATE VIEW v_today_attendance AS
SELECT
    e.id              AS employee_id,
    e.code            AS employee_code,
    e.full_name,
    d.name            AS department_name,
    p.name            AS position_name,
    ws.name           AS shift_name,
    ws.start_time     AS shift_start,
    ws.end_time       AS shift_end,
    a.check_in,
    a.check_out,
    a.status,
    a.late_minutes,
    a.early_minutes,
    a.working_hours,
    a.overtime_hours,
    a.check_in_confidence,
    a.check_out_confidence
FROM employees e
LEFT JOIN attendance_records a ON e.id = a.employee_id
    AND a.attendance_date = CURRENT_DATE
LEFT JOIN departments d ON e.department_id = d.id
LEFT JOIN positions p ON e.position_id = p.id
LEFT JOIN work_shifts ws ON COALESCE(a.shift_id, e.default_shift_id) = ws.id
WHERE e.is_active = TRUE;

COMMENT ON VIEW v_today_attendance IS 'Dashboard: chấm công hôm nay của tất cả nhân viên active';

-- View: Tổng hợp chấm công tháng
CREATE VIEW v_monthly_summary AS
SELECT
    e.id              AS employee_id,
    e.code            AS employee_code,
    e.full_name,
    d.name            AS department_name,
    DATE_TRUNC('month', a.attendance_date) AS month,
    COUNT(*)                                                AS total_records,
    COUNT(*) FILTER (WHERE a.status = 'Present')            AS present_days,
    COUNT(*) FILTER (WHERE a.status = 'Late')               AS late_days,
    COUNT(*) FILTER (WHERE a.status = 'EarlyLeave')         AS early_leave_days,
    COUNT(*) FILTER (WHERE a.status = 'Absent')             AS absent_days,
    COUNT(*) FILTER (WHERE a.status = 'Leave')              AS leave_days,
    COUNT(*) FILTER (WHERE a.status = 'Holiday')            AS holiday_days,
    SUM(COALESCE(a.late_minutes, 0))                        AS total_late_minutes,
    SUM(COALESCE(a.early_minutes, 0))                       AS total_early_minutes,
    SUM(COALESCE(a.working_hours, 0))                       AS total_working_hours,
    SUM(COALESCE(a.overtime_hours, 0))                      AS total_overtime_hours
FROM employees e
INNER JOIN attendance_records a ON e.id = a.employee_id
LEFT JOIN departments d ON e.department_id = d.id
WHERE e.is_active = TRUE
GROUP BY e.id, e.code, e.full_name, d.name, DATE_TRUNC('month', a.attendance_date);

COMMENT ON VIEW v_monthly_summary IS 'Báo cáo tổng hợp chấm công theo tháng, theo nhân viên';

-- View: Nhân viên chưa đăng ký Face ID
CREATE VIEW v_employees_no_face AS
SELECT
    e.id,
    e.code,
    e.full_name,
    d.name AS department_name,
    p.name AS position_name,
    e.hire_date
FROM employees e
LEFT JOIN departments d ON e.department_id = d.id
LEFT JOIN positions p ON e.position_id = p.id
WHERE e.is_active = TRUE AND e.is_face_registered = FALSE;

COMMENT ON VIEW v_employees_no_face IS 'Danh sách nhân viên chưa đăng ký nhận diện khuôn mặt';

-- View: Đơn nghỉ phép đang chờ duyệt
CREATE VIEW v_pending_leaves AS
SELECT
    lr.id             AS request_id,
    e.code            AS employee_code,
    e.full_name,
    d.name            AS department_name,
    lr.leave_type,
    lr.start_date,
    lr.end_date,
    lr.total_days,
    lr.reason,
    lr.created_at     AS requested_at
FROM leave_requests lr
INNER JOIN employees e ON lr.employee_id = e.id
LEFT JOIN departments d ON e.department_id = d.id
WHERE lr.status = 'Pending'
ORDER BY lr.created_at;

COMMENT ON VIEW v_pending_leaves IS 'Danh sách đơn nghỉ phép đang chờ phê duyệt';

-- ============================================
-- DỮ LIỆU MẪU
-- ============================================

-- Phòng ban
INSERT INTO departments (code, name, description) VALUES
('HR',    'Phòng Nhân sự',    'Quản lý nhân sự, tuyển dụng'),
('IT',    'Phòng Công nghệ',  'Phát triển và vận hành hệ thống'),
('SALES', 'Phòng Kinh doanh', 'Bán hàng và CSKH'),
('ACCT',  'Phòng Kế toán',    'Tài chính, kế toán'),
('ADMIN', 'Phòng Hành chính', 'Hành chính văn phòng');

-- Chức vụ
INSERT INTO positions (code, name, base_salary) VALUES
('DIR',  'Giám đốc',         50000000.00),
('MGR',  'Trưởng phòng',     25000000.00),
('LEAD', 'Trưởng nhóm',      18000000.00),
('SR',   'Nhân viên cao cấp', 15000000.00),
('JR',   'Nhân viên',         10000000.00),
('INT',  'Thực tập sinh',      5000000.00);

-- Ca làm việc
INSERT INTO work_shifts (code, name, start_time, end_time, break_minutes, is_overnight) VALUES
('MAIN',      'Ca hành chính', '08:00', '17:00', 60, FALSE),
('MORNING',   'Ca sáng',       '06:00', '14:00', 30, FALSE),
('AFTERNOON', 'Ca chiều',      '14:00', '22:00', 30, FALSE),
('NIGHT',     'Ca đêm',        '22:00', '06:00', 30, TRUE);

-- Ngày lễ 2026
INSERT INTO holidays (holiday_date, name, is_recurring, recurring_month, recurring_day) VALUES
('2026-01-01', 'Tết Dương lịch',       TRUE,  1,  1),
('2026-02-16', 'Nghỉ Tết Nguyên đán',  FALSE, NULL, NULL),
('2026-02-17', 'Nghỉ Tết Nguyên đán',  FALSE, NULL, NULL),
('2026-02-18', 'Tết Nguyên đán',        FALSE, NULL, NULL),
('2026-02-19', 'Nghỉ Tết Nguyên đán',  FALSE, NULL, NULL),
('2026-02-20', 'Nghỉ Tết Nguyên đán',  FALSE, NULL, NULL),
('2026-04-06', 'Giỗ Tổ Hùng Vương',    FALSE, NULL, NULL),
('2026-04-30', 'Giải phóng miền Nam',  TRUE,  4, 30),
('2026-05-01', 'Quốc tế Lao động',     TRUE,  5,  1),
('2026-09-02', 'Quốc khánh',            TRUE,  9,  2);

-- Cài đặt hệ thống
INSERT INTO system_settings (setting_key, setting_value, data_type, description) VALUES
('company_name',   'Công ty ABC',  'String',  'Tên công ty'),
('work_start_time', '08:00',       'String',  'Giờ vào làm'),
('work_end_time',   '17:00',       'String',  'Giờ tan làm'),
('late_threshold',  '15',          'Int',     'Ngưỡng đi muộn (phút)'),
('early_threshold', '15',          'Int',     'Ngưỡng về sớm (phút)'),
('face_threshold',  '0.6',         'Decimal', 'Ngưỡng nhận diện (0-1)'),
('max_face_images', '5',           'Int',     'Số ảnh Face ID tối đa'),
('camera_index',    '0',           'Int',     'Camera index'),
('auto_checkout',   '23:59',       'String',  'Giờ tự động checkout'),
('require_checkout', 'true',       'Boolean', 'Bắt buộc checkout');

-- User Admin (password cần hash thực tế)
INSERT INTO users (username, password_hash, role) VALUES
('admin', '$2a$10$HASH_PASSWORD_HERE', 'Admin');

-- Nhân viên mẫu
INSERT INTO employees (code, full_name, gender, phone, email, department_id, position_id, default_shift_id) VALUES
('NV001', 'Nguyễn Văn An',   'M', '0901234567', 'an.nv@company.com',   2, 2, 1),
('NV002', 'Trần Thị Bình',   'F', '0901234568', 'binh.tt@company.com', 1, 3, 1),
('NV003', 'Lê Văn Cường',    'M', '0901234569', 'cuong.lv@company.com', 2, 5, 1),
('NV004', 'Phạm Thị Dung',   'F', '0901234570', 'dung.pt@company.com', 3, 5, 1),
('NV005', 'Hoàng Văn Em',    'M', '0901234571', 'em.hv@company.com',   4, 4, 1);

-- ============================================
-- GHI CHÚ VẬN HÀNH
-- ============================================
-- 1. Chạy hàng tháng: SELECT fn_create_attendance_partition();
--    Hoặc cài pg_cron:
--    SELECT cron.schedule('create_next_partition', '0 0 25 * *',
--           'SELECT fn_create_attendance_partition()');
--
-- 2. Archival: Detach partition cũ khi không còn cần truy vấn thường xuyên:
--    ALTER TABLE attendance_records DETACH PARTITION attendance_2024_01;
--
-- 3. Monitoring indexes: Kiểm tra unused indexes định kỳ:
--    SELECT * FROM pg_stat_user_indexes WHERE idx_scan = 0;
