using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.CustomControls;
using FaceIDApp.Theme;

namespace FaceIDApp.UserControls
{
    public partial class UCSettings : UserControl
    {
        private RoundedButton btnSave;
        private ModernTextBox txtWorkStart, txtWorkEnd, txtLateThreshold;
        private bool _cameraAutoStart = true;
        private bool _notifyEnabled = true;

        public UCSettings()
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
                Height = DesignSystem.Scale(44)
            };
            pnlTitle.Paint += (s, e) =>
            {
                DesignSystem.ApplyHighQualityRendering(e.Graphics);
                DesignSystem.DrawSectionTitle(e.Graphics, "Cài Đặt Hệ Thống", 0, 0, 40);
            };

            // ─── Card 1: Thời gian làm việc ───────────────────────
            int card1H = DesignSystem.Scale(210);
            var card1 = new Panel
            {
                Dock = DockStyle.Top,
                Height = card1H
            };
            card1.Paint += (s, e) => PaintSettingsCard(e.Graphics, card1, "Thời Gian Làm Việc");

            int labelX = DesignSystem.Scale(20);
            int inputX = DesignSystem.Scale(190);
            int fieldStart = DesignSystem.Scale(56);
            int lineH = DesignSystem.Scale(48);

            AddField(card1, "Giờ bắt đầu:", labelX, fieldStart, out txtWorkStart, inputX, fieldStart - DesignSystem.Scale(4), "08:00", DesignSystem.Scale(120));
            AddField(card1, "Giờ kết thúc:", labelX, fieldStart + lineH, out txtWorkEnd, inputX, fieldStart + lineH - DesignSystem.Scale(4), "17:30", DesignSystem.Scale(120));
            AddField(card1, "Ngưỡng đi trễ (phút):", labelX, fieldStart + lineH * 2, out txtLateThreshold, inputX, fieldStart + lineH * 2 - DesignSystem.Scale(4), "15", DesignSystem.Scale(80));

            var gap1 = new Panel { Dock = DockStyle.Top, Height = DesignSystem.Scale(12), BackColor = Color.Transparent };

            // ─── Card 2: Camera & Thông Báo ───────────────────────
            int card2H = DesignSystem.Scale(160);
            var card2 = new Panel
            {
                Dock = DockStyle.Top,
                Height = card2H
            };
            card2.Paint += (s, e) => PaintSettingsCard(e.Graphics, card2, "Camera & Thông Báo");

            int toggleX = inputX;
            AddFieldLabel(card2, "Tự động mở camera:", labelX, fieldStart + DesignSystem.Scale(4));
            CreateToggle(card2, toggleX, fieldStart, _cameraAutoStart, (val) => _cameraAutoStart = val);

            AddFieldLabel(card2, "Bật thông báo:", labelX, fieldStart + lineH + DesignSystem.Scale(4));
            CreateToggle(card2, toggleX, fieldStart + lineH, _notifyEnabled, (val) => _notifyEnabled = val);

            var gap2 = new Panel { Dock = DockStyle.Top, Height = DesignSystem.Scale(12), BackColor = Color.Transparent };

            // ─── Card 3: Nhận Diện Khuôn Mặt ─────────────────────
            int card3H = DesignSystem.Scale(110);
            var card3 = new Panel
            {
                Dock = DockStyle.Top,
                Height = card3H
            };
            card3.Paint += (s, e) => PaintSettingsCard(e.Graphics, card3, "Nhận Diện Khuôn Mặt");
            AddFieldLabel(card3, "Độ tin cậy tối thiểu: 75%", labelX, fieldStart);

            var gap3 = new Panel { Dock = DockStyle.Top, Height = DesignSystem.Scale(16), BackColor = Color.Transparent };

            // ─── Save Button ───────────────────────────────────────
            var pnlSave = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(50)
            };
            btnSave = new RoundedButton
            {
                Text = "Lưu cài đặt",
                ButtonIcon = "💾",
                Variant = ButtonVariant.Primary,
                Size = new Size(DesignSystem.Scale(160), DesignSystem.ButtonHeight),
                Location = new Point(0, 0)
            };
            btnSave.Click += (s, e) =>
            {
                MessageBox.Show("Đã lưu cài đặt thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            pnlSave.Controls.Add(btnSave);

            // Build order
            Controls.Add(pnlSave);
            Controls.Add(gap3);
            Controls.Add(card3);
            Controls.Add(gap2);
            Controls.Add(card2);
            Controls.Add(gap1);
            Controls.Add(card1);
            Controls.Add(pnlTitle);
            ResumeLayout(false);
        }

        private void PaintSettingsCard(Graphics g, Panel card, string title)
        {
            DesignSystem.ApplyHighQualityRendering(g);
            var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
            DesignSystem.DrawElevatedSurface(g, rect, DesignSystem.BorderRadiusLg);

            // Title
            using (var brush = new SolidBrush(DesignSystem.TextPrimary))
                g.DrawString(title, DesignSystem.TitleSection, brush, DesignSystem.Scale(20), DesignSystem.Scale(16));

            // Divider
            int divY = DesignSystem.Scale(44);
            using (var pen = new Pen(DesignSystem.BorderLight, 1f))
                g.DrawLine(pen, DesignSystem.Scale(20), divY, card.Width - DesignSystem.Scale(20), divY);
        }

        private void AddField(Panel card, string label, int lx, int ly,
            out ModernTextBox textBox, int tx, int ty, string defaultValue, int width)
        {
            AddFieldLabel(card, label, lx, ly + DesignSystem.Scale(6));
            textBox = new ModernTextBox
            {
                Text = defaultValue,
                Size = new Size(width, DesignSystem.InputHeight),
                Location = new Point(tx, ty)
            };
            card.Controls.Add(textBox);
        }

        private void AddFieldLabel(Panel card, string text, int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                Font = DesignSystem.Data,
                ForeColor = DesignSystem.TextSecondary,
                Location = new Point(x, y),
                AutoSize = true
            };
            card.Controls.Add(lbl);
        }

        private void CreateToggle(Panel parent, int x, int y, bool initialValue, Action<bool> onToggle)
        {
            int toggleW = DesignSystem.Scale(46);
            int toggleH = DesignSystem.Scale(24);
            float _animT = initialValue ? 1f : 0f;
            bool value = initialValue;

            var toggle = new Panel
            {
                Size = new Size(toggleW, toggleH),
                Location = new Point(x, y),
                Cursor = Cursors.Hand
            };

            Timer animTimer = new Timer { Interval = 16 };
            animTimer.Tick += (s, e) =>
            {
                float target = value ? 1f : 0f;
                _animT += (target - _animT) * 0.22f;
                if (Math.Abs(_animT - target) < 0.01f)
                {
                    _animT = target;
                    animTimer.Stop();
                }
                toggle.Invalidate();
            };

            toggle.Paint += (s, e) =>
            {
                var g = e.Graphics;
                DesignSystem.ApplyHighQualityRendering(g);

                var rect = new Rectangle(0, 0, toggleW - 1, toggleH - 1);
                int radius = toggleH / 2;

                var trackColor = DesignSystem.LerpColor(DesignSystem.Border, DesignSystem.Accent, _animT);
                using (var path = DesignSystem.CreateRoundedRect(rect, radius))
                using (var brush = new SolidBrush(trackColor))
                    g.FillPath(brush, path);

                int thumbR = toggleH - DesignSystem.Scale(6);
                int thumbMargin = DesignSystem.Scale(3);
                int thumbTravel = toggleW - thumbR - thumbMargin * 2;
                int thumbX = thumbMargin + (int)(thumbTravel * _animT);

                using (var shadow = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                    g.FillEllipse(shadow, thumbX, thumbMargin + 1, thumbR, thumbR);
                using (var brush = new SolidBrush(Color.White))
                    g.FillEllipse(brush, thumbX, thumbMargin, thumbR, thumbR);
            };

            toggle.Click += (s, e) =>
            {
                value = !value;
                onToggle?.Invoke(value);
                animTimer.Start();
            };

            toggle.Disposed += (s, e) => { animTimer?.Stop(); animTimer?.Dispose(); };
            parent.Controls.Add(toggle);
        }
    }
}
