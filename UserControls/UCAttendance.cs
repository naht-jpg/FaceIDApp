using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.CustomControls;
using FaceIDApp.Theme;

namespace FaceIDApp.UserControls
{
    public partial class UCAttendance : UserControl
    {
        private PictureBox picCamera;
        private Panel pnlCameraCard;
        private Panel pnlInfo;
        private ModernDataGridView dgvLog;
        private RoundedButton btnStart, btnStop;
        private StatusBadge badgeStatus;
        private Label lblName, lblCode, lblUnit, lblTime;
        private Timer _glowTimer;
        private float _glowPhase = 0f;
        private bool _cameraActive = false;

        public UCAttendance()
        {
            InitializeUI();
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
                Height = DesignSystem.Scale(50)
            };
            pnlTitle.Paint += (s, e) =>
            {
                DesignSystem.ApplyHighQualityRendering(e.Graphics);
                DesignSystem.DrawSectionTitle(e.Graphics, "Chấm Công Nhận Diện Khuôn Mặt", 0, 0, 55);
            };

            // ─── Top area: Camera + Info side by side ──────────────
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(380),
                Padding = new Padding(0)
            };

            // Camera card container (with glow effect border)
            int camWidth = DesignSystem.Scale(480);
            pnlCameraCard = new Panel
            {
                Dock = DockStyle.Left,
                Width = camWidth,
                Padding = new Padding(DesignSystem.Scale(3))
            };
            pnlCameraCard.Paint += PnlCameraCard_Paint;

            picCamera = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(20, 20, 30)
            };
            picCamera.Paint += PicCamera_Paint;
            pnlCameraCard.Controls.Add(picCamera);

            // Glow animation timer
            _glowTimer = new Timer { Interval = 30 };
            _glowTimer.Tick += (s, e) =>
            {
                _glowPhase += 0.06f;
                if (_glowPhase > 2 * Math.PI) _glowPhase -= (float)(2 * Math.PI);
                pnlCameraCard.Invalidate();
            };

            // ─── Info Panel ────────────────────────────────────────
            pnlInfo = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(DesignSystem.SectionSpacing, 0, 0, 0)
            };

            var pnlInfoCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(DesignSystem.CardPadding)
            };
            pnlInfoCard.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, pnlInfoCard.Width - 1, pnlInfoCard.Height - 1);
                DesignSystem.DrawElevatedSurface(e.Graphics, rect, DesignSystem.BorderRadiusLg);
            };

            // Recognition status badge
            badgeStatus = new StatusBadge
            {
                Text = "Chờ nhận diện",
                Type = BadgeType.Neutral,
                ShowDot = true,
                Size = new Size(DesignSystem.Scale(140), DesignSystem.Scale(26)),
                Location = new Point(DesignSystem.CardPadding, DesignSystem.CardPadding)
            };

            // Info fields
            int fieldTop = DesignSystem.Scale(60);
            int lineH = DesignSystem.Scale(50);
            int labelX = DesignSystem.CardPadding;
            int valueX = DesignSystem.Scale(110);
            string[] labels = { "Họ tên:", "Mã NV:", "Đơn vị:", "Giờ vào:" };
            Label[] values = new Label[4];

            for (int i = 0; i < labels.Length; i++)
            {
                var lbl = new Label
                {
                    Text = labels[i],
                    Font = DesignSystem.Label,
                    ForeColor = DesignSystem.TextSecondary,
                    Location = new Point(labelX, fieldTop + i * lineH),
                    AutoSize = true
                };
                pnlInfoCard.Controls.Add(lbl);

                values[i] = new Label
                {
                    Text = "—",
                    Font = DesignSystem.DataBold,
                    ForeColor = DesignSystem.TextPrimary,
                    Location = new Point(valueX, fieldTop + i * lineH),
                    AutoSize = true
                };
                pnlInfoCard.Controls.Add(values[i]);
            }
            lblName = values[0]; lblCode = values[1]; lblUnit = values[2]; lblTime = values[3];

            pnlInfoCard.Controls.Add(badgeStatus);

            // Buttons
            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.LeftToRight,
                Height = DesignSystem.Scale(52),
                Padding = new Padding(DesignSystem.CardPadding, DesignSystem.Scale(6), 0, 0),
                BackColor = Color.Transparent
            };

            btnStart = new RoundedButton
            {
                Text = "Bắt đầu",
                ButtonIcon = "▶",
                Variant = ButtonVariant.Primary,
                Size = new Size(DesignSystem.Scale(140), DesignSystem.ButtonHeight),
                Margin = new Padding(0, 0, DesignSystem.Scale(10), 0)
            };
            btnStart.Click += (s, e) => StartCamera();

            btnStop = new RoundedButton
            {
                Text = "Dừng",
                ButtonIcon = "⏹",
                Variant = ButtonVariant.Outline,
                Size = new Size(DesignSystem.Scale(110), DesignSystem.ButtonHeight),
                Enabled = false
            };
            btnStop.Click += (s, e) => StopCamera();

            pnlButtons.Controls.AddRange(new Control[] { btnStart, btnStop });
            pnlInfoCard.Controls.Add(pnlButtons);
            pnlInfo.Controls.Add(pnlInfoCard);

            pnlTop.Controls.Add(pnlInfo);
            pnlTop.Controls.Add(pnlCameraCard);

            // ─── Log Table ─────────────────────────────────────────
            var pnlLogTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(52),
                Padding = new Padding(0, DesignSystem.SectionSpacing, 0, DesignSystem.Scale(4))
            };
            pnlLogTitle.Paint += (s, e) =>
            {
                DesignSystem.ApplyHighQualityRendering(e.Graphics);
                DesignSystem.DrawSectionTitle(e.Graphics, "Lịch Sử Chấm Công Hôm Nay", 0, DesignSystem.Scale(14), 44);
            };

            dgvLog = new ModernDataGridView { Dock = DockStyle.Fill };
            dgvLog.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "colTime", HeaderText = "Thời gian", Width = DesignSystem.Scale(100) },
                new DataGridViewTextBoxColumn { Name = "colCode", HeaderText = "Mã", Width = DesignSystem.Scale(80) },
                new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Họ tên", Width = DesignSystem.Scale(160) },
                new DataGridViewTextBoxColumn { Name = "colUnit", HeaderText = "Đơn vị", Width = DesignSystem.Scale(120) },
                new DataGridViewTextBoxColumn { Name = "colConf", HeaderText = "Độ tin cậy", Width = DesignSystem.Scale(90) },
                new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Trạng thái", Width = DesignSystem.Scale(100) }
            });

            // Build order
            Controls.Add(dgvLog);
            Controls.Add(pnlLogTitle);
            Controls.Add(pnlTop);
            Controls.Add(pnlTitle);

            Disposed += (s, e) =>
            {
                _glowTimer?.Stop();
                _glowTimer?.Dispose();
            };
            ResumeLayout(false);
        }

        // ─── Camera Card Border + Glow Effect ──────────────────────

        private void PnlCameraCard_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            int radius = DesignSystem.BorderRadiusLg;
            var rect = new Rectangle(0, 0, pnlCameraCard.Width - 1, pnlCameraCard.Height - 1);

            DesignSystem.DrawFluentShadow(g, rect, radius);

            // Background 
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var bg = new SolidBrush(Color.FromArgb(20, 20, 30)))
                g.FillPath(bg, path);

            // Accent glow border when camera active
            if (_cameraActive)
            {
                float glowIntensity = (float)(Math.Sin(_glowPhase) * 0.3 + 0.5);
                int alpha = (int)(60 * glowIntensity);
                for (int i = 3; i >= 1; i--)
                {
                    var glowRect = new Rectangle(rect.X - i, rect.Y - i, rect.Width + i * 2, rect.Height + i * 2);
                    using (var pen = new Pen(Color.FromArgb(alpha / i, DesignSystem.Accent), 1.5f))
                    using (var path = DesignSystem.CreateRoundedRect(glowRect, radius + i))
                        g.DrawPath(pen, path);
                }
            }

            // Border
            Color borderColor = _cameraActive ? Color.FromArgb(90, DesignSystem.Accent) : Color.FromArgb(40, DesignSystem.Border);
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var pen = new Pen(borderColor, 1.5f))
                g.DrawPath(pen, path);
        }

        // ─── Camera Feed Paint ─────────────────────────────────────

        private void PicCamera_Paint(object sender, PaintEventArgs e)
        {
            if (picCamera.Image != null) return;
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            g.Clear(Color.FromArgb(20, 20, 30));

            // Center icon
            using (var brush = new SolidBrush(Color.FromArgb(80, DesignSystem.TextMuted)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var iconFont = new Font("Segoe UI", DesignSystem.Scale(48), GraphicsUnit.Pixel);
                g.DrawString("📷", iconFont, brush, picCamera.ClientRectangle, sf);
                iconFont.Dispose();
            }

            using (var brush = new SolidBrush(Color.FromArgb(100, DesignSystem.TextMuted)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var textRect = new RectangleF(0, picCamera.Height / 2f + DesignSystem.Scale(30), picCamera.Width, DesignSystem.Scale(28));
                g.DrawString("Nhấn [Bắt đầu] để mở camera", DesignSystem.Data, brush, textRect, sf);
            }

            // Crosshair
            int cx = picCamera.Width / 2;
            int cy = picCamera.Height / 2;
            using (var pen = new Pen(Color.FromArgb(25, DesignSystem.Accent), 1f))
            {
                g.DrawLine(pen, cx - DesignSystem.Scale(50), cy, cx + DesignSystem.Scale(50), cy);
                g.DrawLine(pen, cx, cy - DesignSystem.Scale(50), cx, cy + DesignSystem.Scale(50));
            }
        }

        // ─── Camera Control ─────────────────────────────────────────

        private void StartCamera()
        {
            _cameraActive = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            badgeStatus.SetStatus("Đang nhận diện...", BadgeType.Info);
            _glowTimer.Start();
            pnlCameraCard.Invalidate();
        }

        private void StopCamera()
        {
            _cameraActive = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            badgeStatus.SetStatus("Đã dừng", BadgeType.Neutral);
            _glowTimer.Stop();
            pnlCameraCard.Invalidate();
        }

        // [API] Gọi khi nhận diện thành công
        public void ShowRecognitionSuccess(string name, string code, string unit, DateTime time, float confidence)
        {
            lblName.Text = name;
            lblCode.Text = code;
            lblUnit.Text = unit;
            lblTime.Text = time.ToString("HH:mm:ss");
            badgeStatus.SetStatus($"✓ {confidence:P0} tin cậy", BadgeType.Success);

            dgvLog.Rows.Insert(0, time.ToString("HH:mm:ss"), code, name, unit, $"{confidence:P0}", "Thành công");
            dgvLog.Rows[0].DefaultCellStyle.BackColor = Color.FromArgb(10, DesignSystem.Success);
        }
    }
}
