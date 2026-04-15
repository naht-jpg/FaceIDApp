using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.Theme;

namespace FaceIDApp.CustomControls
{
    /// <summary>
    /// TextBox hiện đại — Fluent Design: 8px radius, focus accent bottom bar, placeholder, icon + clear.
    /// </summary>
    public class ModernTextBox : UserControl
    {
        private TextBox _innerTextBox;
        private string _placeholder = "";
        private string _icon = "";
        private bool _showClearButton = false;
        private bool _isFocused = false;
        private bool _hoverClear = false;

        public string PlaceholderText
        {
            get => _placeholder;
            set { _placeholder = value; UpdatePlaceholder(); }
        }

        public string Icon
        {
            get => _icon;
            set { _icon = value; UpdateLayout(); Invalidate(); }
        }

        public bool ShowClearButton
        {
            get => _showClearButton;
            set { _showClearButton = value; Invalidate(); }
        }

        public override string Text
        {
            get => _innerTextBox?.Text ?? "";
            set { if (_innerTextBox != null) _innerTextBox.Text = value; }
        }

        public new event EventHandler TextChanged;

        public ModernTextBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            Cursor = Cursors.IBeam;

            _innerTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = DesignSystem.InputBg,
                Font = DesignSystem.InputFont,
                ForeColor = DesignSystem.TextPrimary
            };
            _innerTextBox.TextChanged += (s, e) =>
            {
                TextChanged?.Invoke(this, e);
                Invalidate();
            };
            _innerTextBox.GotFocus += (s, e) => { _isFocused = true; Invalidate(); };
            _innerTextBox.LostFocus += (s, e) => { _isFocused = false; UpdatePlaceholder(); Invalidate(); };
            _innerTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape && _showClearButton && _innerTextBox.Text.Length > 0)
                {
                    _innerTextBox.Text = "";
                    e.SuppressKeyPress = true;
                }
            };

            Controls.Add(_innerTextBox);
            Size = new Size(DesignSystem.Scale(260), DesignSystem.InputHeight);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (_innerTextBox == null) return;
            int iconW = string.IsNullOrEmpty(_icon) ? 0 : DesignSystem.Scale(28);
            int clearW = DesignSystem.Scale(24);
            int pad = DesignSystem.Scale(10);
            int left = pad + iconW;
            int right = _showClearButton ? clearW + DesignSystem.Scale(4) : pad;
            int top = (Height - _innerTextBox.PreferredHeight) / 2;

            _innerTextBox.Location = new Point(left, top);
            _innerTextBox.Width = Width - left - right;
        }

        private void UpdatePlaceholder()
        {
            if (_innerTextBox == null) return;
            if (!_isFocused && string.IsNullOrEmpty(_innerTextBox.Text) && !string.IsNullOrEmpty(_placeholder))
            {
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_innerTextBox == null) { base.OnPaint(e); return; }
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
            _innerTextBox.BackColor = DesignSystem.InputBg;

            // Border
            var borderColor = _isFocused ? DesignSystem.InputBorderFocus : DesignSystem.InputBorder;
            float borderW = _isFocused ? DesignSystem.Scale(1.5f) : 1f;
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var pen = new Pen(borderColor, borderW))
            {
                g.DrawPath(pen, path);
            }

            // Fluent focus bottom accent bar (2px)
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

            // Focus glow
            if (_isFocused)
            {
                var shadowRect = new Rectangle(-1, -1, Width + 1, Height + 1);
                using (var path = DesignSystem.CreateRoundedRect(shadowRect, radius + 1))
                using (var pen = new Pen(Color.FromArgb(30, DesignSystem.Accent), DesignSystem.Scale(2)))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Icon
            if (!string.IsNullOrEmpty(_icon))
            {
                int iconX = DesignSystem.Scale(8);
                using (var brush = new SolidBrush(_isFocused ? DesignSystem.Accent : DesignSystem.TextMuted))
                {
                    var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(_icon, DesignSystem.Label, brush, iconX, Height / 2f, sf);
                }
            }

            // Placeholder
            if (string.IsNullOrEmpty(_innerTextBox.Text) && !_isFocused && !string.IsNullOrEmpty(_placeholder))
            {
                int iconW = string.IsNullOrEmpty(_icon) ? 0 : DesignSystem.Scale(28);
                int tx = DesignSystem.Scale(10) + iconW;
                using (var brush = new SolidBrush(DesignSystem.TextMuted))
                {
                    var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(_placeholder, DesignSystem.InputFont, brush, tx, Height / 2f, sf);
                }
            }

            // Clear button
            if (_showClearButton && _innerTextBox.Text.Length > 0)
            {
                int btnSize = DesignSystem.Scale(16);
                int bx = Width - DesignSystem.Scale(24);
                int by = (Height - btnSize) / 2;
                var clearColor = _hoverClear ? DesignSystem.TextPrimary : DesignSystem.TextMuted;
                using (var brush = new SolidBrush(clearColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("✕", DesignSystem.Small, brush, new RectangleF(bx, by, btnSize, btnSize), sf);
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Check clear button click
            if (_showClearButton && _innerTextBox.Text.Length > 0)
            {
                int bx = Width - DesignSystem.Scale(28);
                if (e.X >= bx)
                {
                    _innerTextBox.Text = "";
                    _innerTextBox.Focus();
                    return;
                }
            }
            _innerTextBox.Focus();
            base.OnMouseClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_showClearButton && _innerTextBox.Text.Length > 0)
            {
                int bx = Width - DesignSystem.Scale(28);
                bool wasHover = _hoverClear;
                _hoverClear = e.X >= bx;
                Cursor = _hoverClear ? Cursors.Hand : Cursors.IBeam;
                if (wasHover != _hoverClear) Invalidate();
            }
            base.OnMouseMove(e);
        }

        public void FocusInput() => _innerTextBox.Focus();
    }
}
