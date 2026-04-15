using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.CustomControls;
using FaceIDApp.Theme;

namespace FaceIDApp.UserControls
{
    public partial class UCEmployeeManagement : UserControl
    {
        private ModernDataGridView dgvEmployees;
        private ModernTextBox txtSearch;
        private ModernComboBox cboFilter;
        private RoundedButton btnAdd, btnEdit, btnDelete, btnFaceReg;
        private Panel pnlToolbar;
        private Label lblTotal;

        public UCEmployeeManagement()
        {
            InitializeUI();
            LoadSampleData();
        }

        private void InitializeUI()
        {
            SuspendLayout();
            BackColor = DesignSystem.Background;
            Padding = new Padding(DesignSystem.PaddingOuter);
            AutoScroll = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // ─── Title ─────────────────────────────────────────────
            var pnlTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(44)
            };
            pnlTitle.Paint += (s, e) =>
            {
                DesignSystem.ApplyHighQualityRendering(e.Graphics);
                DesignSystem.DrawSectionTitle(e.Graphics, "Quản Lý Thành Viên", 0, 0, 40);
            };

            // ─── Toolbar Card ──────────────────────────────────────
            int tbH = DesignSystem.Scale(72);
            pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = tbH
            };
            pnlToolbar.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, pnlToolbar.Width - 1, pnlToolbar.Height - 1);
                DesignSystem.DrawElevatedSurface(e.Graphics, rect, DesignSystem.BorderRadiusLg);
            };

            // All controls positioned manually inside the toolbar for perfect alignment
            int midY = tbH / 2;
            int ctrlH = DesignSystem.InputHeight;
            int y = midY - ctrlH / 2;
            int x = DesignSystem.Scale(16);

            // Search
            txtSearch = new ModernTextBox
            {
                PlaceholderText = "Tìm kiếm...",
                Icon = "🔍",
                ShowClearButton = true,
                Size = new Size(DesignSystem.Scale(230), ctrlH),
                Location = new Point(x, y),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            x += txtSearch.Width + DesignSystem.Scale(10);

            // Filter
            cboFilter = new ModernComboBox
            {
                Size = new Size(DesignSystem.Scale(150), ctrlH),
                Location = new Point(x, y),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            cboFilter.Items.AddRange(new[] { "Tất cả", "Phòng IT", "Phòng Kế toán", "Phòng Nhân sự" });
            cboFilter.SelectedIndex = 0;

            // Right-side buttons (anchored right)
            int btnH = DesignSystem.ButtonHeight;
            int btnY = midY - btnH / 2;

            btnAdd = new RoundedButton
            {
                Text = "Thêm mới",
                ButtonIcon = "＋",
                Variant = ButtonVariant.Primary,
                Size = new Size(DesignSystem.Scale(120), btnH),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnFaceReg = new RoundedButton
            {
                Text = "Đăng ký FaceID",
                ButtonIcon = "📷",
                Variant = ButtonVariant.Outline,
                Size = new Size(DesignSystem.Scale(150), btnH),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnEdit = new RoundedButton
            {
                Text = "Sửa",
                ButtonIcon = "✏",
                Variant = ButtonVariant.Outline,
                Size = new Size(DesignSystem.Scale(80), btnH),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnDelete = new RoundedButton
            {
                Text = "Xóa",
                ButtonIcon = "🗑",
                Variant = ButtonVariant.Danger,
                Size = new Size(DesignSystem.Scale(80), btnH),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            lblTotal = new Label
            {
                Text = "Tổng: 0",
                Font = DesignSystem.Small,
                ForeColor = DesignSystem.TextMuted,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Position right-side buttons from right edge
            int rightX = pnlToolbar.Width - DesignSystem.Scale(16);

            // We'll use Resize event to reposition
            pnlToolbar.Resize += (s, e) =>
            {
                int rx = pnlToolbar.Width - DesignSystem.Scale(16);

                rx -= btnAdd.Width;
                btnAdd.Location = new Point(rx, btnY);

                rx -= btnFaceReg.Width + DesignSystem.Scale(8);
                btnFaceReg.Location = new Point(rx, btnY);

                rx -= btnEdit.Width + DesignSystem.Scale(8);
                btnEdit.Location = new Point(rx, btnY);

                rx -= btnDelete.Width + DesignSystem.Scale(8);
                btnDelete.Location = new Point(rx, btnY);

                rx -= DesignSystem.Scale(12);
                lblTotal.Location = new Point(rx - lblTotal.Width, midY - lblTotal.Height / 2);
            };

            // Click handlers
            btnAdd.Click += (s, e) => MessageBox.Show("Thêm mới nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnEdit.Click += (s, e) =>
            {
                if (dgvEmployees.CurrentRow == null) { MessageBox.Show("Vui lòng chọn nhân viên", "Thông báo"); return; }
                MessageBox.Show("Chỉnh sửa: " + dgvEmployees.CurrentRow.Cells["colName"].Value, "Thông báo");
            };
            btnDelete.Click += (s, e) =>
            {
                if (dgvEmployees.CurrentRow == null) { MessageBox.Show("Vui lòng chọn nhân viên", "Thông báo"); return; }
                if (MessageBox.Show("Xác nhận xóa?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    dgvEmployees.Rows.Remove(dgvEmployees.CurrentRow);
                    UpdateTotalCount();
                }
            };

            pnlToolbar.Controls.AddRange(new Control[]
            {
                txtSearch, cboFilter, btnDelete, btnEdit, btnFaceReg, btnAdd, lblTotal
            });

            // Spacer
            var pnlGap = new Panel { Dock = DockStyle.Top, Height = DesignSystem.Scale(12) };

            // ─── Data Grid ─────────────────────────────────────────
            dgvEmployees = new ModernDataGridView { Dock = DockStyle.Fill };
            dgvEmployees.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "colCode", HeaderText = "Mã NV", Width = DesignSystem.Scale(80) },
                new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Họ tên", Width = DesignSystem.Scale(180) },
                new DataGridViewTextBoxColumn { Name = "colUnit", HeaderText = "Đơn vị", Width = DesignSystem.Scale(140) },
                new DataGridViewTextBoxColumn { Name = "colEmail", HeaderText = "Email", Width = DesignSystem.Scale(200) },
                new DataGridViewTextBoxColumn { Name = "colPhone", HeaderText = "Điện thoại", Width = DesignSystem.Scale(120) },
                new DataGridViewTextBoxColumn { Name = "colFaceID", HeaderText = "FaceID", Width = DesignSystem.Scale(100) },
                new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Trạng thái", Width = DesignSystem.Scale(100) }
            });

            Controls.Add(dgvEmployees);
            Controls.Add(pnlGap);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlTitle);

            txtSearch.TextChanged += (s, e) => FilterGrid();
            cboFilter.SelectedIndexChanged += (s, e) => FilterGrid();

            // Trigger initial layout
            pnlToolbar.PerformLayout();
            ResumeLayout(false);
        }

        private void FilterGrid()
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            string filter = cboFilter.SelectedItem?.ToString() ?? "Tất cả";

            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                if (row.IsNewRow) continue;
                bool matchKeyword = string.IsNullOrEmpty(keyword);
                bool matchFilter = filter == "Tất cả";

                if (!matchKeyword)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString().ToLower().Contains(keyword))
                        { matchKeyword = true; break; }
                    }
                }

                if (!matchFilter)
                    matchFilter = row.Cells["colUnit"].Value?.ToString() == filter;

                row.Visible = matchKeyword && matchFilter;
            }
            UpdateTotalCount();
        }

        private void UpdateTotalCount()
        {
            int visible = 0;
            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                if (!row.IsNewRow && row.Visible) visible++;
            }
            lblTotal.Text = $"Tổng: {visible} thành viên";
        }

        private void LoadSampleData()
        {
            dgvEmployees.Rows.Add("NV001", "Nguyễn Văn An", "Phòng IT", "an.nv@company.com", "0901234567", "Đã đăng ký", "Hoạt động");
            dgvEmployees.Rows.Add("NV002", "Trần Thị Bình", "Phòng Kế toán", "binh.tt@company.com", "0907654321", "Đã đăng ký", "Hoạt động");
            dgvEmployees.Rows.Add("NV003", "Lê Văn Cường", "Phòng Nhân sự", "cuong.lv@company.com", "0912345678", "Chưa đăng ký", "Hoạt động");
            dgvEmployees.Rows.Add("NV004", "Phạm Thị Dung", "Phòng IT", "dung.pt@company.com", "0987654321", "Đã đăng ký", "Tạm nghỉ");
            dgvEmployees.Rows.Add("NV005", "Hoàng Minh Anh", "Phòng Kế toán", "anh.hm@company.com", "0934567890", "Đã đăng ký", "Hoạt động");

            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                var faceIDCell = row.Cells["colFaceID"];
                if (faceIDCell.Value?.ToString() == "Đã đăng ký")
                    faceIDCell.Style.ForeColor = DesignSystem.SuccessDark;
                else
                    faceIDCell.Style.ForeColor = DesignSystem.Danger;

                var statusCell = row.Cells["colStatus"];
                if (statusCell.Value?.ToString() == "Hoạt động")
                    statusCell.Style.ForeColor = DesignSystem.SuccessDark;
                else
                    statusCell.Style.ForeColor = DesignSystem.Warning;
            }

            dgvEmployees.ClearSelection();
            dgvEmployees.CurrentCell = null;
            UpdateTotalCount();
        }
    }
}
