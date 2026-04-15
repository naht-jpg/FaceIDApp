using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.Theme;

namespace FaceIDApp.CustomControls
{
    public enum BadgeType { Success, Warning, Danger, Info, Neutral }

    /// <summary>
    /// Badge trạng thái — Fluent pill shape, dot indicator, pulse animation.
    /// </summary>
    public class StatusBadge : Control
    {
        private BadgeType _badgeType = BadgeType.Neutral;
        private bool _showDot = true;
        private float _pulseAlpha = 1f;
        private float _pulseDir = -0.04f;
        private Timer _pulseTimer;

        public BadgeType Type
        {
            get => _badgeType;
            set
            {
                _badgeType = value;
                UpdatePulse();
                Invalidate();
            }
        }

        public bool ShowDot
        {
            get => _showDot;
            set { _showDot = value; Invalidate(); }
        }

        public StatusBadge()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Size = new Size(DesignSystem.Scale(80), DesignSystem.Scale(26));

            _pulseTimer = new Timer { Interval = 50 };
            _pulseTimer.Tick += (s, e) =>
            {
                _pulseAlpha += _pulseDir;
                if (_pulseAlpha <= 0.3f) { _pulseAlpha = 0.3f; _pulseDir = 0.04f; }
                if (_pulseAlpha >= 1f) { _pulseAlpha = 1f; _pulseDir = -0.04f; }
                Invalidate();
            };
        }

        private void UpdatePulse()
        {
            if (_badgeType == BadgeType.Success || _badgeType == BadgeType.Info)
                _pulseTimer.Start();
            else
                _pulseTimer.Stop();
        }

        private void GetColors(out Color bg, out Color fg)
        {
            switch (_badgeType)
            {
                case BadgeType.Success:
                    bg = Color.FromArgb(25, DesignSystem.Success);
                    fg = DesignSystem.SuccessDark;
                    break;
                case BadgeType.Warning:
                    bg = Color.FromArgb(25, DesignSystem.Warning);
                    fg = DesignSystem.WarningDark;
                    break;
                case BadgeType.Danger:
                    bg = Color.FromArgb(25, DesignSystem.Danger);
                    fg = DesignSystem.DangerDark;
                    break;
                case BadgeType.Info:
                    bg = Color.FromArgb(25, DesignSystem.Accent);
                    fg = DesignSystem.Primary;
                    break;
                default:
                    bg = Color.FromArgb(25, DesignSystem.TextSecondary);
                    fg = DesignSystem.TextSecondary;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            GetColors(out var bg, out var fg);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = DesignSystem.CreateRoundedRect(rect, Height / 2))
            using (var brush = new SolidBrush(bg))
            {
                g.FillPath(brush, path);
            }

            int textOffsetX = 0;

            // Dot indicator
            if (_showDot)
            {
                int dotSize = DesignSystem.Scale(6);
                int dotX = DesignSystem.Scale(10);
                int dotY = (Height - dotSize) / 2;
                int dotAlpha = (_badgeType == BadgeType.Success || _badgeType == BadgeType.Info)
                    ? (int)(255 * _pulseAlpha) : 255;
                using (var brush = new SolidBrush(Color.FromArgb(dotAlpha, fg)))
                {
                    g.FillEllipse(brush, dotX, dotY, dotSize, dotSize);
                }
                textOffsetX = dotSize + DesignSystem.Scale(4);
            }

            var textRect = new Rectangle(textOffsetX, 0, Width - textOffsetX, Height);
            TextRenderer.DrawText(g, Text, DesignSystem.Badge, textRect, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        public void SetStatus(string text, BadgeType type)
        {
            Text = text;
            _badgeType = type;
            UpdatePulse();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _pulseTimer?.Stop(); _pulseTimer?.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
