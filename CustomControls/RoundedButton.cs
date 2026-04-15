using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.Theme;

namespace FaceIDApp.CustomControls
{
    public enum ButtonVariant { Primary, Outline, Danger, Success }

    /// <summary>
    /// Button bo góc — Fluent Design: 10px radius, hover/press animation, focus ring, disabled state.
    /// </summary>
    public class RoundedButton : Control
    {
        private ButtonVariant _variant = ButtonVariant.Primary;
        private bool _isHovered;
        private bool _isPressed;
        private float _hoverT = 0f;
        private Timer _animTimer;
        private string _buttonIcon = "";

        public ButtonVariant Variant
        {
            get => _variant;
            set { _variant = value; Invalidate(); }
        }

        public string ButtonIcon
        {
            get => _buttonIcon;
            set { _buttonIcon = value; Invalidate(); }
        }

        public RoundedButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.Selectable, true);
            Size = new Size(DesignSystem.Scale(120), DesignSystem.ButtonHeight);
            Cursor = Cursors.Hand;

            _animTimer = new Timer { Interval = 16 };
            _animTimer.Tick += (s, e) =>
            {
                float target = _isHovered ? 1f : 0f;
                float speed = 0.14f;
                _hoverT += (_hoverT < target) ? speed : -speed;
                _hoverT = Math.Max(0, Math.Min(1, _hoverT));
                if (Math.Abs(_hoverT - target) < 0.02f)
                {
                    _hoverT = target;
                    _animTimer.Stop();
                }
                Invalidate();
            };
        }

        private void GetColors(out Color bg, out Color fg, out Color border)
        {
            float hoverVisual = DesignSystem.EaseOutCubic(_hoverT);

            if (!Enabled)
            {
                bg = Color.FromArgb(240, 242, 245);
                fg = DesignSystem.TextMuted;
                border = DesignSystem.BorderLight;
                return;
            }

            switch (_variant)
            {
                case ButtonVariant.Danger:
                    bg = DesignSystem.LerpColor(DesignSystem.Danger, DesignSystem.DangerDark, _isPressed ? 0.5f : 0f);
                    bg = DesignSystem.LerpColor(bg, Color.FromArgb(248, 100, 100), hoverVisual * 0.4f);
                    fg = Color.White;
                    border = bg;
                    break;
                case ButtonVariant.Success:
                    bg = DesignSystem.LerpColor(DesignSystem.Success, DesignSystem.SuccessDark, _isPressed ? 0.5f : 0f);
                    bg = DesignSystem.LerpColor(bg, Color.FromArgb(74, 222, 128), hoverVisual * 0.4f);
                    fg = Color.White;
                    border = bg;
                    break;
                case ButtonVariant.Outline:
                    bg = DesignSystem.LerpColor(Color.White, DesignSystem.CardHover, hoverVisual);
                    if (_isPressed) bg = DesignSystem.BorderLight; // Use standard darker color when pressed
                    fg = DesignSystem.TextPrimary;
                    // Stronger border line for Outline
                    border = DesignSystem.LerpColor(DesignSystem.Border, DesignSystem.Accent, hoverVisual);
                    break;
                default: // Primary
                    bg = DesignSystem.LerpColor(DesignSystem.Primary, DesignSystem.PrimaryLight, hoverVisual * 0.5f);
                    if (_isPressed) bg = DesignSystem.PrimaryDark;
                    fg = Color.White;
                    border = bg;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            GetColors(out var bg, out var fg, out var borderColor);

            int radius = DesignSystem.ButtonBorderRadius;
            int shrink = _isPressed && Enabled ? 1 : 0;
            var rect = new Rectangle(1 + shrink, 1 + shrink, Width - 3 - shrink * 2, Height - 3 - shrink * 2);

            // Subtle shadow for filled buttons
            if (Enabled && _variant != ButtonVariant.Outline)
            {
                var shadowRect = new Rectangle(rect.X, rect.Y + 1, rect.Width, rect.Height);
                using (var path = DesignSystem.CreateRoundedRect(shadowRect, radius))
                using (var brush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                    g.FillPath(brush, path);
            }

            // Focus ring
            if (Focused && Enabled)
            {
                var focusRect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (var path = DesignSystem.CreateRoundedRect(focusRect, radius + 2))
                using (var pen = new Pen(Color.FromArgb(60, DesignSystem.Accent), DesignSystem.Scale(2)))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Button body
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var brush = new SolidBrush(bg))
            {
                g.FillPath(brush, path);
            }

            // Button border (drawn inside the bounds so it's fully visible)
            if (_variant == ButtonVariant.Outline)
            {
                var borderRect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
                using (var path = DesignSystem.CreateRoundedRect(borderRect, radius))
                using (var pen = new Pen(borderColor, DesignSystem.Scale(1.5f)))
                {
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                using (var path = DesignSystem.CreateRoundedRect(rect, radius))
                using (var pen = new Pen(borderColor, 1f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Top highlight for filled variants (Fluent glass effect)
            if (Enabled && _variant != ButtonVariant.Outline)
            {
                int highlightH = rect.Height / 2;
                var highlightRect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, highlightH);
                using (var path = DesignSystem.CreateRoundedRect(rect, radius))
                using (var brush = DesignSystem.CreateVerticalGradient(highlightRect,
                    Color.FromArgb(30, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255)))
                {
                    var oldClip = g.Clip;
                    g.SetClip(path);
                    g.FillRectangle(brush, highlightRect);
                    g.Clip = oldClip;
                }
            }

            // Text + icon
            string displayText = Text;
            if (!string.IsNullOrEmpty(_buttonIcon))
                displayText = _buttonIcon + "  " + displayText;

            TextRenderer.DrawText(g, displayText, DesignSystem.ButtonFont, rect, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!Enabled) return;
            _isHovered = true;
            Cursor = Cursors.Hand;
            _animTimer.Start();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            _animTimer.Start();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Enabled) return;
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }

        protected override bool IsInputKey(Keys key) => key == Keys.Enter || key == Keys.Space || base.IsInputKey(key);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space) && Enabled)
            {
                _isPressed = true;
                Invalidate();
                OnClick(EventArgs.Empty);
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnKeyUp(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _animTimer?.Stop(); _animTimer?.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
