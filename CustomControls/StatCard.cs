using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.Theme;

namespace FaceIDApp.CustomControls
{
    public enum StatType { Primary, Success, Warning, Danger }
    public enum TrendType { Neutral, Up, Down }

    /// <summary>
    /// Card thống kê — Fluent: 16px radius, soft shadow, icon circle, accent bar, hover elevation.
    /// </summary>
    public class StatCard : Control
    {
        private string _icon = "👤";
        private string _value = "0";
        private string _label = "Label";
        private StatType _statType = StatType.Primary;
        private TrendType _trend = TrendType.Neutral;
        private bool _isHovered;

        public string Icon { get => _icon; set { _icon = value; Invalidate(); } }
        public string Value { get => _value; set { _value = value; Invalidate(); } }
        public string StatLabel { get => _label; set { _label = value; Invalidate(); } }
        public StatType Type { get => _statType; set { _statType = value; Invalidate(); } }
        public TrendType Trend { get => _trend; set { _trend = value; Invalidate(); } }

        public StatCard()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Size = new Size(DesignSystem.Scale(220), DesignSystem.Scale(105));
        }

        private Color GetAccentColor()
        {
            switch (_statType)
            {
                case StatType.Success: return DesignSystem.Success;
                case StatType.Warning: return DesignSystem.Warning;
                case StatType.Danger:  return DesignSystem.Danger;
                default:               return DesignSystem.Accent;
            }
        }

        private Color GetAccentSubtle()
        {
            switch (_statType)
            {
                case StatType.Success: return DesignSystem.SuccessSubtle;
                case StatType.Warning: return DesignSystem.WarningSubtle;
                case StatType.Danger:  return DesignSystem.DangerSubtle;
                default:               return DesignSystem.AccentSubtle;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            var accent = GetAccentColor();
            int radius = DesignSystem.BorderRadiusLg;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Fluent shadow
            DesignSystem.DrawFluentShadow(g, rect, radius, _isHovered);

            // Card background
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var bg = new SolidBrush(DesignSystem.Surface))
                g.FillPath(bg, path);

            // Hover — subtle accent border
            var borderColor = _isHovered ? Color.FromArgb(50, accent) : DesignSystem.BorderLight;
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var pen = new Pen(borderColor, 1f))
                g.DrawPath(pen, path);

            // Left accent bar (rounded pill)
            int barW = DesignSystem.Scale(4);
            int barMargin = DesignSystem.Scale(14);
            var barRect = new Rectangle(DesignSystem.Scale(1), barMargin, barW, Height - barMargin * 2);
            using (var path = DesignSystem.CreateRoundedRect(barRect, barW))
            using (var brush = new SolidBrush(accent))
                g.FillPath(brush, path);

            // Icon with subtle background circle
            int iconAreaSize = DesignSystem.Scale(40);
            int iconX = DesignSystem.Scale(18);
            int iconY = DesignSystem.Scale(14);
            var iconBgRect = new Rectangle(iconX, iconY, iconAreaSize, iconAreaSize);
            using (var brush = new SolidBrush(Color.FromArgb(20, accent)))
                g.FillEllipse(brush, iconBgRect);

            int iconSz = DesignSystem.Scale(20);
            using (var iconFont = new Font("Segoe UI", iconSz, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(accent))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(_icon, iconFont, brush, iconBgRect, sf);
            }

            // Value — moved down to prevent clipping at top
            int valueX = DesignSystem.Scale(68);
            using (var brush = new SolidBrush(DesignSystem.TextPrimary))
            {
                g.DrawString(_value, DesignSystem.StatNumber, brush, valueX, DesignSystem.Scale(16));
            }

            // Trend indicator
            if (_trend != TrendType.Neutral)
            {
                var trendColor = _trend == TrendType.Up ? DesignSystem.Success : DesignSystem.Danger;
                string arrow = _trend == TrendType.Up ? "▲" : "▼";
                var valSize = g.MeasureString(_value, DesignSystem.StatNumber);
                int trendX = valueX + (int)valSize.Width + DesignSystem.Scale(4);
                int trendY = DesignSystem.Scale(30); // moved down
                using (var brush = new SolidBrush(trendColor))
                {
                    g.DrawString(arrow, DesignSystem.Small, brush, trendX, trendY);
                }
            }

            // Label
            using (var brush = new SolidBrush(DesignSystem.TextSecondary))
            {
                g.DrawString(_label, DesignSystem.StatLabel, brush, valueX + DesignSystem.Scale(2), Height - DesignSystem.Scale(26));
            }
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }
    }
}
