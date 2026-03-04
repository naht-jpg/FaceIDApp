-- ============================================
-- MIGRATION SCRIPT: v1 → v2
-- Hệ thống Chấm công bằng Nhận diện Khuôn mặt
-- ============================================
-- ⚠️  CHẠY TRÊN DATABASE CÓ DỮ LIỆU PRODUCTION
-- ⚠️  BACKUP TRƯỚC KHI CHẠY: pg_dump -Fc face_id_attendance > backup_v1.dump
-- ============================================

BEGIN;

SET timezone = 'Asia/Ho_Chi_Minh';

-- ============================================
-- BƯỚC 1: TIMESTAMP → TIMESTAMPTZ
-- ============================================
ALTER TABLE departments
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE positions
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE employees
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE users
    ALTER COLUMN last_login TYPE TIMESTAMPTZ,
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE face_data
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE work_shifts
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE attendance_records
    ALTER COLUMN check_in TYPE TIMESTAMPTZ,
    ALTER COLUMN check_out TYPE TIMESTAMPTZ,
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE leave_requests
    ALTER COLUMN approved_at TYPE TIMESTAMPTZ,
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE holidays
    ALTER COLUMN created_at TYPE TIMESTAMPTZ;

ALTER TABLE system_settings
    ALTER COLUMN created_at TYPE TIMESTAMPTZ,
    ALTER COLUMN updated_at TYPE TIMESTAMPTZ;

ALTER TABLE audit_logs
    ALTER COLUMN created_at TYPE TIMESTAMPTZ;

-- ============================================
-- BƯỚC 2: ĐỔI TÊN CỘT RESERVED KEYWORDS
-- ============================================
ALTER TABLE system_settings RENAME COLUMN key TO setting_key;
ALTER TABLE system_settings RENAME COLUMN value TO setting_value;
ALTER TABLE system_settings RENAME COLUMN type TO data_type;

-- ============================================
-- BƯỚC 3: ĐỔI TÊN CỘT TRÁNH NHẦM
-- ============================================
ALTER TABLE attendance_records RENAME COLUMN date TO attendance_date;
ALTER TABLE holidays RENAME COLUMN date TO holiday_date;

-- ============================================
-- BƯỚC 4: GENDER ENCODING
-- ============================================
-- 4a. Đổi kiểu dữ liệu gender
ALTER TABLE employees DROP CONSTRAINT IF EXISTS employees_gender_check;
UPDATE employees SET gender = CASE
    WHEN gender = 'Nam'  THEN 'M'
    WHEN gender = 'Nữ'   THEN 'F'
    WHEN gender = 'Khác' THEN 'O'
    ELSE gender
END WHERE gender IS NOT NULL;
ALTER TABLE employees ALTER COLUMN gender TYPE CHAR(1);
ALTER TABLE employees ADD CONSTRAINT employees_gender_check CHECK (gender IN ('M', 'F', 'O'));

-- ============================================
-- BƯỚC 5: THÊM CỘT MỚI
-- ============================================
-- departments: parent_id
ALTER TABLE departments ADD COLUMN IF NOT EXISTS parent_id INT REFERENCES departments(id) ON DELETE SET NULL;

-- employees: default_shift_id, termination_date
ALTER TABLE employees ADD COLUMN IF NOT EXISTS default_shift_id INT REFERENCES work_shifts(id) ON DELETE SET NULL;
ALTER TABLE employees ADD COLUMN IF NOT EXISTS termination_date DATE;
ALTER TABLE employees ADD CONSTRAINT chk_employee_dates CHECK (
    termination_date IS NULL OR termination_date >= hire_date
);

-- users: failed_login_count, locked_until
ALTER TABLE users ADD COLUMN IF NOT EXISTS failed_login_count SMALLINT NOT NULL DEFAULT 0;
ALTER TABLE users ADD COLUMN IF NOT EXISTS locked_until TIMESTAMPTZ;

-- work_shifts: is_overnight
ALTER TABLE work_shifts ADD COLUMN IF NOT EXISTS is_overnight BOOLEAN NOT NULL DEFAULT FALSE;
UPDATE work_shifts SET is_overnight = TRUE WHERE code = 'NIGHT';

-- holidays: recurring_month, recurring_day
ALTER TABLE holidays ADD COLUMN IF NOT EXISTS recurring_month SMALLINT CHECK (recurring_month BETWEEN 1 AND 12);
ALTER TABLE holidays ADD COLUMN IF NOT EXISTS recurring_day SMALLINT CHECK (recurring_day BETWEEN 1 AND 31);
UPDATE holidays SET
    recurring_month = EXTRACT(MONTH FROM holiday_date)::SMALLINT,
    recurring_day = EXTRACT(DAY FROM holiday_date)::SMALLINT
WHERE is_recurring = TRUE;

-- audit_logs: user_agent, ip_address type change
ALTER TABLE audit_logs ADD COLUMN IF NOT EXISTS user_agent TEXT;
ALTER TABLE audit_logs ALTER COLUMN record_id TYPE TEXT;
ALTER TABLE audit_logs ALTER COLUMN ip_address TYPE INET USING ip_address::INET;

-- ============================================
-- BƯỚC 6: THÊM CHECK CONSTRAINTS
-- ============================================
-- attendance: check_out >= check_in
ALTER TABLE attendance_records ADD CONSTRAINT chk_checkout_after_checkin
    CHECK (check_out IS NULL OR check_in IS NULL OR check_out >= check_in);

-- leave_requests: end_date >= start_date
ALTER TABLE leave_requests ADD CONSTRAINT chk_leave_dates
    CHECK (end_date >= start_date);

-- face_data: unique (employee_id, image_index)
ALTER TABLE face_data ADD CONSTRAINT uq_face_employee_index
    UNIQUE (employee_id, image_index);

-- attendance_records: status update
ALTER TABLE attendance_records DROP CONSTRAINT IF EXISTS attendance_records_status_check;
ALTER TABLE attendance_records ADD CONSTRAINT attendance_records_status_check
    CHECK (status IN ('Present', 'Late', 'EarlyLeave', 'Absent', 'Leave', 'Holiday'));
UPDATE attendance_records SET status = 'EarlyLeave' WHERE status = 'Early';

-- ============================================
-- BƯỚC 7: CẬP NHẬT FK BEHAVIOR
-- ============================================
-- employees → departments
ALTER TABLE employees DROP CONSTRAINT IF EXISTS employees_department_id_fkey;
ALTER TABLE employees ADD CONSTRAINT employees_department_id_fkey
    FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE SET NULL;

-- employees → positions
ALTER TABLE employees DROP CONSTRAINT IF EXISTS employees_position_id_fkey;
ALTER TABLE employees ADD CONSTRAINT employees_position_id_fkey
    FOREIGN KEY (position_id) REFERENCES positions(id) ON DELETE SET NULL;

-- users → employees
ALTER TABLE users DROP CONSTRAINT IF EXISTS users_employee_id_fkey;
ALTER TABLE users ADD CONSTRAINT users_employee_id_fkey
    FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE SET NULL;
ALTER TABLE users ADD CONSTRAINT uq_users_employee UNIQUE (employee_id);

-- departments.manager_id
ALTER TABLE departments DROP CONSTRAINT IF EXISTS fk_dept_manager;
ALTER TABLE departments ADD CONSTRAINT fk_dept_manager
    FOREIGN KEY (manager_id) REFERENCES employees(id) ON DELETE SET NULL;

-- leave_requests.approved_by → employees (was users)
ALTER TABLE leave_requests DROP CONSTRAINT IF EXISTS leave_requests_approved_by_fkey;
ALTER TABLE leave_requests ADD CONSTRAINT leave_requests_approved_by_fkey
    FOREIGN KEY (approved_by) REFERENCES employees(id) ON DELETE SET NULL;

-- attendance_records → employees (RESTRICT delete for data integrity)
ALTER TABLE attendance_records DROP CONSTRAINT IF EXISTS attendance_records_employee_id_fkey;
ALTER TABLE attendance_records ADD CONSTRAINT attendance_records_employee_id_fkey
    FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE RESTRICT;

-- audit_logs → users
ALTER TABLE audit_logs DROP CONSTRAINT IF EXISTS audit_logs_user_id_fkey;
ALTER TABLE audit_logs ADD CONSTRAINT audit_logs_user_id_fkey
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL;

-- ============================================
-- BƯỚC 8: TẠO BẢNG MỚI employee_shifts
-- ============================================
CREATE TABLE IF NOT EXISTS employee_shifts (
    id              SERIAL          PRIMARY KEY,
    employee_id     INT             NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
    shift_id        INT             NOT NULL REFERENCES work_shifts(id) ON DELETE CASCADE,
    effective_date  DATE            NOT NULL,
    end_date        DATE,
    created_by      INT             REFERENCES employees(id) ON DELETE SET NULL,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_shift_dates CHECK (end_date IS NULL OR end_date >= effective_date),
    CONSTRAINT uq_employee_shift_date UNIQUE (employee_id, effective_date)
);

-- ============================================
-- BƯỚC 9: CẬP NHẬT INDEXES
-- ============================================
-- Drop indexes thừa
DROP INDEX IF EXISTS idx_attendance_employee;   -- bao phủ bởi idx_attendance_emp_date

-- Thêm indexes mới
CREATE INDEX IF NOT EXISTS idx_dept_parent ON departments(parent_id) WHERE parent_id IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_dept_active ON departments(is_active) WHERE is_active = TRUE;
CREATE INDEX IF NOT EXISTS idx_emp_name ON employees(full_name);
CREATE INDEX IF NOT EXISTS idx_emp_default_shift ON employees(default_shift_id) WHERE default_shift_id IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_face_active ON face_data(employee_id, is_active) WHERE is_active = TRUE;
CREATE INDEX IF NOT EXISTS idx_empshift_employee ON employee_shifts(employee_id, effective_date);
CREATE INDEX IF NOT EXISTS idx_empshift_shift ON employee_shifts(shift_id);
CREATE INDEX IF NOT EXISTS idx_leave_dates ON leave_requests(start_date, end_date);
CREATE INDEX IF NOT EXISTS idx_holiday_recurring ON holidays(recurring_month, recurring_day) WHERE is_recurring = TRUE;
CREATE INDEX IF NOT EXISTS idx_audit_table_action ON audit_logs(table_name, action);

-- Đổi idx_attendance_date sang tên mới (nếu cần)
-- idx_attendance_emp_date vẫn giữ nguyên

-- ============================================
-- BƯỚC 10: CẬP NHẬT TRIGGER fn_update_face_status
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

-- Tạo lại trigger với UPDATE OF is_active
DROP TRIGGER IF EXISTS trg_face_status ON face_data;
CREATE TRIGGER trg_face_status
AFTER INSERT OR UPDATE OF is_active OR DELETE ON face_data
FOR EACH ROW EXECUTE FUNCTION fn_update_face_status();

-- ============================================
-- BƯỚC 11: TẠO FUNCTION AUTO-PARTITION
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

COMMIT;

-- ============================================
-- ⚠️  LƯU Ý VỀ PARTITIONING
-- ============================================
-- Migration này KHÔNG chuyển attendance_records sang partitioned table.
-- Để partition hóa attendance_records, cần:
--   1. Tạo bảng partitioned mới (attendance_records_new)
--   2. Copy dữ liệu: INSERT INTO attendance_records_new SELECT * FROM attendance_records;
--   3. Swap: ALTER TABLE attendance_records RENAME TO attendance_records_old;
--            ALTER TABLE attendance_records_new RENAME TO attendance_records;
--   4. Recreate indexes, FK, triggers trên bảng mới
--   5. Verify → DROP TABLE attendance_records_old;
-- ĐỀ XUẤT: Thực hiện partitioning khi dữ liệu > 100,000 rows
-- hoặc sử dụng create_database_v2.sql cho hệ thống mới.
