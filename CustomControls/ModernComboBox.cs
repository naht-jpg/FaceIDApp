using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.Theme;

namespace FaceIDApp.CustomControls
{
    /// <summary>
    /// ComboBox hiện đại — Fluent Design: 8px radius, focus accent bar, chevron arrow.
    /// </summary>
    public class ModernComboBox : UserControl
    {
        private ComboBox _innerCombo;
        private bool _isFocused = false;

        public ComboBox.ObjectCollection Items => _innerCombo.Items;
        public int SelectedIndex
        {
            get => _innerCombo.SelectedIndex;
            set => _innerCombo.SelectedIndex = value;
        }
        public object SelectedItem => _innerCombo.SelectedItem;

        public event EventHandler SelectedIndexChanged;

        public ModernComboBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            _innerCombo = new ComboBox
            {
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = DesignSystem.InputFont,
                BackColor = DesignSystem.InputBg,
                ForeColor = DesignSystem.TextPrimary
            };
            _innerCombo.SelectedIndexChanged += (s, e) => SelectedIndexChanged?.Invoke(this, e);
            _innerCombo.GotFocus += (s, e) => { _isFocused = true; Invalidate(); };
            _innerCombo.LostFocus += (s, e) => { _isFocused = false; Invalidate(); };

            Controls.Add(_innerCombo);
            Size = new Size(DesignSystem.Scale(180), DesignSystem.InputHeight);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (_innerCombo == null) return;
            int pad = DesignSystem.Scale(8);
            int top = (Height - _innerCombo.PreferredHeight) / 2;
            _innerCombo.Location = new Point(pad, Math.Max(1, top));
            _innerCombo.Width = Width - pad * 2 - DesignSystem.Scale(16);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_innerCombo == null) { base.OnPaint(e); return; }
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
            float borderW = _isFocused ? DesignSystem.Scale(1.5f) : 1f;
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var pen = new Pen(borderColor, borderW))
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

            // Chevron arrow (▾)
            int arrowSize = DesignSystem.Scale(5);
            int ax = Width - DesignSystem.Scale(16);
            int ay = Height / 2 - arrowSize / 3;
            using (var pen = new Pen(_isFocused ? DesignSystem.Accent : DesignSystem.TextMuted, DesignSystem.Scale(1.5f)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                g.DrawLines(pen, new[] {
                    new PointF(ax - arrowSize, ay),
                    new PointF(ax, ay + arrowSize),
                    new PointF(ax + arrowSize, ay)
                });
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            _innerCombo.Focus();
            _innerCombo.DroppedDown = true;
            base.OnMouseClick(e);
        }
    }
}
