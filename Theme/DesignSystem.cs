using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace FaceIDApp.Theme
{
    /// <summary>
    /// Hệ thống thiết kế — Windows 11 Fluent Design System
    /// Light + Glass (Mica) | Accent Blue | Soft Shadows | Large Radii
    /// Hỗ trợ DPI Scaling để giao diện sắc nét trên mọi màn hình.
    /// </summary>
    public static class DesignSystem
    {
        // ─── DPI SCALING ────────────────────────────────────────────
        private static float _dpiScale = 1f;
        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized) return;
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                _dpiScale = g.DpiX / 96f;
            }
            _initialized = true;
        }

        public static int Scale(int value) => (int)Math.Round(value * DpiScale);
        public static float Scale(float value) => value * DpiScale;

        public static float DpiScale
        {
            get
            {
                if (!_initialized) Initialize();
                return _dpiScale;
            }
        }

        // ─── COLORS — Fluent Design System ──────────────────────────
        // Core palette — Light theme with app accent blue

        // Primary (used for buttons, emphasis)
        public static readonly Color Primary           = Color.FromArgb(37, 99, 235);    // Blue 600 — app accent
        public static readonly Color PrimaryDark       = Color.FromArgb(29, 78, 216);    // Blue 700 — pressed
        public static readonly Color PrimaryLight      = Color.FromArgb(59, 130, 246);   // Blue 500 — hover

        // Accent
        public static readonly Color Accent            = Color.FromArgb(59, 130, 246);   // #3B82F6 — app accent
        public static readonly Color AccentLight       = Color.FromArgb(96, 165, 250);   // #60A5FA — lighter accent
        public static readonly Color AccentSubtle      = Color.FromArgb(219, 234, 254);  // #DBEAFE — very light accent bg

        // Semantic
        public static readonly Color Success           = Color.FromArgb(16, 185, 129);   // Emerald 500
        public static readonly Color SuccessDark       = Color.FromArgb(5, 150, 105);    // Emerald 600
        public static readonly Color SuccessSubtle     = Color.FromArgb(209, 250, 229);  // Emerald 100
        public static readonly Color Warning           = Color.FromArgb(245, 158, 11);   // Amber 500
        public static readonly Color WarningDark       = Color.FromArgb(217, 119, 6);    // Amber 600
        public static readonly Color WarningSubtle     = Color.FromArgb(254, 243, 199);  // Amber 100
        public static readonly Color Danger            = Color.FromArgb(239, 68, 68);    // Red 500
        public static readonly Color DangerDark        = Color.FromArgb(220, 38, 38);    // Red 600
        public static readonly Color DangerSubtle      = Color.FromArgb(254, 226, 226);  // Red 100

        // Surfaces — Fluent Light
        public static readonly Color Background        = Color.FromArgb(243, 245, 249);  // #F3F5F9 — Mica-like
        public static readonly Color BackgroundSoftTop = Color.FromArgb(250, 251, 253);  // near-white
        public static readonly Color BackgroundSoftBottom = Color.FromArgb(238, 242, 248); // slightly cooler

        public static readonly Color Surface           = Color.FromArgb(255, 255, 255);  // White
        public static readonly Color SurfaceRaised     = Color.FromArgb(255, 255, 255);  // White (elevated)
        public static readonly Color SurfaceAlt        = Color.FromArgb(250, 251, 253);  // #FAFBFD — zebra/alt
        public static readonly Color CardHover         = Color.FromArgb(245, 248, 255);  // Very soft blue tint

        // Borders — Visible definition lines
        public static readonly Color Border            = Color.FromArgb(195, 203, 214);  // #C3CBD6 — clear border
        public static readonly Color BorderLight       = Color.FromArgb(213, 219, 229);  // #D5DBE5 — lighter
        public static readonly Color BorderSubtle      = Color.FromArgb(232, 236, 242);  // #E8ECF2 — zebra lines

        // Text — Dark on light
        public static readonly Color TextPrimary       = Color.FromArgb(26, 26, 46);     // #1A1A2E — near-black
        public static readonly Color TextSecondary     = Color.FromArgb(68, 84, 111);    // #44546F — muted dark
        public static readonly Color TextMuted         = Color.FromArgb(136, 153, 166);  // #8899A6 — placeholder

        // Sidebar — Fluent Light Sidebar
        public static readonly Color SidebarBg          = Color.FromArgb(240, 244, 249);  // #F0F4F9
        public static readonly Color SidebarHover       = Color.FromArgb(232, 237, 245);  // #E8EDF5
        public static readonly Color SidebarActive      = Color.FromArgb(59, 130, 246);   // Accent
        public static readonly Color SidebarText        = Color.FromArgb(68, 84, 111);    // #44546F
        public static readonly Color SidebarTextActive  = Color.FromArgb(59, 130, 246);   // Accent
        public static readonly Color SidebarDivider     = Color.FromArgb(213, 219, 229);  // Same as BorderLight

        // Header — Glass effect
        public static readonly Color HeaderBg           = Color.FromArgb(240, 244, 249);  // Light glass base
        public static readonly Color HeaderGlow         = Color.FromArgb(59, 130, 246);   // Accent glow

        // Gradient (for accent fills)
        public static readonly Color PrimaryGradientTop    = Color.FromArgb(59, 130, 246);
        public static readonly Color PrimaryGradientBottom = Color.FromArgb(37, 99, 235);

        // Input / Card
        public static readonly Color InputBg            = Color.FromArgb(252, 252, 253);  // #FCFCFD
        public static readonly Color InputBorder        = Color.FromArgb(195, 203, 214);  // Match new Border color
        public static readonly Color InputBorderFocus   = Color.FromArgb(59, 130, 246);   // Accent

        // ─── FONTS — Segoe UI (Win11 standard) ─────────────────────
        public static Font TitleScreen  => new Font("Segoe UI", Scale(20f), FontStyle.Bold, GraphicsUnit.Pixel);
        public static Font TitleSection => new Font("Segoe UI Semibold", Scale(15f), FontStyle.Bold, GraphicsUnit.Pixel);
        public static Font Label        => new Font("Segoe UI", Scale(12f), GraphicsUnit.Pixel);
        public static Font Data         => new Font("Segoe UI", Scale(13.5f), GraphicsUnit.Pixel);
        public static Font DataBold     => new Font("Segoe UI Semibold", Scale(13.5f), FontStyle.Bold, GraphicsUnit.Pixel);
        public static Font StatNumber   => new Font("Segoe UI", Scale(32f), FontStyle.Bold, GraphicsUnit.Pixel);
        public static Font StatLabel    => new Font("Segoe UI", Scale(11.5f), GraphicsUnit.Pixel);
        public static Font Sidebar      => new Font("Segoe UI", Scale(13f), GraphicsUnit.Pixel);
        public static Font SidebarBold  => new Font("Segoe UI Semibold", Scale(13f), FontStyle.Bold, GraphicsUnit.Pixel);
        public static Font Small        => new Font("Segoe UI", Scale(11f), GraphicsUnit.Pixel);
        public static Font ButtonFont   => new Font("Segoe UI Semibold", Scale(12.5f), FontStyle.Bold, GraphicsUnit.Pixel);
        public static Font InputFont    => new Font("Segoe UI", Scale(13f), GraphicsUnit.Pixel);
        public static Font Badge        => new Font("Segoe UI Semibold", Scale(10.5f), FontStyle.Bold, GraphicsUnit.Pixel);

        // ─── LAYOUT — Fluent spacing ────────────────────────────────
        public static int PaddingOuter       => Scale(28);
        public static int SectionSpacing     => Scale(20);
        public static int SectionSpacingLg   => Scale(28);
        public static int ControlSpacing     => Scale(10);

        // Border radius scale
        public static int BorderRadius       => Scale(12);       // Default
        public static int BorderRadiusSm     => Scale(8);        // Inputs, small
        public static int BorderRadiusLg     => Scale(16);       // Cards, panels
        public static int BorderRadiusXl     => Scale(20);       // Large containers
        public static int ButtonBorderRadius => Scale(10);       // Buttons
        public static int InputBorderRadius  => Scale(8);        // Inputs

        // Sizing
        public static int SidebarWidth       => Scale(260);
        public static int HeaderHeight       => Scale(52);
        public static int InputHeight        => Scale(38);
        public static int ButtonHeight       => Scale(38);
        public static int CardPadding        => Scale(18);
        public static int StatusBarHeight    => Scale(28);
        public static int ToolbarHeight      => Scale(68);

        // ─── RENDERING HELPERS ─────────────────────────────────────

        public static void ApplyHighQualityRendering(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
        }

        // ─── GRADIENT HELPERS ──────────────────────────────────────

        public static LinearGradientBrush CreateVerticalGradient(Rectangle rect, Color top, Color bottom)
        {
            if (rect.Height <= 0) rect = new Rectangle(rect.X, rect.Y, rect.Width, 1);
            return new LinearGradientBrush(rect, top, bottom, LinearGradientMode.Vertical);
        }

        public static LinearGradientBrush CreateHorizontalGradient(Rectangle rect, Color left, Color right)
        {
            if (rect.Width <= 0) rect = new Rectangle(rect.X, rect.Y, 1, rect.Height);
            return new LinearGradientBrush(rect, left, right, LinearGradientMode.Horizontal);
        }

        // ─── COLOR HELPERS ─────────────────────────────────────────

        /// <summary>Lerp giữa 2 màu theo t (0→1)</summary>
        public static Color LerpColor(Color from, Color to, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return Color.FromArgb(
                (int)(from.A + (to.A - from.A) * t),
                (int)(from.R + (to.R - from.R) * t),
                (int)(from.G + (to.G - from.G) * t),
                (int)(from.B + (to.B - from.B) * t));
        }

        public static float Clamp01(float t) => Math.Max(0f, Math.Min(1f, t));

        public static float EaseOutCubic(float t)
        {
            t = Clamp01(t);
            float inv = 1f - t;
            return 1f - inv * inv * inv;
        }

        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), color.R, color.G, color.B);
        }

        // ─── FLUENT SHADOW — Soft multi-layer shadows ──────────────

        /// <summary>Vẽ Fluent shadow mềm (nhiều layer, alpha thấp, offset nhẹ)</summary>
        public static void DrawFluentShadow(Graphics g, Rectangle bounds, int radius, bool elevated = false)
        {
            int layers = elevated ? 6 : 5;
            int alphaBase = elevated ? 10 : 6;
            int offsetBase = elevated ? 3 : 2;

            for (int i = layers; i >= 1; i--)
            {
                int expand = i * offsetBase;
                var shadowRect = new Rectangle(
                    bounds.X - expand / 2,
                    bounds.Y + i * offsetBase,
                    bounds.Width + expand,
                    bounds.Height + expand / 2);

                using (var path = CreateRoundedRect(shadowRect, radius + i))
                using (var brush = new SolidBrush(Color.FromArgb(alphaBase * i, 15, 23, 42)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        /// <summary>Vẽ card shadow cũ (backward compat)</summary>
        public static void DrawCardShadow(Graphics g, Rectangle bounds, int radius)
        {
            DrawFluentShadow(g, bounds, radius, false);
        }

        // ─── FLUENT SURFACES ───────────────────────────────────────

        /// <summary>Vẽ Mica-like background nhẹ với gradient mềm</summary>
        public static void DrawSoftBackground(Graphics g, Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var bg = CreateVerticalGradient(rect, BackgroundSoftTop, BackgroundSoftBottom))
                g.FillRectangle(bg, rect);

            // Subtle accent glow (top-right)
            int glowSize = Math.Max(Scale(200), rect.Width / 4);
            var glowRect = new Rectangle(rect.Right - glowSize - Scale(50), rect.Y - Scale(60), glowSize, glowSize);
            using (var glowPath = new GraphicsPath())
            {
                glowPath.AddEllipse(glowRect);
                using (var pgb = new PathGradientBrush(glowPath))
                {
                    pgb.CenterColor = Color.FromArgb(12, AccentLight);
                    pgb.SurroundColors = new[] { Color.FromArgb(0, AccentLight) };
                    g.FillEllipse(pgb, glowRect);
                }
            }
        }

        /// <summary>Vẽ elevated surface (card, panel) — Fluent style with clear border</summary>
        public static void DrawElevatedSurface(Graphics g, Rectangle bounds, int radius, bool hovered = false)
        {
            DrawFluentShadow(g, bounds, radius, hovered);

            using (var surfacePath = CreateRoundedRect(bounds, radius))
            using (var surfaceBrush = new SolidBrush(Surface))
            {
                g.FillPath(surfaceBrush, surfacePath);
            }

            // Clear visible border
            var borderCol = hovered ? WithAlpha(Accent, 70) : Border;
            using (var surfacePath = CreateRoundedRect(bounds, radius))
            using (var borderPen = new Pen(borderCol, Scale(1.2f)))
            {
                g.DrawPath(borderPen, surfacePath);
            }
        }

        /// <summary>Vẽ glass panel — semi-transparent with border</summary>
        public static void DrawGlassPanel(Graphics g, Rectangle bounds, int radius)
        {
            // Glass fill
            using (var path = CreateRoundedRect(bounds, radius))
            using (var fill = CreateVerticalGradient(bounds,
                Color.FromArgb(200, 255, 255, 255),
                Color.FromArgb(160, 248, 250, 253)))
            {
                g.FillPath(fill, path);
            }

            // Top highlight
            using (var topPen = new Pen(Color.FromArgb(120, 255, 255, 255), 1f))
            {
                g.DrawLine(topPen, bounds.X + radius, bounds.Y, bounds.Right - radius, bounds.Y);
            }

            // Border
            using (var path = CreateRoundedRect(bounds, radius))
            using (var border = new Pen(Color.FromArgb(50, Border), 1f))
            {
                g.DrawPath(border, path);
            }
        }

        // ─── DGV STYLE — Fluent Light Table ────────────────────────

        public static void ApplyDataGridViewStyle(DataGridView dgv)
        {
            dgv.BackgroundColor = Surface;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Border;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;

            // Fluent light header — visible tinted background
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 237, 248);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            dgv.ColumnHeadersDefaultCellStyle.Font = DataBold;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(Scale(10), 0, 0, 0);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = Scale(48);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Row styles
            dgv.DefaultCellStyle.Font = Data;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(212, 228, 255); // #D4E4FF
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
            dgv.DefaultCellStyle.Padding = new Padding(Scale(10), 0, 0, 0);
            dgv.RowTemplate.Height = Scale(48);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = SurfaceAlt;
        }

        public static void ApplyFormDefaults(Form form)
        {
            form.BackColor = Background;
            form.Font = Data;
            form.ForeColor = TextPrimary;
        }

        // ─── SHAPE HELPERS ─────────────────────────────────────────

        public static GraphicsPath CreateRoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>Vẽ section title với accent underline</summary>
        public static void DrawSectionTitle(Graphics g, string text, int x, int y, int accentWidth = 40)
        {
            using (var brush = new SolidBrush(TextPrimary))
                g.DrawString(text, TitleSection, brush, x, y);

            int lineY = y + Scale(24);
            using (var pen = new Pen(Accent, Scale(2.5f)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, x, lineY, x + Scale(accentWidth), lineY);
            }
        }

        /// <summary>Vẽ avatar placeholder tròn (chữ cái đầu)</summary>
        public static void DrawAvatarCircle(Graphics g, Rectangle bounds, string name, Color bgColor)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(bounds);
                using (var brush = new SolidBrush(bgColor))
                    g.FillPath(brush, path);
            }

            string initial = string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpper();
            using (var brush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var font = new Font("Segoe UI Semibold", bounds.Height * 0.4f, FontStyle.Bold, GraphicsUnit.Pixel);
                g.DrawString(initial, font, brush, bounds, sf);
                font.Dispose();
            }
        }
    }
}
