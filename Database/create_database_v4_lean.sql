-- ================================================================
-- HỆ THỐNG CHẤM CÔNG BẰNG NHẬN DIỆN KHUÔN MẶT
-- Phiên bản: 4.0 (Tinh gọn, Đa tổ chức, Tiếng Việt)
-- Database: PostgreSQL 16+
-- Ngày: 2026-03-04
-- ================================================================
-- SỐ BẢNG: 7 (giảm từ 17 bảng v3)
--   1. to_chuc           — Tổ chức (tenant)
--   2. don_vi            — Đơn vị quản lý (đa cấp)
--   3. vai_tro           — Vai trò / chức vụ
--   4. nguoi_dung        — Người dùng (gộp accounts)
--   5. ca_lam_viec       — Ca / lịch / tiết học
--   6. du_lieu_khuon_mat — Encoding khuôn mặt
--   7. lich_su_cham_cong — Bản ghi chấm công
-- ================================================================
-- CÁCH DÙNG:
--   1. Tạo database: CREATE DATABASE cham_cong_face_id;
--   2. Kết nối vào database
--   3. Chạy toàn bộ file này
-- ================================================================

SET client_encoding = 'UTF8';
SET timezone = 'Asia/Ho_Chi_Minh';

-- ================================================================
-- XÓA BẢNG CŨ
-- ================================================================
DROP TABLE IF EXISTS lich_su_cham_cong CASCADE;
DROP TABLE IF EXISTS du_lieu_khuon_mat CASCADE;
DROP TABLE IF EXISTS nguoi_dung CASCADE;
DROP TABLE IF EXISTS ca_lam_viec CASCADE;
DROP TABLE IF EXISTS vai_tro CASCADE;
DROP TABLE IF EXISTS don_vi CASCADE;
DROP TABLE IF EXISTS to_chuc CASCADE;

-- ================================================================
-- BẢNG 1: to_chuc
-- Tenant gốc. Mỗi tổ chức tự cấu hình qua cột cau_hinh (JSONB).
-- Không cần bảng org_settings, holidays, devices riêng.
-- ================================================================
CREATE TABLE to_chuc (
    id           SERIAL       PRIMARY KEY,
    ma           VARCHAR(30)  NOT NULL UNIQUE,
    ten          VARCHAR(150) NOT NULL,
    loai         VARCHAR(30)  NOT NULL
                 CHECK (loai IN ('truong_hoc','van_phong','nha_may','trung_tam_dao_tao','khac')),
    cau_hinh     JSONB        NOT NULL DEFAULT '{}',
    dang_su_dung BOOLEAN      NOT NULL DEFAULT TRUE,
    tao_luc      TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cap_nhat_luc TIMESTAMPTZ
);
-- cau_hinh chứa: nhan_nguoi_dung, nhan_don_vi, nguong_nhan_dien,
--   so_anh_toi_da, model_ai, ngay_nghi, ...
-- Ví dụ: {"nhan_nguoi_dung":"Học sinh", "nhan_don_vi":"Lớp",
--          "model_ai":{"loai":"arcface","kich_thuoc_vector":512,
--          "do_do":"cosine","nguong":0.4},
--          "ngay_nghi":["2026-01-01","2026-04-30","2026-05-01"]}

-- ================================================================
-- BẢNG 2: don_vi
-- Đơn vị quản lý đa cấp: Phòng ban / Lớp / Tổ đội / Khoa
-- Dùng cha_don_vi_id để tạo cây. Không cần bảng riêng.
-- ================================================================
CREATE TABLE don_vi (
    id             SERIAL       PRIMARY KEY,
    to_chuc_id     INT          NOT NULL REFERENCES to_chuc(id) ON DELETE CASCADE,
    ma             VARCHAR(30)  NOT NULL,
    ten            VARCHAR(150) NOT NULL,
    loai           VARCHAR(30),
    cha_don_vi_id  INT          REFERENCES don_vi(id) ON DELETE SET NULL,
    thu_tu         SMALLINT     NOT NULL DEFAULT 0,
    dang_su_dung   BOOLEAN      NOT NULL DEFAULT TRUE,
    tao_luc        TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cap_nhat_luc   TIMESTAMPTZ,
    UNIQUE (to_chuc_id, ma)
);
-- loai: 'phong_ban', 'lop', 'khoi', 'to_doi', 'day_chuyen', ...

-- ================================================================
-- BẢNG 3: vai_tro
-- Vai trò / chức vụ / loại thành viên.
-- Metadata mở rộng qua JSONB (lương, cấp bậc, ...).
-- ================================================================
CREATE TABLE vai_tro (
    id           SERIAL       PRIMARY KEY,
    to_chuc_id   INT          NOT NULL REFERENCES to_chuc(id) ON DELETE CASCADE,
    ma           VARCHAR(30)  NOT NULL,
    ten          VARCHAR(100) NOT NULL,
    mo_rong      JSONB        NOT NULL DEFAULT '{}',
    thu_tu       SMALLINT     NOT NULL DEFAULT 0,
    dang_su_dung BOOLEAN      NOT NULL DEFAULT TRUE,
    tao_luc      TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cap_nhat_luc TIMESTAMPTZ,
    UNIQUE (to_chuc_id, ma)
);
-- mo_rong: {"luong_co_ban":25000000} hoặc {"khoi_lop":12}

-- ================================================================
-- BẢNG 4: ca_lam_viec
-- Ca / tiết học / buổi. Dùng chung cho mọi loại tổ chức.
-- Chi tiết linh hoạt qua cột quy_tac (JSONB).
-- ================================================================
CREATE TABLE ca_lam_viec (
    id              SERIAL       PRIMARY KEY,
    to_chuc_id      INT          NOT NULL REFERENCES to_chuc(id) ON DELETE CASCADE,
    ma              VARCHAR(30)  NOT NULL,
    ten             VARCHAR(100) NOT NULL,
    loai            VARCHAR(30)  NOT NULL DEFAULT 'ca_co_dinh'
                    CHECK (loai IN ('ca_co_dinh','tiet_hoc','ca_xoay','linh_hoat')),
    gio_bat_dau     TIME         NOT NULL,
    gio_ket_thuc    TIME         NOT NULL,
    qua_dem         BOOLEAN      NOT NULL DEFAULT FALSE,
    phut_nghi       SMALLINT     NOT NULL DEFAULT 0,
    phut_tre_cho_phep  SMALLINT  NOT NULL DEFAULT 15,
    phut_som_cho_phep  SMALLINT  NOT NULL DEFAULT 15,
    quy_tac         JSONB        NOT NULL DEFAULT '{}',
    dang_su_dung    BOOLEAN      NOT NULL DEFAULT TRUE,
    tao_luc         TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cap_nhat_luc    TIMESTAMPTZ,
    UNIQUE (to_chuc_id, ma)
);
-- quy_tac ví dụ:
--   Ca cố định: {"ngay_ap_dung":[1,2,3,4,5]}
--   Tiết học:    {"tiet":[{"bat_dau":"07:00","ket_thuc":"07:45"},
--                         {"bat_dau":"07:50","ket_thuc":"08:35"}]}

-- ================================================================
-- BẢNG 5: nguoi_dung
-- Gộp members + accounts thành 1 bảng.
-- Ai cần đăng nhập → có ten_dang_nhap + mat_khau_hash.
-- Ai chỉ chấm công → 2 cột đó NULL.
-- ================================================================
CREATE TABLE nguoi_dung (
    id              SERIAL       PRIMARY KEY,
    to_chuc_id      INT          NOT NULL REFERENCES to_chuc(id) ON DELETE CASCADE,
    ma              VARCHAR(30)  NOT NULL,
    ho_ten          VARCHAR(100) NOT NULL,
    gioi_tinh       CHAR(1)      CHECK (gioi_tinh IN ('M','F','O')),
    ngay_sinh       DATE,
    dien_thoai      VARCHAR(20),
    email           VARCHAR(100),
    don_vi_id       INT          REFERENCES don_vi(id) ON DELETE SET NULL,
    vai_tro_id      INT          REFERENCES vai_tro(id) ON DELETE SET NULL,
    ca_mac_dinh_id  INT          REFERENCES ca_lam_viec(id) ON DELETE SET NULL,
    ten_dang_nhap   VARCHAR(50),
    mat_khau_hash   VARCHAR(255),
    quyen           VARCHAR(20)  DEFAULT 'nguoi_dung'
                    CHECK (quyen IN ('quan_tri','quan_ly','nguoi_dung')),
    ngay_tham_gia   DATE         NOT NULL DEFAULT CURRENT_DATE,
    da_dang_ky_mat  BOOLEAN      NOT NULL DEFAULT FALSE,
    dang_su_dung    BOOLEAN      NOT NULL DEFAULT TRUE,
    mo_rong         JSONB        NOT NULL DEFAULT '{}',
    tao_luc         TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cap_nhat_luc    TIMESTAMPTZ,
    UNIQUE (to_chuc_id, ma),
    UNIQUE (to_chuc_id, ten_dang_nhap),
    UNIQUE (to_chuc_id, email)
);
-- mo_rong: {"ma_hoc_sinh":"HS2026","sdt_phu_huynh":"091234"}
--      hoặc {"so_the":"B-001"}

-- ================================================================
-- BẢNG 6: du_lieu_khuon_mat
-- Face embeddings. Cột loai_model theo cấu hình model_ai của to_chuc.
-- Không cần bảng face_models riêng — mỗi org dùng 1 model,
-- thông tin model lưu trong to_chuc.cau_hinh.model_ai
-- ================================================================
CREATE TABLE du_lieu_khuon_mat (
    id              SERIAL      PRIMARY KEY,
    to_chuc_id      INT         NOT NULL REFERENCES to_chuc(id) ON DELETE CASCADE,
    nguoi_dung_id   INT         NOT NULL REFERENCES nguoi_dung(id) ON DELETE CASCADE,
    vector_encoding BYTEA       NOT NULL,
    duong_dan_anh   TEXT,
    thu_tu_anh      SMALLINT    NOT NULL DEFAULT 1 CHECK (thu_tu_anh BETWEEN 1 AND 10),
    chat_luong      REAL        CHECK (chat_luong >= 0 AND chat_luong <= 1),
    loai_model      VARCHAR(30) NOT NULL DEFAULT 'dlib_resnet',
    dang_su_dung    BOOLEAN     NOT NULL DEFAULT TRUE,
    tao_luc         TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cap_nhat_luc    TIMESTAMPTZ,
    UNIQUE (nguoi_dung_id, loai_model, thu_tu_anh)
);
-- loai_model: 'dlib_resnet', 'facenet', 'arcface', 'insightface'
-- Khi đổi model: tạo encoding mới, set cũ dang_su_dung=FALSE

-- ================================================================
-- BẢNG 7: lich_su_cham_cong
-- Bản ghi chấm công — PARTITIONED theo tháng.
-- ================================================================
CREATE TABLE lich_su_cham_cong (
    id               BIGSERIAL,
    to_chuc_id       INT          NOT NULL REFERENCES to_chuc(id) ON DELETE CASCADE,
    nguoi_dung_id    INT          NOT NULL REFERENCES nguoi_dung(id) ON DELETE RESTRICT,
    ngay             DATE         NOT NULL DEFAULT CURRENT_DATE,
    ca_id            INT          REFERENCES ca_lam_viec(id) ON DELETE SET NULL,
    thoi_gian_vao    TIMESTAMPTZ,
    thoi_gian_ra     TIMESTAMPTZ,
    anh_vao          TEXT,
    anh_ra           TEXT,
    do_tin_cay_vao   REAL,
    do_tin_cay_ra    REAL,
    trang_thai       VARCHAR(20)  NOT NULL DEFAULT 'co_mat'
                     CHECK (trang_thai IN ('co_mat','di_tre','ve_som','vang_mat','nghi_phep','nghi_le')),
    phut_tre         SMALLINT     NOT NULL DEFAULT 0,
    phut_ve_som      SMALLINT     NOT NULL DEFAULT 0,
    gio_lam          DECIMAL(5,2),
    tang_ca          DECIMAL(5,2) NOT NULL DEFAULT 0,
    ghi_chu          TEXT,
    tao_luc          TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cap_nhat_luc     TIMESTAMPTZ,

    PRIMARY KEY (id, ngay),
    UNIQUE (nguoi_dung_id, ngay),
    CHECK (thoi_gian_ra IS NULL OR thoi_gian_vao IS NULL OR thoi_gian_ra >= thoi_gian_vao),
    CHECK (do_tin_cay_vao IS NULL OR (do_tin_cay_vao >= 0 AND do_tin_cay_vao <= 1)),
    CHECK (do_tin_cay_ra  IS NULL OR (do_tin_cay_ra  >= 0 AND do_tin_cay_ra  <= 1))
) PARTITION BY RANGE (ngay);

-- Partitions 2026
CREATE TABLE cc_2026_01 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-01-01') TO ('2026-02-01');
CREATE TABLE cc_2026_02 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-02-01') TO ('2026-03-01');
CREATE TABLE cc_2026_03 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-03-01') TO ('2026-04-01');
CREATE TABLE cc_2026_04 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-04-01') TO ('2026-05-01');
CREATE TABLE cc_2026_05 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-05-01') TO ('2026-06-01');
CREATE TABLE cc_2026_06 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-06-01') TO ('2026-07-01');
CREATE TABLE cc_2026_07 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-07-01') TO ('2026-08-01');
CREATE TABLE cc_2026_08 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-08-01') TO ('2026-09-01');
CREATE TABLE cc_2026_09 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-09-01') TO ('2026-10-01');
CREATE TABLE cc_2026_10 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-10-01') TO ('2026-11-01');
CREATE TABLE cc_2026_11 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-11-01') TO ('2026-12-01');
CREATE TABLE cc_2026_12 PARTITION OF lich_su_cham_cong FOR VALUES FROM ('2026-12-01') TO ('2027-01-01');

-- ================================================================
-- INDEXES
-- ================================================================
CREATE INDEX idx_dv_to_chuc    ON don_vi(to_chuc_id);
CREATE INDEX idx_dv_cha        ON don_vi(cha_don_vi_id) WHERE cha_don_vi_id IS NOT NULL;
CREATE INDEX idx_vt_to_chuc    ON vai_tro(to_chuc_id);
CREATE INDEX idx_nd_to_chuc    ON nguoi_dung(to_chuc_id);
CREATE INDEX idx_nd_don_vi     ON nguoi_dung(don_vi_id);
CREATE INDEX idx_nd_dang_dung  ON nguoi_dung(to_chuc_id, dang_su_dung) WHERE dang_su_dung = TRUE;
CREATE INDEX idx_nd_chua_mat   ON nguoi_dung(to_chuc_id) WHERE da_dang_ky_mat = FALSE AND dang_su_dung = TRUE;
CREATE INDEX idx_km_nguoi_dung ON du_lieu_khuon_mat(nguoi_dung_id, loai_model, dang_su_dung) WHERE dang_su_dung = TRUE;
CREATE INDEX idx_cc_org_ngay   ON lich_su_cham_cong(to_chuc_id, ngay);
CREATE INDEX idx_cc_nd_ngay    ON lich_su_cham_cong(nguoi_dung_id, ngay);
CREATE INDEX idx_cc_trang_thai ON lich_su_cham_cong(trang_thai) WHERE trang_thai != 'co_mat';

-- ================================================================
-- TRIGGERS
-- ================================================================

-- Auto-update cap_nhat_luc
CREATE OR REPLACE FUNCTION fn_cap_nhat_thoi_gian()
RETURNS TRIGGER AS $$
BEGIN
    NEW.cap_nhat_luc = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_to_chuc     BEFORE UPDATE ON to_chuc           FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_thoi_gian();
CREATE TRIGGER trg_don_vi      BEFORE UPDATE ON don_vi             FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_thoi_gian();
CREATE TRIGGER trg_vai_tro     BEFORE UPDATE ON vai_tro            FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_thoi_gian();
CREATE TRIGGER trg_ca          BEFORE UPDATE ON ca_lam_viec        FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_thoi_gian();
CREATE TRIGGER trg_nguoi_dung  BEFORE UPDATE ON nguoi_dung         FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_thoi_gian();
CREATE TRIGGER trg_khuon_mat   BEFORE UPDATE ON du_lieu_khuon_mat  FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_thoi_gian();
CREATE TRIGGER trg_cham_cong   BEFORE UPDATE ON lich_su_cham_cong  FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_thoi_gian();

-- Auto-update da_dang_ky_mat
CREATE OR REPLACE FUNCTION fn_cap_nhat_trang_thai_mat()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE nguoi_dung SET da_dang_ky_mat = TRUE WHERE id = NEW.nguoi_dung_id;
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE nguoi_dung SET da_dang_ky_mat = EXISTS(
            SELECT 1 FROM du_lieu_khuon_mat
            WHERE nguoi_dung_id = OLD.nguoi_dung_id AND dang_su_dung = TRUE AND id != OLD.id
        ) WHERE id = OLD.nguoi_dung_id;
        RETURN OLD;
    ELSIF TG_OP = 'UPDATE' AND OLD.dang_su_dung IS DISTINCT FROM NEW.dang_su_dung THEN
        UPDATE nguoi_dung SET da_dang_ky_mat = EXISTS(
            SELECT 1 FROM du_lieu_khuon_mat
            WHERE nguoi_dung_id = NEW.nguoi_dung_id AND dang_su_dung = TRUE
        ) WHERE id = NEW.nguoi_dung_id;
        RETURN NEW;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_trang_thai_mat
AFTER INSERT OR UPDATE OF dang_su_dung OR DELETE ON du_lieu_khuon_mat
FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_trang_thai_mat();

-- Auto-create partition tháng tiếp theo
CREATE OR REPLACE FUNCTION fn_tao_partition_thang()
RETURNS void AS $$
DECLARE
    thang_sau DATE;
    ten_bang  TEXT;
BEGIN
    thang_sau := DATE_TRUNC('month', CURRENT_DATE + INTERVAL '1 month');
    ten_bang  := 'cc_' || TO_CHAR(thang_sau, 'YYYY_MM');
    IF NOT EXISTS (SELECT 1 FROM pg_class WHERE relname = ten_bang) THEN
        EXECUTE format(
            'CREATE TABLE %I PARTITION OF lich_su_cham_cong FOR VALUES FROM (%L) TO (%L)',
            ten_bang, thang_sau, thang_sau + INTERVAL '1 month'
        );
    END IF;
END;
$$ LANGUAGE plpgsql;

-- ================================================================
-- VIEWS
-- ================================================================

-- Chấm công hôm nay
CREATE VIEW v_cham_cong_hom_nay AS
SELECT
    nd.to_chuc_id,     nd.id AS nguoi_dung_id,
    nd.ma,             nd.ho_ten,
    ca.ten AS ten_ca,  ca.gio_bat_dau,    ca.gio_ket_thuc,
    cc.thoi_gian_vao,  cc.thoi_gian_ra,   cc.trang_thai,
    cc.phut_tre,       cc.gio_lam,
    cc.do_tin_cay_vao, cc.do_tin_cay_ra
FROM nguoi_dung nd
LEFT JOIN lich_su_cham_cong cc ON nd.id = cc.nguoi_dung_id AND cc.ngay = CURRENT_DATE
LEFT JOIN ca_lam_viec ca       ON COALESCE(cc.ca_id, nd.ca_mac_dinh_id) = ca.id
WHERE nd.dang_su_dung = TRUE;

-- Tổng hợp tháng
CREATE VIEW v_tong_hop_thang AS
SELECT
    nd.to_chuc_id,     nd.id AS nguoi_dung_id,
    nd.ma,             nd.ho_ten,
    DATE_TRUNC('month', cc.ngay)                       AS thang,
    COUNT(*)                                            AS tong,
    COUNT(*) FILTER (WHERE cc.trang_thai = 'co_mat')    AS co_mat,
    COUNT(*) FILTER (WHERE cc.trang_thai = 'di_tre')    AS di_tre,
    COUNT(*) FILTER (WHERE cc.trang_thai = 'vang_mat')  AS vang_mat,
    SUM(COALESCE(cc.phut_tre, 0))                       AS tong_phut_tre,
    SUM(COALESCE(cc.gio_lam, 0))                        AS tong_gio_lam,
    SUM(COALESCE(cc.tang_ca, 0))                        AS tong_tang_ca
FROM nguoi_dung nd
INNER JOIN lich_su_cham_cong cc ON nd.id = cc.nguoi_dung_id
WHERE nd.dang_su_dung = TRUE
GROUP BY nd.to_chuc_id, nd.id, nd.ma, nd.ho_ten, DATE_TRUNC('month', cc.ngay);

-- ================================================================
-- DỮ LIỆU MẪU — 2 TỔ CHỨC DÙNG CHUNG CSDL
-- ================================================================

-- ── Tổ chức ─────────────────────────────────────────────────────

INSERT INTO to_chuc (ma, ten, loai, cau_hinh) VALUES
('CTY_ABC', 'Công ty ABC', 'van_phong',
 '{"nhan_nguoi_dung":"Nhân viên","nhan_don_vi":"Phòng ban",
   "model_ai":{"loai":"dlib_resnet","kich_thuoc_vector":128,"do_do":"euclidean","nguong":0.6},
   "ngay_nghi":["2026-01-01","2026-04-30","2026-05-01","2026-09-02"]}'),
('TH_XYZ', 'Trường THPT XYZ', 'truong_hoc',
 '{"nhan_nguoi_dung":"Học sinh","nhan_don_vi":"Lớp",
   "model_ai":{"loai":"arcface","kich_thuoc_vector":512,"do_do":"cosine","nguong":0.4},
   "ngay_nghi":["2026-01-01","2026-05-31"]}');

-- ── Đơn vị ──────────────────────────────────────────────────────

INSERT INTO don_vi (to_chuc_id, ma, ten, loai) VALUES
(1, 'HR',   'Phòng Nhân sự',   'phong_ban'),
(1, 'IT',   'Phòng Công nghệ', 'phong_ban'),
(2, 'K12',  'Khối 12',         'khoi');

INSERT INTO don_vi (to_chuc_id, ma, ten, loai, cha_don_vi_id) VALUES
(2, '12A1', 'Lớp 12A1', 'lop', 3);

-- ── Vai trò ─────────────────────────────────────────────────────

INSERT INTO vai_tro (to_chuc_id, ma, ten, mo_rong) VALUES
(1, 'TP',      'Trưởng phòng', '{"luong_co_ban":25000000}'),
(1, 'NV',      'Nhân viên',    '{"luong_co_ban":10000000}'),
(2, 'HOC_SINH','Học sinh',     '{"khoi_lop":12}');

-- ── Ca / lịch ───────────────────────────────────────────────────

INSERT INTO ca_lam_viec (to_chuc_id, ma, ten, loai, gio_bat_dau, gio_ket_thuc, phut_nghi, quy_tac) VALUES
(1, 'HC', 'Ca hành chính', 'ca_co_dinh', '08:00','17:00', 60,
 '{"ngay_ap_dung":[1,2,3,4,5]}'),
(2, 'SANG', 'Buổi sáng', 'tiet_hoc', '07:00','11:30', 20,
 '{"tiet":[{"bat_dau":"07:00","ket_thuc":"07:45"},{"bat_dau":"07:50","ket_thuc":"08:35"},{"bat_dau":"08:55","ket_thuc":"09:40"}]}');

-- ── Người dùng ──────────────────────────────────────────────────

INSERT INTO nguoi_dung (to_chuc_id, ma, ho_ten, gioi_tinh, don_vi_id, vai_tro_id, ca_mac_dinh_id, ten_dang_nhap, mat_khau_hash, quyen) VALUES
(1, 'NV001', 'Nguyễn Văn An', 'M', 1, 1, 1, 'admin', '$2a$10$HASH', 'quan_tri'),
(1, 'NV002', 'Trần Thị Bình', 'F', 2, 2, 1, NULL, NULL, 'nguoi_dung');

INSERT INTO nguoi_dung (to_chuc_id, ma, ho_ten, gioi_tinh, ngay_sinh, don_vi_id, vai_tro_id, ca_mac_dinh_id, mo_rong) VALUES
(2, 'HS001', 'Phạm Minh Đức', 'M', '2008-05-15', 4, 3, 2,
 '{"ma_hoc_sinh":"HS202601","sdt_phu_huynh":"0912345678"}');

-- ================================================================
-- SƠ ĐỒ QUAN HỆ
-- ================================================================
--
--  to_chuc (1)
--    ├──< don_vi (N)         [to_chuc_id]
--    │      └──< don_vi      [cha_don_vi_id] (self-ref, đa cấp)
--    ├──< vai_tro (N)        [to_chuc_id]
--    ├──< ca_lam_viec (N)    [to_chuc_id]
--    ├──< nguoi_dung (N)     [to_chuc_id]
--    │      ├─── don_vi      [don_vi_id]       (N-1)
--    │      ├─── vai_tro     [vai_tro_id]      (N-1)
--    │      ├─── ca_lam_viec [ca_mac_dinh_id]  (N-1)
--    │      ├──< du_lieu_khuon_mat (N)         [nguoi_dung_id]
--    │      └──< lich_su_cham_cong (N)         [nguoi_dung_id]
--    │             └─── ca_lam_viec [ca_id]    (N-1)
--    └──< du_lieu_khuon_mat (N) [to_chuc_id]
--    └──< lich_su_cham_cong (N) [to_chuc_id]
--
-- ================================================================
-- BẢNG ĐÃ XÓA (so với v3, 17 bảng → 7 bảng)
-- ================================================================
--  XÓA                  | LÝ DO
-- ──────────────────────┼──────────────────────────────────────
--  member_units         | Gộp: nguoi_dung.don_vi_id (1 người = 1 đơn vị)
--  member_roles         | Gộp: nguoi_dung.vai_tro_id (1 người = 1 vai trò)
--  member_schedules     | Gộp: nguoi_dung.ca_mac_dinh_id
--  accounts             | Gộp: ten_dang_nhap, mat_khau_hash vào nguoi_dung
--  face_models          | Gộp: to_chuc.cau_hinh.model_ai (JSONB)
--  devices              | Không thuộc luồng chính (thêm sau nếu cần)
--  absence_requests     | Không thuộc luồng chính (thêm sau nếu cần)
--  holidays             | Gộp: to_chuc.cau_hinh.ngay_nghi (JSONB array)
--  org_settings         | Gộp: to_chuc.cau_hinh (JSONB)
--  audit_logs           | Không thuộc luồng chính (thêm sau nếu cần)
-- ================================================================

-- Chạy hàng tháng: SELECT fn_tao_partition_thang();
-- ================================================================
-- KẾT THÚC SCRIPT
-- ================================================================
