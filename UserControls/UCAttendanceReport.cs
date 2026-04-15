using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.CustomControls;
using FaceIDApp.Theme;

namespace FaceIDApp.UserControls
{
    public partial class UCAttendanceReport : UserControl
    {
        private ModernDataGridView dgvReport;
        private ModernDatePicker dtpFrom, dtpTo;
        private ModernComboBox cboUnit;
        private RoundedButton btnSearch, btnExport;
        private Panel pnlFilter;
        private Panel pnlMiniCards;

        public UCAttendanceReport()
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
                DesignSystem.DrawSectionTitle(e.Graphics, "Báo Cáo Chấm Công", 0, 0, 40);
            };

            // ─── Filter Bar (manual positioning) ───────────────────
            int tbH = DesignSystem.Scale(72);
            pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = tbH
            };
            pnlFilter.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, pnlFilter.Width - 1, pnlFilter.Height - 1);
                DesignSystem.DrawElevatedSurface(e.Graphics, rect, DesignSystem.BorderRadiusLg);
            };

            int midY = tbH / 2;
            int ctrlH = DesignSystem.InputHeight;
            int y = midY - ctrlH / 2;
            int x = DesignSystem.Scale(16);

            // Label + DatePicker From
            var lblFrom = new Label
            {
                Text = "Từ ngày:",
                Font = DesignSystem.Label,
                ForeColor = DesignSystem.TextSecondary,
                AutoSize = true
            };
            var lblFromSize = TextRenderer.MeasureText(lblFrom.Text, lblFrom.Font);
            lblFrom.Location = new Point(x, midY - lblFromSize.Height / 2);
            x += lblFromSize.Width + DesignSystem.Scale(4);

            dtpFrom = new ModernDatePicker
            {
                Size = new Size(DesignSystem.Scale(140), ctrlH),
                Location = new Point(x, y)
            };
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            x += dtpFrom.Width + DesignSystem.Scale(12);

            // Label + DatePicker To
            var lblTo = new Label
            {
                Text = "Đến:",
                Font = DesignSystem.Label,
                ForeColor = DesignSystem.TextSecondary,
                AutoSize = true
            };
            var lblToSize = TextRenderer.MeasureText(lblTo.Text, lblTo.Font);
            lblTo.Location = new Point(x, midY - lblToSize.Height / 2);
            x += lblToSize.Width + DesignSystem.Scale(4);

            dtpTo = new ModernDatePicker
            {
                Size = new Size(DesignSystem.Scale(140), ctrlH),
                Location = new Point(x, y)
            };
            x += dtpTo.Width + DesignSystem.Scale(12);

            // Unit combo
            cboUnit = new ModernComboBox
            {
                Size = new Size(DesignSystem.Scale(150), ctrlH),
                Location = new Point(x, y)
            };
            cboUnit.Items.AddRange(new[] { "Tất cả đơn vị", "Phòng IT", "Phòng Kế toán", "Phòng Nhân sự" });
            cboUnit.SelectedIndex = 0;
            x += cboUnit.Width + DesignSystem.Scale(10);

            // Search button
            int btnH = DesignSystem.ButtonHeight;
            int btnY = midY - btnH / 2;
            btnSearch = new RoundedButton
            {
                Text = "Tìm kiếm",
                ButtonIcon = "🔍",
                Variant = ButtonVariant.Primary,
                Size = new Size(DesignSystem.Scale(120), btnH),
                Location = new Point(x, btnY)
            };

            // Export button (anchored right)
            btnExport = new RoundedButton
            {
                Text = "Xuất Excel",
                ButtonIcon = "📥",
                Variant = ButtonVariant.Outline,
                Size = new Size(DesignSystem.Scale(125), btnH),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            pnlFilter.Resize += (s, e) =>
            {
                btnExport.Location = new Point(pnlFilter.Width - DesignSystem.Scale(16) - btnExport.Width, btnY);
            };

            pnlFilter.Controls.AddRange(new Control[] {
                lblFrom, dtpFrom, lblTo, dtpTo, cboUnit, btnSearch, btnExport
            });

            // ─── Mini Stat Cards ───────────────────────────────────
            pnlMiniCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(72),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, DesignSystem.Scale(10), 0, DesignSystem.Scale(6))
            };

            CreateMiniCard("Có mặt", "156", DesignSystem.Success);
            CreateMiniCard("Vắng", "12", DesignSystem.Danger);
            CreateMiniCard("Đi trễ", "23", DesignSystem.Warning);
            CreateMiniCard("Tổng", "191", DesignSystem.Accent);

            // ─── Table ─────────────────────────────────────────────
            dgvReport = new ModernDataGridView { Dock = DockStyle.Fill };
            dgvReport.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "colDate", HeaderText = "Ngày", Width = DesignSystem.Scale(100) },
                new DataGridViewTextBoxColumn { Name = "colCode", HeaderText = "Mã NV", Width = DesignSystem.Scale(80) },
                new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Họ tên", Width = DesignSystem.Scale(160) },
                new DataGridViewTextBoxColumn { Name = "colUnit", HeaderText = "Đơn vị", Width = DesignSystem.Scale(130) },
                new DataGridViewTextBoxColumn { Name = "colCheckIn", HeaderText = "Giờ vào", Width = DesignSystem.Scale(80) },
                new DataGridViewTextBoxColumn { Name = "colCheckOut", HeaderText = "Giờ ra", Width = DesignSystem.Scale(80) },
                new DataGridViewTextBoxColumn { Name = "colHours", HeaderText = "Giờ làm", Width = DesignSystem.Scale(80) },
                new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Trạng thái", Width = DesignSystem.Scale(100) }
            });

            Controls.Add(dgvReport);
            Controls.Add(pnlMiniCards);
            Controls.Add(pnlFilter);
            Controls.Add(pnlTitle);
            ResumeLayout(false);
        }

        private void CreateMiniCard(string label, string value, Color accent)
        {
            int cardWidth = DesignSystem.Scale(150);
            int cardHeight = DesignSystem.Scale(52);
            int radius = DesignSystem.BorderRadius;

            var pnl = new Panel
            {
                Size = new Size(cardWidth, cardHeight),
                Margin = new Padding(0, 0, DesignSystem.Scale(10), 0),
                BackColor = Color.Transparent
            };

            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics;
                DesignSystem.ApplyHighQualityRendering(g);
                var rect = new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1);
                DesignSystem.DrawElevatedSurface(g, rect, radius);

                // Left accent bar
                int barW = DesignSystem.Scale(3);
                var barRect = new Rectangle(DesignSystem.Scale(1), DesignSystem.Scale(8), barW, pnl.Height - DesignSystem.Scale(16));
                using (var path = DesignSystem.CreateRoundedRect(barRect, barW))
                using (var brush = new SolidBrush(accent))
                    g.FillPath(brush, path);

                int padX = DesignSystem.Scale(14);
                using (var valBrush = new SolidBrush(DesignSystem.TextPrimary))
                    g.DrawString(value, DesignSystem.TitleSection, valBrush, padX, DesignSystem.Scale(4));

                using (var labelBrush = new SolidBrush(DesignSystem.TextSecondary))
                    g.DrawString(label, DesignSystem.Small, labelBrush, padX, DesignSystem.Scale(28));
            };

            pnlMiniCards.Controls.Add(pnl);
        }

        private void LoadSampleData()
        {
            dgvReport.Rows.Add("15/04/2026", "NV001", "Nguyễn Văn An", "Phòng IT", "08:05", "17:30", "8h25'", "Đúng giờ");
            dgvReport.Rows.Add("15/04/2026", "NV002", "Trần Thị Bình", "Phòng Kế toán", "08:12", "17:45", "8h33'", "Đi trễ");
            dgvReport.Rows.Add("15/04/2026", "NV003", "Lê Văn Cường", "Phòng Nhân sự", "08:00", "17:30", "8h30'", "Đúng giờ");
            dgvReport.Rows.Add("14/04/2026", "NV001", "Nguyễn Văn An", "Phòng IT", "07:58", "17:35", "8h37'", "Đúng giờ");
            dgvReport.Rows.Add("14/04/2026", "NV004", "Phạm Thị Dung", "Phòng IT", "—", "—", "0h", "Vắng mặt");

            foreach (DataGridViewRow row in dgvReport.Rows)
            {
                var status = row.Cells["colStatus"].Value?.ToString();
                if (status == "Đi trễ")
                    row.Cells["colStatus"].Style.ForeColor = DesignSystem.Warning;
                else if (status == "Vắng mặt")
                    row.Cells["colStatus"].Style.ForeColor = DesignSystem.Danger;
                else
                    row.Cells["colStatus"].Style.ForeColor = DesignSystem.SuccessDark;
            }

            dgvReport.ClearSelection();
            dgvReport.CurrentCell = null;
        }
    }
}
