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
