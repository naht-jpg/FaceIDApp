using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.Theme;

namespace FaceIDApp.CustomControls
{
    /// <summary>
    /// DateTimePicker hiện đại — Fluent Design: 8px radius, focus accent bar, calendar icon.
    /// </summary>
    public class ModernDatePicker : UserControl
    {
        private DateTimePicker _innerPicker;
        private bool _isFocused = false;

        public DateTime Value
        {
            get => _innerPicker.Value;
            set => _innerPicker.Value = value;
        }

        public DateTimePickerFormat Format
        {
            get => _innerPicker.Format;
            set => _innerPicker.Format = value;
        }

        public ModernDatePicker()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            _innerPicker = new DateTimePicker
            {
                Font = DesignSystem.InputFont,
                Format = DateTimePickerFormat.Short
            };
            _innerPicker.GotFocus += (s, e) => { _isFocused = true; Invalidate(); };
            _innerPicker.LostFocus += (s, e) => { _isFocused = false; Invalidate(); };

            Controls.Add(_innerPicker);
            Size = new Size(DesignSystem.Scale(150), DesignSystem.InputHeight);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (_innerPicker == null) return;
            int iconW = DesignSystem.Scale(26);
            int pad = DesignSystem.Scale(4);
            int top = (Height - _innerPicker.PreferredHeight) / 2;
            _innerPicker.Location = new Point(iconW + pad, Math.Max(1, top));
            _innerPicker.Width = Width - iconW - pad * 2;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_innerPicker == null) { base.OnPaint(e); return; }
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            int radius = DesignSystem.InputBorderRadius;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Background
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var brush = new SolidBrush(DesignSystem.InputBg))
            {
                g.FillPath(brush, path);
            }

            // Border
            var borderColor = _isFocused ? DesignSystem.InputBorderFocus : DesignSystem.InputBorder;
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var pen = new Pen(borderColor, _isFocused ? DesignSystem.Scale(1.5f) : 1f))
            {
                g.DrawPath(pen, path);
            }

            // Fluent focus bottom accent bar
            if (_isFocused)
            {
                int barH = DesignSystem.Scale(2);
                int barMargin = radius;
                using (var pen = new Pen(DesignSystem.Accent, barH))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, barMargin, rect.Bottom, rect.Right - barMargin, rect.Bottom);
                }
            }

            // Calendar icon
            int iconX = DesignSystem.Scale(6);
            using (var brush = new SolidBrush(_isFocused ? DesignSystem.Accent : DesignSystem.TextMuted))
            {
                var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString("📅", DesignSystem.Small, brush, iconX, Height / 2f, sf);
            }
        }
    }
}
