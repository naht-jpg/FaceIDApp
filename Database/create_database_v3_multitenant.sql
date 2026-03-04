-- ================================================================
-- FILE: create_database_v3_multitenant.sql
-- MÔ TẢ: Script tạo hoàn chỉnh CSDL Chấm Công Đa Tổ Chức
--         bằng Nhận Diện Khuôn Mặt (Multi-Tenant, Multi-Model AI)
-- DATABASE: PostgreSQL 16+
-- VERSION: 3.0
-- NGÀY: 2026-03-04
-- ================================================================
-- HƯỚNG DẪN SỬ DỤNG:
--   1. Mở pgAdmin hoặc psql, kết nối vào server PostgreSQL
--   2. Chạy phần "TẠO DATABASE" bên dưới (bước riêng)
--   3. Kết nối vào database vừa tạo
--   4. Chạy toàn bộ phần còn lại của file này
-- ================================================================

-- ================================================================
-- PHẦN 0: TẠO DATABASE
-- (Chạy riêng khi kết nối vào postgres mặc định)
-- ================================================================
-- CREATE DATABASE face_id_attendance
--     WITH OWNER = postgres
--          ENCODING = 'UTF8'
--          LC_COLLATE = 'vi_VN.UTF-8'
--          LC_CTYPE = 'vi_VN.UTF-8'
--          TEMPLATE = template0;
-- \c face_id_attendance

-- ================================================================
-- PHẦN 1: THIẾT LẬP MÔI TRƯỜNG
-- ================================================================
SET client_encoding = 'UTF8';
SET timezone = 'Asia/Ho_Chi_Minh';
SET statement_timeout = 0;

-- ================================================================
-- PHẦN 2: XÓA BẢNG CŨ (nếu có, theo thứ tự phụ thuộc FK)
-- ================================================================
DROP TABLE IF EXISTS audit_logs CASCADE;
DROP TABLE IF EXISTS absence_requests CASCADE;
DROP TABLE IF EXISTS attendance_logs CASCADE;
DROP TABLE IF EXISTS member_schedules CASCADE;
DROP TABLE IF EXISTS face_data CASCADE;
DROP TABLE IF EXISTS face_models CASCADE;
DROP TABLE IF EXISTS devices CASCADE;
DROP TABLE IF EXISTS member_roles CASCADE;
DROP TABLE IF EXISTS member_units CASCADE;
DROP TABLE IF EXISTS accounts CASCADE;
DROP TABLE IF EXISTS members CASCADE;
DROP TABLE IF EXISTS schedules CASCADE;
DROP TABLE IF EXISTS roles CASCADE;
DROP TABLE IF EXISTS units CASCADE;
DROP TABLE IF EXISTS holidays CASCADE;
DROP TABLE IF EXISTS org_settings CASCADE;
DROP TABLE IF EXISTS organizations CASCADE;

-- ================================================================
-- PHẦN 3: TẠO CÁC BẢNG
-- Tổng cộng 17 bảng, chia 5 nhóm:
--   [A] Tổ chức & Cấu trúc  (organizations, units, roles, schedules)
--   [B] Thành viên & Tài khoản (members, member_units, member_roles, accounts)
--   [C] Nhận diện khuôn mặt   (face_models, face_data, devices)
--   [D] Chấm công & Nghỉ phép (member_schedules, attendance_logs,
--                               absence_requests, holidays)
--   [E] Hệ thống              (org_settings, audit_logs)
-- ================================================================

-- ────────────────────────────────────────────────────────────────
-- [A] TỔ CHỨC & CẤU TRÚC
-- ────────────────────────────────────────────────────────────────

-- A1. organizations — Tenant gốc (công ty / trường / nhà máy)
CREATE TABLE organizations (
    org_id      SERIAL       PRIMARY KEY,
    code        VARCHAR(30)  NOT NULL UNIQUE,
    name        VARCHAR(150) NOT NULL,
    org_type    VARCHAR(30)  NOT NULL
                CHECK (org_type IN ('company','school','factory','training_center','other')),
    timezone    VARCHAR(50)  NOT NULL DEFAULT 'Asia/Ho_Chi_Minh',
    locale      VARCHAR(10)  NOT NULL DEFAULT 'vi_VN',
    logo        TEXT,
    config      JSONB        NOT NULL DEFAULT '{}',
    is_active   BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMPTZ
);
COMMENT ON TABLE  organizations IS 'Tenant gốc — mỗi tổ chức là 1 tenant riêng biệt';
COMMENT ON COLUMN organizations.org_type IS 'company / school / factory / training_center / other';
COMMENT ON COLUMN organizations.config  IS '{"member_label":"Học sinh", "unit_label":"Lớp", "attendance_label":"Điểm danh"}';

-- A2. units — Đơn vị quản lý đa cấp (phòng ban / lớp / dây chuyền)
CREATE TABLE units (
    unit_id    SERIAL       PRIMARY KEY,
    org_id     INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    code       VARCHAR(30)  NOT NULL,
    name       VARCHAR(150) NOT NULL,
    unit_type  VARCHAR(30),
    parent_id  INT          REFERENCES units(unit_id) ON DELETE SET NULL,
    leader_id  INT,         -- FK → members (thêm sau)
    level      SMALLINT     NOT NULL DEFAULT 0,
    sort_order SMALLINT     NOT NULL DEFAULT 0,
    is_active  BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ,
    CONSTRAINT uq_unit_code_org UNIQUE (org_id, code)
);
COMMENT ON TABLE  units IS 'Đơn vị đa cấp: Phòng ban / Lớp / Dây chuyền / Team';
COMMENT ON COLUMN units.unit_type IS 'department / class / grade / production_line / team';
COMMENT ON COLUMN units.level    IS 'Độ sâu trong cây (0 = gốc)';

-- A3. roles — Vai trò / Chức vụ linh hoạt
CREATE TABLE roles (
    role_id     SERIAL       PRIMARY KEY,
    org_id      INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    code        VARCHAR(30)  NOT NULL,
    name        VARCHAR(100) NOT NULL,
    role_type   VARCHAR(30)  NOT NULL DEFAULT 'position',
    permissions JSONB        NOT NULL DEFAULT '{}',
    metadata    JSONB        NOT NULL DEFAULT '{}',
    sort_order  SMALLINT     NOT NULL DEFAULT 0,
    is_active   BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMPTZ,
    CONSTRAINT uq_role_code_org UNIQUE (org_id, code)
);
COMMENT ON TABLE  roles IS 'Vai trò linh hoạt: chức vụ / cấp bậc / loại thành viên';
COMMENT ON COLUMN roles.role_type IS 'position / grade / worker_type / custom';
COMMENT ON COLUMN roles.metadata  IS '{"base_salary":25000000} hoặc {"grade_level":12}';

-- A4. schedules — Ca / Lịch linh hoạt
CREATE TABLE schedules (
    schedule_id  SERIAL       PRIMARY KEY,
    org_id       INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    code         VARCHAR(30)  NOT NULL,
    name         VARCHAR(100) NOT NULL,
    schedule_type VARCHAR(30) NOT NULL DEFAULT 'fixed_shift'
                 CHECK (schedule_type IN ('fixed_shift','class_period','rotating_shift','flexible')),
    start_time   TIME         NOT NULL,
    end_time     TIME         NOT NULL,
    is_overnight BOOLEAN      NOT NULL DEFAULT FALSE,
    break_minutes           SMALLINT NOT NULL DEFAULT 0  CHECK (break_minutes >= 0),
    late_threshold_minutes  SMALLINT NOT NULL DEFAULT 15 CHECK (late_threshold_minutes >= 0),
    early_threshold_minutes SMALLINT NOT NULL DEFAULT 15 CHECK (early_threshold_minutes >= 0),
    rules        JSONB        NOT NULL DEFAULT '{}',
    is_active    BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at   TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at   TIMESTAMPTZ,
    CONSTRAINT uq_schedule_code_org UNIQUE (org_id, code)
);
COMMENT ON TABLE  schedules IS 'Ca/lịch: ca cố định, tiết học, ca xoay, linh hoạt';
COMMENT ON COLUMN schedules.schedule_type IS 'fixed_shift / class_period / rotating_shift / flexible';
COMMENT ON COLUMN schedules.rules IS '{"days_of_week":[1,2,3,4,5]} hoặc {"period":3,"duration_min":45}';

-- ────────────────────────────────────────────────────────────────
-- [B] THÀNH VIÊN & TÀI KHOẢN
-- ────────────────────────────────────────────────────────────────

-- B1. members — Thành viên (nhân viên / học sinh / công nhân / học viên)
CREATE TABLE members (
    member_id          SERIAL       PRIMARY KEY,
    org_id             INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    code               VARCHAR(30)  NOT NULL,
    full_name          VARCHAR(100) NOT NULL,
    gender             CHAR(1)      CHECK (gender IN ('M','F','O')),
    date_of_birth      DATE,
    phone              VARCHAR(20),
    email              VARCHAR(100),
    address            TEXT,
    identity_number    VARCHAR(30),
    avatar             TEXT,
    default_schedule_id INT         REFERENCES schedules(schedule_id) ON DELETE SET NULL,
    join_date          DATE         NOT NULL DEFAULT CURRENT_DATE,
    leave_date         DATE,
    is_active          BOOLEAN      NOT NULL DEFAULT TRUE,
    is_face_registered BOOLEAN      NOT NULL DEFAULT FALSE,
    metadata           JSONB        NOT NULL DEFAULT '{}',
    created_at         TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at         TIMESTAMPTZ,
    CONSTRAINT uq_member_code_org  UNIQUE (org_id, code),
    CONSTRAINT uq_member_email_org UNIQUE (org_id, email),
    CONSTRAINT chk_member_dates    CHECK (leave_date IS NULL OR leave_date >= join_date)
);
COMMENT ON TABLE  members IS 'Thành viên trung lập: NV / HS / CN / HV';
COMMENT ON COLUMN members.gender   IS 'M=Male, F=Female, O=Other';
COMMENT ON COLUMN members.metadata IS '{"student_id":"HS2026"} hoặc {"badge":"B-105"}';

-- Thêm FK: units.leader_id → members
ALTER TABLE units ADD CONSTRAINT fk_unit_leader
    FOREIGN KEY (leader_id) REFERENCES members(member_id) ON DELETE SET NULL;

-- B2. member_units — Phân bổ thành viên ↔ đơn vị (N-N)
CREATE TABLE member_units (
    id         SERIAL      PRIMARY KEY,
    member_id  INT         NOT NULL REFERENCES members(member_id) ON DELETE CASCADE,
    unit_id    INT         NOT NULL REFERENCES units(unit_id) ON DELETE CASCADE,
    is_primary BOOLEAN     NOT NULL DEFAULT TRUE,
    start_date DATE        NOT NULL DEFAULT CURRENT_DATE,
    end_date   DATE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_member_unit UNIQUE (member_id, unit_id),
    CONSTRAINT chk_mu_dates   CHECK (end_date IS NULL OR end_date >= start_date)
);

-- B3. member_roles — Gán vai trò cho thành viên (N-N)
CREATE TABLE member_roles (
    id         SERIAL      PRIMARY KEY,
    member_id  INT         NOT NULL REFERENCES members(member_id) ON DELETE CASCADE,
    role_id    INT         NOT NULL REFERENCES roles(role_id) ON DELETE CASCADE,
    is_primary BOOLEAN     NOT NULL DEFAULT TRUE,
    start_date DATE        NOT NULL DEFAULT CURRENT_DATE,
    end_date   DATE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_member_role UNIQUE (member_id, role_id),
    CONSTRAINT chk_mr_dates   CHECK (end_date IS NULL OR end_date >= start_date)
);

-- B4. accounts — Tài khoản đăng nhập
CREATE TABLE accounts (
    account_id         SERIAL       PRIMARY KEY,
    org_id             INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    username           VARCHAR(50)  NOT NULL,
    password_hash      VARCHAR(255) NOT NULL,
    member_id          INT          UNIQUE REFERENCES members(member_id) ON DELETE SET NULL,
    account_role       VARCHAR(20)  NOT NULL DEFAULT 'User'
                       CHECK (account_role IN ('SuperAdmin','Admin','Manager','User')),
    is_active          BOOLEAN      NOT NULL DEFAULT TRUE,
    last_login         TIMESTAMPTZ,
    failed_login_count SMALLINT     NOT NULL DEFAULT 0,
    locked_until       TIMESTAMPTZ,
    created_at         TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at         TIMESTAMPTZ,
    CONSTRAINT uq_account_username_org UNIQUE (org_id, username)
);
COMMENT ON COLUMN accounts.account_role IS 'SuperAdmin: đa org | Admin/Manager/User: trong org';

-- ────────────────────────────────────────────────────────────────
-- [C] NHẬN DIỆN KHUÔN MẶT
-- ────────────────────────────────────────────────────────────────

-- C1. face_models — Định nghĩa model AI nhận diện
CREATE TABLE face_models (
    model_id             SERIAL       PRIMARY KEY,
    org_id               INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    code                 VARCHAR(30)  NOT NULL,
    name                 VARCHAR(100) NOT NULL,
    model_type           VARCHAR(30)  NOT NULL
                         CHECK (model_type IN ('dlib_resnet','facenet','arcface','insightface','custom')),
    embedding_dimensions SMALLINT     NOT NULL CHECK (embedding_dimensions > 0),
    distance_metric      VARCHAR(20)  NOT NULL DEFAULT 'euclidean'
                         CHECK (distance_metric IN ('euclidean','cosine')),
    match_threshold      REAL         NOT NULL CHECK (match_threshold > 0 AND match_threshold < 1),
    model_version        VARCHAR(30),
    model_path           TEXT,
    config               JSONB        NOT NULL DEFAULT '{}',
    is_default           BOOLEAN      NOT NULL DEFAULT FALSE,
    is_active            BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at           TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at           TIMESTAMPTZ,
    CONSTRAINT uq_face_model_code_org UNIQUE (org_id, code)
);
COMMENT ON TABLE  face_models IS 'Model AI nhận diện khuôn mặt — per org';
COMMENT ON COLUMN face_models.embedding_dimensions IS '128 (dlib) / 512 (FaceNet, ArcFace)';
COMMENT ON COLUMN face_models.distance_metric   IS 'euclidean hoặc cosine';
COMMENT ON COLUMN face_models.match_threshold   IS 'Ngưỡng: dlib~0.6, ArcFace~0.4';
COMMENT ON COLUMN face_models.config IS '{"input_size":[112,112],"backend":"onnx","preprocessing":"mtcnn"}';

-- Mỗi org chỉ có 1 model mặc định
CREATE UNIQUE INDEX uq_face_model_default ON face_models(org_id) WHERE is_default = TRUE;

-- C2. face_data — Dữ liệu encoding khuôn mặt
CREATE TABLE face_data (
    face_id     SERIAL      PRIMARY KEY,
    org_id      INT         NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    member_id   INT         NOT NULL REFERENCES members(member_id) ON DELETE CASCADE,
    model_id    INT         NOT NULL REFERENCES face_models(model_id) ON DELETE RESTRICT,
    encoding    BYTEA       NOT NULL,
    image_path  TEXT,
    image_index SMALLINT    NOT NULL DEFAULT 1 CHECK (image_index BETWEEN 1 AND 10),
    quality     REAL        CHECK (quality >= 0 AND quality <= 1),
    is_active   BOOLEAN     NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMPTZ,
    CONSTRAINT uq_face_member_model_idx UNIQUE (member_id, model_id, image_index)
);
COMMENT ON TABLE  face_data IS 'Face embeddings — mỗi encoding gắn với 1 model cụ thể';
COMMENT ON COLUMN face_data.model_id    IS 'Model nào tạo ra encoding này';
COMMENT ON COLUMN face_data.image_index IS 'Vị trí 1-10, mỗi member tối đa 10 ảnh / model';

-- C3. devices — Thiết bị chấm công (camera / tablet / kiosk)
CREATE TABLE devices (
    device_id   SERIAL       PRIMARY KEY,
    org_id      INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    code        VARCHAR(30)  NOT NULL,
    name        VARCHAR(100) NOT NULL,
    device_type VARCHAR(30)  NOT NULL DEFAULT 'camera'
                CHECK (device_type IN ('camera','tablet','kiosk','other')),
    location    VARCHAR(200),
    unit_id     INT          REFERENCES units(unit_id) ON DELETE SET NULL,
    model_id    INT          REFERENCES face_models(model_id) ON DELETE SET NULL,
    config      JSONB        NOT NULL DEFAULT '{}',
    is_active   BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMPTZ,
    CONSTRAINT uq_device_code_org UNIQUE (org_id, code)
);
COMMENT ON COLUMN devices.model_id IS 'Model AI cho device này (NULL = dùng org default)';

-- ────────────────────────────────────────────────────────────────
-- [D] CHẤM CÔNG & NGHỈ PHÉP
-- ────────────────────────────────────────────────────────────────

-- D1. member_schedules — Phân ca / lịch cho thành viên
CREATE TABLE member_schedules (
    id             SERIAL      PRIMARY KEY,
    member_id      INT         NOT NULL REFERENCES members(member_id) ON DELETE CASCADE,
    schedule_id    INT         NOT NULL REFERENCES schedules(schedule_id) ON DELETE CASCADE,
    effective_date DATE        NOT NULL,
    end_date       DATE,
    created_by     INT         REFERENCES members(member_id) ON DELETE SET NULL,
    created_at     TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_ms_dates           CHECK (end_date IS NULL OR end_date >= effective_date),
    CONSTRAINT uq_member_schedule_date UNIQUE (member_id, effective_date)
);

-- D2. attendance_logs — Bản ghi chấm công (PARTITIONED theo tháng)
CREATE TABLE attendance_logs (
    log_id               BIGSERIAL,
    org_id               INT         NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    member_id            INT         NOT NULL REFERENCES members(member_id) ON DELETE RESTRICT,
    attendance_date      DATE        NOT NULL DEFAULT CURRENT_DATE,
    schedule_id          INT         REFERENCES schedules(schedule_id) ON DELETE SET NULL,
    device_id            INT         REFERENCES devices(device_id) ON DELETE SET NULL,
    check_in             TIMESTAMPTZ,
    check_out            TIMESTAMPTZ,
    check_in_image       TEXT,
    check_out_image      TEXT,
    check_in_confidence  REAL,
    check_out_confidence REAL,
    check_in_model_id    INT         REFERENCES face_models(model_id) ON DELETE SET NULL,
    check_out_model_id   INT         REFERENCES face_models(model_id) ON DELETE SET NULL,
    status               VARCHAR(20) NOT NULL DEFAULT 'Present'
                         CHECK (status IN ('Present','Late','EarlyLeave','Absent','Leave','Holiday')),
    late_minutes         SMALLINT    NOT NULL DEFAULT 0 CHECK (late_minutes >= 0),
    early_minutes        SMALLINT    NOT NULL DEFAULT 0 CHECK (early_minutes >= 0),
    working_hours        DECIMAL(5,2) CHECK (working_hours >= 0),
    overtime_hours       DECIMAL(5,2) NOT NULL DEFAULT 0 CHECK (overtime_hours >= 0),
    note                 TEXT,
    created_at           TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at           TIMESTAMPTZ,

    PRIMARY KEY (log_id, attendance_date),
    CONSTRAINT uq_att_member_date       UNIQUE (member_id, attendance_date),
    CONSTRAINT chk_checkout_after_checkin CHECK (check_out IS NULL OR check_in IS NULL OR check_out >= check_in),
    CONSTRAINT chk_confidence            CHECK (
        (check_in_confidence  IS NULL OR (check_in_confidence  >= 0 AND check_in_confidence  <= 1)) AND
        (check_out_confidence IS NULL OR (check_out_confidence >= 0 AND check_out_confidence <= 1))
    )
) PARTITION BY RANGE (attendance_date);

COMMENT ON TABLE  attendance_logs IS 'Chấm công — partitioned theo tháng';
COMMENT ON COLUMN attendance_logs.check_in_model_id IS 'Model AI nào nhận diện lúc check-in';

-- Tạo partitions cho năm 2026
CREATE TABLE att_2026_01 PARTITION OF attendance_logs FOR VALUES FROM ('2026-01-01') TO ('2026-02-01');
CREATE TABLE att_2026_02 PARTITION OF attendance_logs FOR VALUES FROM ('2026-02-01') TO ('2026-03-01');
CREATE TABLE att_2026_03 PARTITION OF attendance_logs FOR VALUES FROM ('2026-03-01') TO ('2026-04-01');
CREATE TABLE att_2026_04 PARTITION OF attendance_logs FOR VALUES FROM ('2026-04-01') TO ('2026-05-01');
CREATE TABLE att_2026_05 PARTITION OF attendance_logs FOR VALUES FROM ('2026-05-01') TO ('2026-06-01');
CREATE TABLE att_2026_06 PARTITION OF attendance_logs FOR VALUES FROM ('2026-06-01') TO ('2026-07-01');
CREATE TABLE att_2026_07 PARTITION OF attendance_logs FOR VALUES FROM ('2026-07-01') TO ('2026-08-01');
CREATE TABLE att_2026_08 PARTITION OF attendance_logs FOR VALUES FROM ('2026-08-01') TO ('2026-09-01');
CREATE TABLE att_2026_09 PARTITION OF attendance_logs FOR VALUES FROM ('2026-09-01') TO ('2026-10-01');
CREATE TABLE att_2026_10 PARTITION OF attendance_logs FOR VALUES FROM ('2026-10-01') TO ('2026-11-01');
CREATE TABLE att_2026_11 PARTITION OF attendance_logs FOR VALUES FROM ('2026-11-01') TO ('2026-12-01');
CREATE TABLE att_2026_12 PARTITION OF attendance_logs FOR VALUES FROM ('2026-12-01') TO ('2027-01-01');

-- D3. absence_requests — Đơn xin nghỉ (phép / nghỉ học / lý do)
CREATE TABLE absence_requests (
    request_id    SERIAL       PRIMARY KEY,
    org_id        INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    member_id     INT          NOT NULL REFERENCES members(member_id) ON DELETE CASCADE,
    absence_type  VARCHAR(30)  NOT NULL,
    start_date    DATE         NOT NULL,
    end_date      DATE         NOT NULL,
    total_days    DECIMAL(4,1) NOT NULL CHECK (total_days > 0),
    reason        TEXT,
    status        VARCHAR(20)  NOT NULL DEFAULT 'Pending'
                  CHECK (status IN ('Pending','Approved','Rejected','Cancelled')),
    approved_by   INT          REFERENCES members(member_id) ON DELETE SET NULL,
    approved_at   TIMESTAMPTZ,
    reject_reason TEXT,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    TIMESTAMPTZ,
    CONSTRAINT chk_absence_dates CHECK (end_date >= start_date)
);
COMMENT ON COLUMN absence_requests.absence_type IS 'Tùy org: Annual/Sick/Personal hoặc Excused/Unexcused';

-- D4. holidays — Ngày nghỉ (per-org)
CREATE TABLE holidays (
    holiday_id      SERIAL      PRIMARY KEY,
    org_id          INT         NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    holiday_date    DATE        NOT NULL,
    name            VARCHAR(100) NOT NULL,
    description     TEXT,
    is_recurring    BOOLEAN     NOT NULL DEFAULT FALSE,
    recurring_month SMALLINT    CHECK (recurring_month BETWEEN 1 AND 12),
    recurring_day   SMALLINT    CHECK (recurring_day BETWEEN 1 AND 31),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_holiday_org_date UNIQUE (org_id, holiday_date),
    CONSTRAINT chk_recurring       CHECK (
        (is_recurring = FALSE AND recurring_month IS NULL AND recurring_day IS NULL)
        OR (is_recurring = TRUE AND recurring_month IS NOT NULL AND recurring_day IS NOT NULL)
    )
);

-- ────────────────────────────────────────────────────────────────
-- [E] HỆ THỐNG
-- ────────────────────────────────────────────────────────────────

-- E1. org_settings — Cài đặt hệ thống (per-org)
CREATE TABLE org_settings (
    setting_id    SERIAL       PRIMARY KEY,
    org_id        INT          NOT NULL REFERENCES organizations(org_id) ON DELETE CASCADE,
    setting_key   VARCHAR(100) NOT NULL,
    setting_value TEXT,
    data_type     VARCHAR(20)  NOT NULL DEFAULT 'String'
                  CHECK (data_type IN ('String','Int','Decimal','Boolean','Json')),
    description   TEXT,
    is_editable   BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    TIMESTAMPTZ,
    CONSTRAINT uq_setting_org_key UNIQUE (org_id, setting_key)
);

-- E2. audit_logs — Nhật ký thao tác
CREATE TABLE audit_logs (
    log_id      BIGSERIAL   PRIMARY KEY,
    org_id      INT         REFERENCES organizations(org_id) ON DELETE SET NULL,
    account_id  INT         REFERENCES accounts(account_id) ON DELETE SET NULL,
    action      VARCHAR(50) NOT NULL,
    table_name  VARCHAR(50),
    record_id   TEXT,
    old_data    JSONB,
    new_data    JSONB,
    ip_address  INET,
    user_agent  TEXT,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- ================================================================
-- PHẦN 4: TẠO INDEXES
-- ================================================================

-- [A] Tổ chức
CREATE INDEX idx_org_type      ON organizations(org_type);
CREATE INDEX idx_unit_org      ON units(org_id);
CREATE INDEX idx_unit_parent   ON units(parent_id) WHERE parent_id IS NOT NULL;
CREATE INDEX idx_unit_active   ON units(org_id, is_active) WHERE is_active = TRUE;
CREATE INDEX idx_role_org      ON roles(org_id);
CREATE INDEX idx_sched_org     ON schedules(org_id);
CREATE INDEX idx_sched_type    ON schedules(org_id, schedule_type);

-- [B] Thành viên
CREATE INDEX idx_member_org    ON members(org_id);
CREATE INDEX idx_member_active ON members(org_id, is_active) WHERE is_active = TRUE;
CREATE INDEX idx_member_name   ON members(org_id, full_name);
CREATE INDEX idx_member_face   ON members(org_id) WHERE is_face_registered = FALSE AND is_active = TRUE;
CREATE INDEX idx_mu_member     ON member_units(member_id);
CREATE INDEX idx_mu_unit       ON member_units(unit_id);
CREATE INDEX idx_mr_member     ON member_roles(member_id);
CREATE INDEX idx_mr_role       ON member_roles(role_id);
CREATE INDEX idx_acc_org       ON accounts(org_id);
CREATE INDEX idx_acc_member    ON accounts(member_id) WHERE member_id IS NOT NULL;

-- [C] Nhận diện
CREATE INDEX idx_fm_org        ON face_models(org_id);
CREATE INDEX idx_fm_active     ON face_models(org_id, is_active) WHERE is_active = TRUE;
CREATE INDEX idx_fd_member     ON face_data(member_id, model_id, is_active) WHERE is_active = TRUE;
CREATE INDEX idx_fd_org        ON face_data(org_id);
CREATE INDEX idx_dev_org       ON devices(org_id);
CREATE INDEX idx_dev_unit      ON devices(unit_id) WHERE unit_id IS NOT NULL;

-- [D] Chấm công
CREATE INDEX idx_ms_member     ON member_schedules(member_id, effective_date);
CREATE INDEX idx_att_org_date  ON attendance_logs(org_id, attendance_date);
CREATE INDEX idx_att_mem_date  ON attendance_logs(member_id, attendance_date);
CREATE INDEX idx_att_status    ON attendance_logs(status) WHERE status != 'Present';
CREATE INDEX idx_abs_org       ON absence_requests(org_id);
CREATE INDEX idx_abs_member    ON absence_requests(member_id);
CREATE INDEX idx_abs_pending   ON absence_requests(org_id, status) WHERE status = 'Pending';
CREATE INDEX idx_hol_org_date  ON holidays(org_id, holiday_date);

-- [E] Hệ thống
CREATE INDEX idx_aulog_org     ON audit_logs(org_id) WHERE org_id IS NOT NULL;
CREATE INDEX idx_aulog_created ON audit_logs(created_at);
CREATE INDEX idx_aulog_table   ON audit_logs(table_name, action);

-- ================================================================
-- PHẦN 5: TẠO FUNCTIONS & TRIGGERS
-- ================================================================

-- 5.1 Auto-update updated_at khi UPDATE bản ghi
CREATE OR REPLACE FUNCTION fn_update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_org_updated     BEFORE UPDATE ON organizations    FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_unit_updated    BEFORE UPDATE ON units            FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_role_updated    BEFORE UPDATE ON roles            FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_sched_updated   BEFORE UPDATE ON schedules        FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_member_updated  BEFORE UPDATE ON members          FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_acc_updated     BEFORE UPDATE ON accounts         FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_fm_updated      BEFORE UPDATE ON face_models      FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_fd_updated      BEFORE UPDATE ON face_data        FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_dev_updated     BEFORE UPDATE ON devices          FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_att_updated     BEFORE UPDATE ON attendance_logs   FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_abs_updated     BEFORE UPDATE ON absence_requests  FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
CREATE TRIGGER trg_setting_updated BEFORE UPDATE ON org_settings      FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

-- 5.2 Auto-update is_face_registered khi thay đổi face_data
CREATE OR REPLACE FUNCTION fn_update_face_status()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE members SET is_face_registered = TRUE WHERE member_id = NEW.member_id;
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE members SET is_face_registered = EXISTS(
            SELECT 1 FROM face_data
            WHERE member_id = OLD.member_id AND is_active = TRUE AND face_id != OLD.face_id
        ) WHERE member_id = OLD.member_id;
        RETURN OLD;
    ELSIF TG_OP = 'UPDATE' AND OLD.is_active IS DISTINCT FROM NEW.is_active THEN
        UPDATE members SET is_face_registered = EXISTS(
            SELECT 1 FROM face_data
            WHERE member_id = NEW.member_id AND is_active = TRUE
        ) WHERE member_id = NEW.member_id;
        RETURN NEW;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_face_status
AFTER INSERT OR UPDATE OF is_active OR DELETE ON face_data
FOR EACH ROW EXECUTE FUNCTION fn_update_face_status();

-- 5.3 Tự động tạo partition tháng tiếp theo cho attendance_logs
-- Cách dùng: SELECT fn_create_attendance_partition();
-- Nên chạy hàng tháng (pg_cron hoặc cron job)
CREATE OR REPLACE FUNCTION fn_create_attendance_partition()
RETURNS void AS $$
DECLARE
    next_month DATE;
    part_name  TEXT;
    p_start    DATE;
    p_end      DATE;
BEGIN
    next_month := DATE_TRUNC('month', CURRENT_DATE + INTERVAL '1 month');
    part_name  := 'att_' || TO_CHAR(next_month, 'YYYY_MM');
    p_start    := next_month;
    p_end      := next_month + INTERVAL '1 month';
    IF NOT EXISTS (SELECT 1 FROM pg_class WHERE relname = part_name) THEN
        EXECUTE format(
            'CREATE TABLE %I PARTITION OF attendance_logs FOR VALUES FROM (%L) TO (%L)',
            part_name, p_start, p_end
        );
        RAISE NOTICE 'Đã tạo partition: %', part_name;
    ELSE
        RAISE NOTICE 'Partition % đã tồn tại', part_name;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- ================================================================
-- PHẦN 6: TẠO VIEWS
-- ================================================================

-- 6.1 Chấm công hôm nay (filter theo org_id khi query)
CREATE OR REPLACE VIEW v_today_attendance AS
SELECT
    m.org_id,
    m.member_id,       m.code AS member_code,      m.full_name,
    s.name AS schedule_name,   s.start_time,        s.end_time,
    a.check_in,        a.check_out,                 a.status,
    a.late_minutes,    a.early_minutes,             a.working_hours,
    a.check_in_confidence,     a.check_out_confidence,
    fm.name AS checkin_model,  d.name AS device_name
FROM members m
LEFT JOIN attendance_logs a  ON m.member_id = a.member_id AND a.attendance_date = CURRENT_DATE
LEFT JOIN schedules s        ON COALESCE(a.schedule_id, m.default_schedule_id) = s.schedule_id
LEFT JOIN face_models fm     ON a.check_in_model_id = fm.model_id
LEFT JOIN devices d          ON a.device_id = d.device_id
WHERE m.is_active = TRUE;

-- 6.2 Tổng hợp chấm công theo tháng
CREATE OR REPLACE VIEW v_monthly_summary AS
SELECT
    m.org_id,          m.member_id,
    m.code AS member_code,                          m.full_name,
    DATE_TRUNC('month', a.attendance_date)          AS month,
    COUNT(*)                                        AS total_records,
    COUNT(*) FILTER (WHERE a.status = 'Present')    AS present_days,
    COUNT(*) FILTER (WHERE a.status = 'Late')       AS late_days,
    COUNT(*) FILTER (WHERE a.status = 'EarlyLeave') AS early_leave_days,
    COUNT(*) FILTER (WHERE a.status = 'Absent')     AS absent_days,
    COUNT(*) FILTER (WHERE a.status = 'Leave')      AS leave_days,
    SUM(COALESCE(a.late_minutes, 0))                AS total_late_minutes,
    SUM(COALESCE(a.working_hours, 0))               AS total_working_hours,
    SUM(COALESCE(a.overtime_hours, 0))               AS total_overtime_hours
FROM members m
INNER JOIN attendance_logs a ON m.member_id = a.member_id
WHERE m.is_active = TRUE
GROUP BY m.org_id, m.member_id, m.code, m.full_name, DATE_TRUNC('month', a.attendance_date);

-- 6.3 Thành viên chưa đăng ký Face ID
CREATE OR REPLACE VIEW v_members_no_face AS
SELECT org_id, member_id, code, full_name, join_date
FROM members
WHERE is_active = TRUE AND is_face_registered = FALSE;

-- 6.4 Đơn xin nghỉ đang chờ duyệt
CREATE OR REPLACE VIEW v_pending_absences AS
SELECT
    ar.org_id,         ar.request_id,
    m.code AS member_code,      m.full_name,
    ar.absence_type,   ar.start_date,     ar.end_date,
    ar.total_days,     ar.reason,         ar.created_at AS requested_at
FROM absence_requests ar
INNER JOIN members m ON ar.member_id = m.member_id
WHERE ar.status = 'Pending'
ORDER BY ar.created_at;

-- 6.5 Thống kê model AI (encoding & members per model)
CREATE OR REPLACE VIEW v_face_model_stats AS
SELECT
    fm.org_id,         fm.model_id,
    fm.name AS model_name,      fm.model_type,
    fm.embedding_dimensions,    fm.match_threshold,    fm.is_default,
    COUNT(fd.face_id) FILTER (WHERE fd.is_active = TRUE)          AS active_encodings,
    COUNT(DISTINCT fd.member_id) FILTER (WHERE fd.is_active = TRUE) AS registered_members
FROM face_models fm
LEFT JOIN face_data fd ON fm.model_id = fd.model_id
WHERE fm.is_active = TRUE
GROUP BY fm.org_id, fm.model_id, fm.name, fm.model_type,
         fm.embedding_dimensions, fm.match_threshold, fm.is_default;

-- ================================================================
-- PHẦN 7: DỮ LIỆU MẪU — 2 TỔ CHỨC TRÊN CÙNG 1 CSDL
-- ================================================================

-- ── 7.1 Tổ chức ─────────────────────────────────────────────────

-- Org 1: Công ty ABC
INSERT INTO organizations (code, name, org_type, config) VALUES
('COMPANY_ABC', 'Công ty ABC', 'company',
 '{"member_label":"Nhân viên","unit_label":"Phòng ban","attendance_label":"Chấm công"}');

-- Org 2: Trường THPT XYZ
INSERT INTO organizations (code, name, org_type, config) VALUES
('SCHOOL_XYZ', 'Trường THPT XYZ', 'school',
 '{"member_label":"Học sinh","unit_label":"Lớp","attendance_label":"Điểm danh"}');

-- ── 7.2 Đơn vị ──────────────────────────────────────────────────

-- Công ty: 3 phòng ban (flat)
INSERT INTO units (org_id, code, name, unit_type, level) VALUES
(1, 'HR',    'Phòng Nhân sự',    'department', 0),
(1, 'IT',    'Phòng Công nghệ',  'department', 0),
(1, 'SALES', 'Phòng Kinh doanh', 'department', 0);

-- Trường: 2 khối → 3 lớp (hierarchy)
INSERT INTO units (org_id, code, name, unit_type, level) VALUES
(2, 'K12', 'Khối 12', 'grade', 0),
(2, 'K11', 'Khối 11', 'grade', 0);

INSERT INTO units (org_id, code, name, unit_type, parent_id, level) VALUES
(2, '12A1', 'Lớp 12A1', 'class', 4, 1),
(2, '12A2', 'Lớp 12A2', 'class', 4, 1),
(2, '11A1', 'Lớp 11A1', 'class', 5, 1);

-- ── 7.3 Vai trò ─────────────────────────────────────────────────

INSERT INTO roles (org_id, code, name, role_type, metadata) VALUES
(1, 'MGR',     'Trưởng phòng',      'position', '{"base_salary":25000000}'),
(1, 'SR',      'Nhân viên cao cấp', 'position', '{"base_salary":15000000}'),
(1, 'JR',      'Nhân viên',         'position', '{"base_salary":10000000}'),
(2, 'STUDENT', 'Học sinh',          'grade',    '{"grade_level":12}'),
(2, 'MONITOR', 'Lớp trưởng',       'grade',    '{"grade_level":12,"is_monitor":true}');

-- ── 7.4 Ca / Lịch ───────────────────────────────────────────────

-- Công ty: ca cố định
INSERT INTO schedules (org_id, code, name, schedule_type, start_time, end_time, break_minutes, is_overnight, rules) VALUES
(1, 'MAIN',  'Ca hành chính', 'fixed_shift', '08:00','17:00', 60, FALSE, '{"days_of_week":[1,2,3,4,5]}'),
(1, 'NIGHT', 'Ca đêm',        'fixed_shift', '22:00','06:00', 30, TRUE,  '{"days_of_week":[1,2,3,4,5]}');

-- Trường: buổi học theo tiết
INSERT INTO schedules (org_id, code, name, schedule_type, start_time, end_time, break_minutes, rules) VALUES
(2, 'MORNING',   'Buổi sáng',  'class_period', '07:00','11:30', 20,
 '{"periods":[{"start":"07:00","end":"07:45"},{"start":"07:50","end":"08:35"},{"start":"08:55","end":"09:40"},{"start":"09:45","end":"10:30"},{"start":"10:35","end":"11:20"}]}'),
(2, 'AFTERNOON', 'Buổi chiều', 'class_period', '13:00','17:00', 15,
 '{"periods":[{"start":"13:00","end":"13:45"},{"start":"13:50","end":"14:35"},{"start":"14:55","end":"15:40"},{"start":"15:45","end":"16:30"}]}');

-- ── 7.5 Model nhận diện ─────────────────────────────────────────

-- Công ty dùng dlib (128D, euclidean)
INSERT INTO face_models (org_id, code, name, model_type, embedding_dimensions, distance_metric, match_threshold, model_version, is_default, config) VALUES
(1, 'DLIB_V1', 'dlib ResNet v1', 'dlib_resnet', 128, 'euclidean', 0.6, 'v1.0', TRUE,
 '{"model_file":"dlib_face_recognition_resnet_model_v1.dat","predictor":"shape_predictor_68_face_landmarks.dat"}');

-- Trường dùng ArcFace (512D, cosine)
INSERT INTO face_models (org_id, code, name, model_type, embedding_dimensions, distance_metric, match_threshold, model_version, is_default, config) VALUES
(2, 'ARCFACE_V1', 'ArcFace R100', 'arcface', 512, 'cosine', 0.4, 'r100', TRUE,
 '{"input_size":[112,112],"backend":"onnx","preprocessing":"retinaface"}');

-- ── 7.6 Thành viên ──────────────────────────────────────────────

-- Công ty
INSERT INTO members (org_id, code, full_name, gender, phone, email, default_schedule_id, metadata) VALUES
(1, 'NV001', 'Nguyễn Văn An',  'M', '0901234567', 'an@abc.com',   1, '{"badge":"B-001"}'),
(1, 'NV002', 'Trần Thị Bình',  'F', '0901234568', 'binh@abc.com', 1, '{"badge":"B-002"}'),
(1, 'NV003', 'Lê Văn Cường',   'M', '0901234569', 'cuong@abc.com',1, '{"badge":"B-003"}');

-- Trường
INSERT INTO members (org_id, code, full_name, gender, date_of_birth, default_schedule_id, metadata) VALUES
(2, 'HS001', 'Phạm Minh Đức',  'M', '2008-05-15', 3, '{"student_id":"HS202601","parent_phone":"0912345678"}'),
(2, 'HS002', 'Hoàng Thị Em',   'F', '2008-08-20', 3, '{"student_id":"HS202602","parent_phone":"0912345679"}'),
(2, 'HS003', 'Ngô Văn Phúc',   'M', '2009-01-10', 3, '{"student_id":"HS202603","parent_phone":"0912345680"}');

-- ── 7.7 Phân bổ đơn vị & vai trò ───────────────────────────────

INSERT INTO member_units (member_id, unit_id) VALUES
(1, 1), (2, 2), (3, 2),     -- Công ty: An→HR, Bình→IT, Cường→IT
(4, 6), (5, 6), (6, 8);     -- Trường: Đức→12A1, Em→12A1, Phúc→11A1

INSERT INTO member_roles (member_id, role_id) VALUES
(1, 1), (2, 2), (3, 3),     -- Công ty: An=MGR, Bình=SR, Cường=JR
(4, 4), (5, 5), (6, 4);     -- Trường: Đức=HS, Em=Lớp trưởng, Phúc=HS

-- ── 7.8 Thiết bị ────────────────────────────────────────────────

INSERT INTO devices (org_id, code, name, device_type, location, model_id) VALUES
(1, 'CAM-LOBBY', 'Camera Sảnh chính',  'camera', 'Tầng 1 - Sảnh vào', 1),
(2, 'CAM-GATE',  'Camera Cổng trường', 'camera', 'Cổng chính',        2);

-- ── 7.9 Tài khoản admin ─────────────────────────────────────────

INSERT INTO accounts (org_id, username, password_hash, account_role) VALUES
(1, 'admin_abc', '$2a$10$THAY_HASH_THUC_TE_O_DAY', 'Admin'),
(2, 'admin_xyz', '$2a$10$THAY_HASH_THUC_TE_O_DAY', 'Admin');

-- ── 7.10 Cài đặt per-org ────────────────────────────────────────

-- Công ty
INSERT INTO org_settings (org_id, setting_key, setting_value, data_type, description) VALUES
(1, 'face_threshold',   '0.6',   'Decimal', 'Ngưỡng nhận diện khuôn mặt'),
(1, 'max_face_images',  '5',     'Int',     'Số ảnh Face ID tối đa'),
(1, 'late_threshold',   '15',    'Int',     'Ngưỡng đi muộn (phút)'),
(1, 'require_checkout', 'true',  'Boolean', 'Bắt buộc checkout'),
(1, 'auto_checkout',    '23:59', 'String',  'Giờ tự động checkout');

-- Trường
INSERT INTO org_settings (org_id, setting_key, setting_value, data_type, description) VALUES
(2, 'face_threshold',   '0.4',   'Decimal', 'Ngưỡng nhận diện (ArcFace thấp hơn dlib)'),
(2, 'max_face_images',  '3',     'Int',     'Số ảnh tối đa'),
(2, 'late_threshold',   '5',     'Int',     'Ngưỡng trễ (phút)'),
(2, 'require_checkout', 'false', 'Boolean', 'HS không bắt buộc checkout');

-- ── 7.11 Ngày lễ / nghỉ ─────────────────────────────────────────

INSERT INTO holidays (org_id, holiday_date, name, is_recurring, recurring_month, recurring_day) VALUES
(1, '2026-01-01', 'Tết Dương lịch',      TRUE,  1,  1),
(1, '2026-04-30', 'Giải phóng miền Nam', TRUE,  4, 30),
(1, '2026-05-01', 'Quốc tế Lao động',    TRUE,  5,  1),
(1, '2026-09-02', 'Quốc khánh',           TRUE,  9,  2),
(2, '2026-01-01', 'Tết Dương lịch',      TRUE,  1,  1),
(2, '2026-05-31', 'Bắt đầu nghỉ hè',    FALSE, NULL, NULL);

-- ================================================================
-- PHẦN 8: GHI CHÚ VẬN HÀNH
-- ================================================================
-- 1. TẠO PARTITION HÀNG THÁNG:
--    SELECT fn_create_attendance_partition();
--    Hoặc dùng pg_cron:
--    SELECT cron.schedule('monthly_partition', '0 0 25 * *',
--           'SELECT fn_create_attendance_partition()');
--
-- 2. ARCHIVAL (gỡ partition cũ):
--    ALTER TABLE attendance_logs DETACH PARTITION att_2025_01;
--
-- 3. ĐỔI MODEL AI CHO 1 TỔ CHỨC:
--    a. INSERT model mới vào face_models, SET is_default = TRUE
--    b. UPDATE model cũ SET is_default = FALSE
--    c. Hệ thống sẽ yêu cầu member đăng ký lại khuôn mặt
--
-- 4. KIỂM TRA MODEL STATS:
--    SELECT * FROM v_face_model_stats WHERE org_id = 1;
--
-- 5. QUERY MẪU — lấy chấm công của 1 org:
--    SELECT * FROM v_today_attendance WHERE org_id = 1;
--    SELECT * FROM v_monthly_summary  WHERE org_id = 2 AND month = '2026-03-01';
-- ================================================================
-- KẾT THÚC SCRIPT
-- ================================================================
