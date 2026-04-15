using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.Theme;

namespace FaceIDApp.CustomControls
{
    /// <summary>
    /// Menu item cho sidebar — Fluent Design light theme, hover animation mượt, accent highlight.
    /// </summary>
    public class SidebarMenuItem : Control
    {
        private bool _isActive;
        private bool _isHovered;
        private string _icon = "📊";
        private float _hoverAlpha = 0f;
        private Timer _animTimer;

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; Invalidate(); }
        }

        public string Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        public bool IsCollapsed { get; set; }

        public event EventHandler ItemClicked;

        public SidebarMenuItem()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Size = new Size(DesignSystem.SidebarWidth, DesignSystem.Scale(46));
            Cursor = Cursors.Hand;

            _animTimer = new Timer { Interval = 16 };
            _animTimer.Tick += AnimTimer_Tick;
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            float target = _isHovered ? 1f : 0f;
            float speed = _isHovered ? 0.14f : 0.16f;

            _hoverAlpha += (_hoverAlpha < target) ? speed : -speed;
            _hoverAlpha = Math.Max(0, Math.Min(1, _hoverAlpha));

            if (Math.Abs(_hoverAlpha - target) < 0.02f)
            {
                _hoverAlpha = target;
                _animTimer.Stop();
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);
            float hoverVisual = DesignSystem.EaseOutCubic(_hoverAlpha);

            int margin = DesignSystem.Scale(8);
            var itemRect = new Rectangle(margin, DesignSystem.Scale(3), Width - margin * 2, Height - DesignSystem.Scale(6));
            int radius = DesignSystem.ButtonBorderRadius;

            Color fg;
            if (_isActive)
            {
                fg = DesignSystem.SidebarTextActive;

                // Active background — subtle accent fill
                using (var path = DesignSystem.CreateRoundedRect(itemRect, radius))
                using (var brush = new SolidBrush(Color.FromArgb(18, DesignSystem.Accent)))
                    g.FillPath(brush, path);

                // Subtle border
                using (var path = DesignSystem.CreateRoundedRect(itemRect, radius))
                using (var pen = new Pen(Color.FromArgb(35, DesignSystem.Accent), 1f))
                    g.DrawPath(pen, path);
            }
            else
            {
                fg = DesignSystem.LerpColor(DesignSystem.SidebarText, DesignSystem.TextPrimary, hoverVisual * 0.6f);
                if (hoverVisual > 0.01f)
                {
                    using (var path = DesignSystem.CreateRoundedRect(itemRect, radius))
                    using (var brush = new SolidBrush(Color.FromArgb((int)(255 * hoverVisual * 0.45f), DesignSystem.SidebarHover)))
                        g.FillPath(brush, path);
                }
            }

            // Active accent bar (left, rounded)
            if (_isActive)
            {
                int barW = DesignSystem.Scale(3);
                int barMargin = DesignSystem.Scale(10);
                int barH = Height - barMargin * 2;
                var barRect = new Rectangle(DesignSystem.Scale(2), barMargin, barW, barH);
                using (var path = DesignSystem.CreateRoundedRect(barRect, barW))
                using (var brush = new SolidBrush(DesignSystem.Accent))
                    g.FillPath(brush, path);
            }

            // Icon
            int iconX = DesignSystem.Scale(22);
            int iconSize = DesignSystem.Scale(18);
            if (IsCollapsed) iconX = (Width - iconSize) / 2;

            Color iconColor = _isActive ? DesignSystem.Accent : fg;
            using (var iconFont = new Font("Segoe UI", iconSize, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(iconColor))
            {
                var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(_icon, iconFont, brush, iconX, Height / 2f, sf);
            }

            // Text (hidden when collapsed)
            if (!IsCollapsed)
            {
                int textX = DesignSystem.Scale(52);
                using (var brush = new SolidBrush(fg))
                {
                    var font = _isActive ? DesignSystem.SidebarBold : DesignSystem.Sidebar;
                    var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(Text, font, brush, textX, Height / 2f, sf);
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            _animTimer.Start();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _animTimer.Start();
            base.OnMouseLeave(e);
        }

        protected override void OnClick(EventArgs e)
        {
            ItemClicked?.Invoke(this, e);
            base.OnClick(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _animTimer?.Stop(); _animTimer?.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
