using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.CustomControls;
using FaceIDApp.Theme;

namespace FaceIDApp.UserControls
{
    public partial class UCDashboard : UserControl
    {
        private StatCard cardPresent, cardAbsent, cardLate, cardTotal;
        private Panel pnlChart;
        private ModernDataGridView dgvActivity;
        private FlowLayoutPanel _pnlCards;
        private List<ChartData> chartData;
        private float _barAnimProgress = 0f;
        private Timer _barAnimTimer;

        public UCDashboard()
        {
            InitializeUI();
            LoadSampleData();
            AnimateBars();
        }

        private void InitializeUI()
        {
            SuspendLayout();
            BackColor = DesignSystem.Background;
            Padding = new Padding(DesignSystem.PaddingOuter);
            AutoScroll = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // ─── Title + Date (single compact row) ─────────────────
            var pnlTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(44),
                Padding = new Padding(0, 0, 0, DesignSystem.Scale(8))
            };
            pnlTitle.Paint += (s, e) =>
            {
                var g = e.Graphics;
                DesignSystem.ApplyHighQualityRendering(g);
                DesignSystem.DrawSectionTitle(g, "Dashboard Tổng Quan", 0, 0, 48);

                // Date on right
                string dateStr = DateTime.Now.ToString("dddd, dd/MM/yyyy");
                using (var brush = new SolidBrush(DesignSystem.TextMuted))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                    g.DrawString(dateStr, DesignSystem.Label, brush,
                        new RectangleF(0, 0, pnlTitle.Width, DesignSystem.Scale(24)), sf);
                }
            };

            // ─── Stat Cards (FlowLayout) ────────────────────────────
            _pnlCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(115),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Padding = new Padding(0, 0, 0, DesignSystem.Scale(10))
            };

            int cardMargin = DesignSystem.Scale(10);
            cardPresent = new StatCard { Icon = "✓", Value = "42", StatLabel = "Có mặt hôm nay", Type = StatType.Success, Trend = TrendType.Up, Margin = new Padding(0, 0, cardMargin, 0) };
            cardAbsent  = new StatCard { Icon = "✗", Value = "3",  StatLabel = "Vắng mặt",       Type = StatType.Danger,  Trend = TrendType.Down, Margin = new Padding(0, 0, cardMargin, 0) };
            cardLate    = new StatCard { Icon = "⏱", Value = "5",  StatLabel = "Đi trễ",          Type = StatType.Warning, Trend = TrendType.Neutral, Margin = new Padding(0, 0, cardMargin, 0) };
            cardTotal   = new StatCard { Icon = "👤", Value = "50", StatLabel = "Tổng thành viên", Type = StatType.Primary, Margin = new Padding(0) };
            _pnlCards.Controls.AddRange(new Control[] { cardPresent, cardAbsent, cardLate, cardTotal });

            // ─── Chart Section ──────────────────────────────────────
            var pnlChartTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(36)
            };
            pnlChartTitle.Paint += (s, e) =>
            {
                DesignSystem.ApplyHighQualityRendering(e.Graphics);
                DesignSystem.DrawSectionTitle(e.Graphics, "Biểu đồ chấm công 7 ngày", 0, DesignSystem.Scale(4), 46);
            };

            var pnlChartWrap = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(220)
            };
            pnlChartWrap.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, pnlChartWrap.Width - 1, pnlChartWrap.Height - 1);
                DesignSystem.DrawElevatedSurface(e.Graphics, rect, DesignSystem.BorderRadiusLg);
            };

            pnlChart = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(DesignSystem.Scale(8))
            };
            pnlChart.Paint += PnlChart_Paint;
            pnlChartWrap.Controls.Add(pnlChart);

            // ─── Activity Table ─────────────────────────────────────
            var pnlActTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(40),
                Padding = new Padding(0, DesignSystem.Scale(10), 0, 0)
            };
            pnlActTitle.Paint += (s, e) =>
            {
                DesignSystem.ApplyHighQualityRendering(e.Graphics);
                DesignSystem.DrawSectionTitle(e.Graphics, "Hoạt động gần đây", 0, DesignSystem.Scale(8), 36);
            };

            dgvActivity = new ModernDataGridView
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(280)
            };
            dgvActivity.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "colTime", HeaderText = "Thời gian", Width = DesignSystem.Scale(100) },
                new DataGridViewTextBoxColumn { Name = "colCode", HeaderText = "Mã", Width = DesignSystem.Scale(80) },
                new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Họ tên", Width = DesignSystem.Scale(160) },
                new DataGridViewTextBoxColumn { Name = "colUnit", HeaderText = "Đơn vị", Width = DesignSystem.Scale(140) },
                new DataGridViewTextBoxColumn { Name = "colAction", HeaderText = "Loại", Width = DesignSystem.Scale(80) },
                new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Trạng thái", Width = DesignSystem.Scale(100) }
            });

            // Build order (bottom-up)
            Controls.Add(dgvActivity);
            Controls.Add(pnlActTitle);
            Controls.Add(pnlChartWrap);
            Controls.Add(pnlChartTitle);
            Controls.Add(_pnlCards);
            Controls.Add(pnlTitle);

            Resize += (s, e) => ReflowCards();
            ReflowCards();
            ResumeLayout(false);
        }

        private void ReflowCards()
        {
            if (_pnlCards == null) return;
            int avail = Math.Max(DesignSystem.Scale(500), _pnlCards.ClientSize.Width);
            int gap = DesignSystem.Scale(10);
            int cardW = (avail - gap * 3) / 4;
            int cardH = DesignSystem.Scale(100);

            cardPresent.Size = new Size(cardW, cardH);
            cardAbsent.Size = new Size(cardW, cardH);
            cardLate.Size = new Size(cardW, cardH);
            cardTotal.Size = new Size(cardW, cardH);
        }

        // ─── Chart ────────────────────────────────────────────────

        private struct ChartData
        {
            public string Day; public int Present, Late, Absent;
        }

        private void AnimateBars()
        {
            _barAnimProgress = 0f;
            _barAnimTimer = new Timer { Interval = 16 };
            _barAnimTimer.Tick += (s, e) =>
            {
                _barAnimProgress += 0.04f;
                if (_barAnimProgress >= 1f)
                {
                    _barAnimProgress = 1f;
                    _barAnimTimer.Stop();
                    _barAnimTimer.Dispose();
                    _barAnimTimer = null;
                }
                pnlChart?.Invalidate();
            };
            _barAnimTimer.Start();
        }

        private void PnlChart_Paint(object sender, PaintEventArgs e)
        {
            if (chartData == null || chartData.Count == 0) return;
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            int leftM = DesignSystem.Scale(44), rightM = DesignSystem.Scale(24);
            int topM = DesignSystem.Scale(16), bottomM = DesignSystem.Scale(40);
            var area = new Rectangle(leftM, topM, pnlChart.Width - leftM - rightM, pnlChart.Height - topM - bottomM);
            if (area.Width <= 0 || area.Height <= 0) return;

            int barGroupW = area.Width / chartData.Count;
            int barW = Math.Min(barGroupW / 4, DesignSystem.Scale(18));

            int maxVal = 10;
            foreach (var d in chartData)
                maxVal = Math.Max(maxVal, Math.Max(d.Present, Math.Max(d.Late, d.Absent)));
            maxVal = (int)Math.Ceiling(maxVal / 10.0) * 10 + 5;

            // Grid lines
            using (var gridPen = new Pen(DesignSystem.BorderSubtle, 1))
            using (var brush = new SolidBrush(DesignSystem.TextMuted))
            {
                for (int i = 0; i <= 4; i++)
                {
                    int y = area.Bottom - (int)(area.Height * (i / 4.0));
                    g.DrawLine(gridPen, area.Left, y, area.Right, y);
                    int labelVal = (int)Math.Round(maxVal * i / 4.0);
                    g.DrawString(labelVal.ToString(), DesignSystem.Small, brush, DesignSystem.Scale(4), y - DesignSystem.Scale(7));
                }
            }

            float anim = _barAnimProgress;
            for (int i = 0; i < chartData.Count; i++)
            {
                var d = chartData[i];
                int cx = area.Left + i * barGroupW + barGroupW / 2;

                DrawBar(g, cx - barW - 2, area, (int)(d.Present * anim), maxVal, barW, DesignSystem.Success);
                DrawBar(g, cx, area, (int)(d.Late * anim), maxVal, barW, DesignSystem.Warning);
                DrawBar(g, cx + barW + 2, area, (int)(d.Absent * anim), maxVal, barW, DesignSystem.Danger);

                using (var brush = new SolidBrush(DesignSystem.TextSecondary))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString(d.Day, DesignSystem.Small, brush, cx, area.Bottom + DesignSystem.Scale(6), sf);
                }
            }

            // Legend
            int ly = area.Top - DesignSystem.Scale(4);
            DrawLegendDot(g, area.Right - DesignSystem.Scale(220), ly, DesignSystem.Success, "Có mặt");
            DrawLegendDot(g, area.Right - DesignSystem.Scale(140), ly, DesignSystem.Warning, "Đi trễ");
            DrawLegendDot(g, area.Right - DesignSystem.Scale(60), ly, DesignSystem.Danger, "Vắng");
        }

        private void DrawBar(Graphics g, int x, Rectangle area, int value, int maxVal, int width, Color color)
        {
            int barH = (int)(area.Height * ((float)value / maxVal));
            if (barH < DesignSystem.Scale(3)) barH = DesignSystem.Scale(3);
            int y = area.Bottom - barH;
            var rect = new Rectangle(x - width / 2, y, width, barH);

            if (rect.Height > 0 && rect.Width > 0)
            {
                int barRadius = DesignSystem.Scale(4);
                using (var brush = DesignSystem.CreateVerticalGradient(rect, color, Color.FromArgb(180, color)))
                using (var path = DesignSystem.CreateRoundedRect(rect, barRadius))
                {
                    g.FillPath(brush, path);
                }

                // Value label
                if (_barAnimProgress >= 1f && value > 0)
                {
                    using (var brush = new SolidBrush(DesignSystem.TextSecondary))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center };
                        g.DrawString(value.ToString(), DesignSystem.Small, brush, x, y - DesignSystem.Scale(14), sf);
                    }
                }
            }
        }

        private void DrawLegendDot(Graphics g, int x, int y, Color color, string label)
        {
            int dotSize = DesignSystem.Scale(8);
            using (var brush = new SolidBrush(color))
                g.FillEllipse(brush, x, y, dotSize, dotSize);
            using (var brush = new SolidBrush(DesignSystem.TextSecondary))
                g.DrawString(label, DesignSystem.Small, brush, x + dotSize + DesignSystem.Scale(4), y - DesignSystem.Scale(2));
        }

        // ─── Data ─────────────────────────────────────────────────

        private void LoadSampleData()
        {
            chartData = new List<ChartData>
            {
                new ChartData { Day = "T2", Present = 45, Late = 3, Absent = 2 },
                new ChartData { Day = "T3", Present = 42, Late = 5, Absent = 3 },
                new ChartData { Day = "T4", Present = 47, Late = 2, Absent = 1 },
                new ChartData { Day = "T5", Present = 44, Late = 4, Absent = 2 },
                new ChartData { Day = "T6", Present = 40, Late = 6, Absent = 4 },
                new ChartData { Day = "T7", Present = 38, Late = 3, Absent = 9 },
                new ChartData { Day = "CN", Present = 10, Late = 1, Absent = 39 }
            };

            dgvActivity.Rows.Add("08:05:23", "NV001", "Nguyễn Văn An", "Phòng IT", "Vào", "Đúng giờ");
            dgvActivity.Rows.Add("08:12:45", "NV002", "Trần Thị Bình", "Phòng Kế toán", "Vào", "Đi trễ");
            dgvActivity.Rows.Add("08:00:12", "NV003", "Lê Văn Cường", "Phòng Nhân sự", "Vào", "Đúng giờ");
            dgvActivity.Rows.Add("07:58:33", "HS001", "Phạm Minh Đức", "Lớp 12A1", "Vào", "Đúng giờ");
            dgvActivity.Rows.Add("17:30:15", "NV001", "Nguyễn Văn An", "Phòng IT", "Ra", "Đúng giờ");

            foreach (DataGridViewRow row in dgvActivity.Rows)
            {
                if (row.Cells["colStatus"].Value?.ToString() == "Đi trễ")
                    row.Cells["colStatus"].Style.ForeColor = DesignSystem.Warning;
                else
                    row.Cells["colStatus"].Style.ForeColor = DesignSystem.SuccessDark;
            }

            dgvActivity.ClearSelection();
            dgvActivity.CurrentCell = null;
        }

        // ─── Public API ────────────────────────────────────────────

        public void UpdateStats(int present, int absent, int late, int total)
        {
            cardPresent.Value = present.ToString();
            cardAbsent.Value = absent.ToString();
            cardLate.Value = late.ToString();
            cardTotal.Value = total.ToString();
        }
    }
}
