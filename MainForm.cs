using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.CustomControls;
using FaceIDApp.Theme;
using FaceIDApp.UserControls;

namespace FaceIDApp
{
    public partial class MainForm : Form
    {
        // ─── User Controls ─────────────────────────────────────────
        private UCDashboard ucDashboard;
        private UCAttendance ucAttendance;
        private UCEmployeeManagement ucEmployeeManagement;
        private UCAttendanceReport ucAttendanceReport;
        private UCSettings ucSettings;

        // ─── UI Components ─────────────────────────────────────────
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Label lblOrgName;
        private Label lblClock;
        private Label lblUser;
        private Label lblLogo;
        private Label btnCollapse;
        private StatusBadge badgeSystemStatus;

        private SidebarMenuItem mnuDashboard;
        private SidebarMenuItem mnuAttendance;
        private SidebarMenuItem mnuEmployees;
        private SidebarMenuItem mnuReports;
        private SidebarMenuItem mnuSettings;
        private SidebarMenuItem currentMenuItem;

        private Timer clockTimer;
        private bool _sidebarCollapsed = false;
        private Timer _collapseTimer;
        private int _sidebarTargetW;
        private string _userName = "Admin";

        public MainForm()
        {
            DesignSystem.Initialize();
            InitializeMainForm();
            InitializeUserControls();
            StartClock();
            ActivateMenu(mnuDashboard);
        }

        private void InitializeMainForm()
        {
            SuspendLayout();
            Text = "Hệ Thống Chấm Công Nhận Diện Khuôn Mặt";
            ClientSize = new Size(DesignSystem.Scale(1280), DesignSystem.Scale(780));
            MinimumSize = new Size(DesignSystem.Scale(1050), DesignSystem.Scale(650));
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = DesignSystem.Background;
            Font = DesignSystem.Data;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            _collapseTimer = new Timer { Interval = 16 };
            _collapseTimer.Tick += CollapseTimer_Tick;

            // ─── HEADER BAR ─────────────────────────────────────────
            int headerH = DesignSystem.Scale(56);
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = headerH,
                BackColor = Color.White
            };
            pnlHeader.Paint += PnlHeader_Paint;

            // Logo (left, inside sidebar width)
            lblLogo = new Label
            {
                Text = "FaceID",
                Font = new Font("Segoe UI Semibold", DesignSystem.Scale(16f), FontStyle.Bold, GraphicsUnit.Pixel),
                ForeColor = DesignSystem.Accent,
                AutoSize = false,
                Size = new Size(DesignSystem.SidebarWidth - DesignSystem.Scale(44), headerH),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Left,
                BackColor = Color.Transparent
            };

            // Collapse button (right edge of sidebar area)
            btnCollapse = new Label
            {
                Text = "◀",
                Font = new Font("Segoe UI", DesignSystem.Scale(11f), GraphicsUnit.Pixel),
                ForeColor = DesignSystem.TextMuted,
                AutoSize = false,
                Size = new Size(DesignSystem.Scale(30), DesignSystem.Scale(30)),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            btnCollapse.Location = new Point(DesignSystem.SidebarWidth - DesignSystem.Scale(36),
                                              (headerH - DesignSystem.Scale(30)) / 2);
            btnCollapse.Click += BtnCollapse_Click;
            btnCollapse.MouseEnter += (s, e) => { btnCollapse.ForeColor = DesignSystem.Accent; btnCollapse.BackColor = Color.FromArgb(15, DesignSystem.Accent); };
            btnCollapse.MouseLeave += (s, e) => { btnCollapse.ForeColor = DesignSystem.TextMuted; btnCollapse.BackColor = Color.Transparent; };

            // Org name (after logo)
            lblOrgName = new Label
            {
                Text = "Tổ Chức",
                Font = DesignSystem.TitleSection,
                ForeColor = DesignSystem.TextPrimary,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(DesignSystem.Scale(12), 0, 0, 0),
                BackColor = Color.Transparent
            };

            // ─── RIGHT SECTION: fixed layout, no FlowLayout ────────
            int rightW = DesignSystem.Scale(380);
            var pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = rightW,
                BackColor = Color.Transparent
            };
            pnlRight.Paint += PnlHeaderRight_Paint;

            // Position elements manually for perfect vertical centering
            int midY = headerH / 2;
            int badgeW = DesignSystem.Scale(120);
            int badgeH = DesignSystem.Scale(24);
            int avatarSize = DesignSystem.Scale(30);
            int gap = DesignSystem.Scale(14);

            // Layout: [badge] [gap] [clock] [gap] [divider] [gap] [avatar] [gap] [username]
            int curX = DesignSystem.Scale(10);

            badgeSystemStatus = new StatusBadge
            {
                Text = "Online",
                Type = BadgeType.Success,
                ShowDot = true,
                Size = new Size(badgeW, badgeH),
                Location = new Point(curX, midY - badgeH / 2)
            };
            curX += badgeW + gap;

            lblClock = new Label
            {
                Text = DateTime.Now.ToString("HH:mm"),
                Font = DesignSystem.DataBold,
                ForeColor = DesignSystem.TextSecondary,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            // Will position after measuring
            var clockSize = TextRenderer.MeasureText(lblClock.Text, lblClock.Font);
            lblClock.Location = new Point(curX, midY - clockSize.Height / 2);
            curX += clockSize.Width + gap;

            // Vertical divider (painted)
            curX += DesignSystem.Scale(2); // divider space

            // Avatar
            var pnlAvatar = new Panel
            {
                Size = new Size(avatarSize, avatarSize),
                Location = new Point(curX + gap, midY - avatarSize / 2),
                BackColor = Color.Transparent
            };
            pnlAvatar.Paint += (s, e) =>
            {
                DesignSystem.ApplyHighQualityRendering(e.Graphics);
                DesignSystem.DrawAvatarCircle(e.Graphics,
                    new Rectangle(0, 0, avatarSize, avatarSize), _userName, DesignSystem.Accent);
            };
            curX += gap + avatarSize + DesignSystem.Scale(8);

            lblUser = new Label
            {
                Text = _userName,
                Font = DesignSystem.DataBold,
                ForeColor = DesignSystem.TextPrimary,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            var userSize = TextRenderer.MeasureText(lblUser.Text, lblUser.Font);
            lblUser.Location = new Point(curX, midY - userSize.Height / 2);

            pnlRight.Controls.AddRange(new Control[] {
                badgeSystemStatus, lblClock, pnlAvatar, lblUser
            });

            pnlHeader.Controls.Add(lblOrgName);
            pnlHeader.Controls.Add(pnlRight);
            pnlHeader.Controls.Add(btnCollapse);
            pnlHeader.Controls.Add(lblLogo);

            // ─── SIDEBAR ────────────────────────────────────────────
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = DesignSystem.SidebarWidth,
                BackColor = DesignSystem.SidebarBg,
                Padding = new Padding(0, DesignSystem.Scale(14), 0, 0)
            };
            pnlSidebar.Paint += PnlSidebar_Paint;

            mnuDashboard  = CreateMenuItem("📊", "Dashboard");
            mnuAttendance = CreateMenuItem("📷", "Chấm Công");
            mnuEmployees  = CreateMenuItem("👥", "Quản Lý Thành Viên");
            mnuReports    = CreateMenuItem("📋", "Báo Cáo");
            mnuSettings   = CreateMenuItem("⚙", "Cài Đặt");

            // Divider
            var divider = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = DesignSystem.SidebarDivider
            };

            // Exit button  
            var btnExit = new RoundedButton
            {
                Text = "Đăng Xuất",
                Variant = ButtonVariant.Outline,
                Size = new Size(DesignSystem.Scale(210), DesignSystem.ButtonHeight)
            };
            btnExit.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Application.Exit();
            };

            var pnlExitWrap = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = DesignSystem.Scale(60),
                BackColor = Color.Transparent,
                Padding = new Padding(DesignSystem.Scale(22), DesignSystem.Scale(10), DesignSystem.Scale(22), DesignSystem.Scale(10))
            };
            btnExit.Dock = DockStyle.Fill;
            pnlExitWrap.Controls.Add(btnExit);

            pnlSidebar.Controls.Add(pnlExitWrap);
            pnlSidebar.Controls.Add(divider);

            var items = new[] { mnuSettings, mnuReports, mnuEmployees, mnuAttendance, mnuDashboard };
            foreach (var item in items)
            {
                item.Dock = DockStyle.Top;
                pnlSidebar.Controls.Add(item);
            }

            // ─── CONTENT AREA ──────────────────────────────────────
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = DesignSystem.Background,
                Padding = new Padding(0)
            };

            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlHeader);
            ResumeLayout(false);

            KeyPreview = true;
            KeyDown += MainForm_KeyDown;
        }

        // ─── Custom Paint ──────────────────────────────────────────

        private void PnlHeader_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);
            var rect = new Rectangle(0, 0, pnlHeader.Width, pnlHeader.Height);

            // White header with very subtle blue tint
            using (var brush = new SolidBrush(Color.FromArgb(252, 253, 255)))
                g.FillRectangle(brush, rect);

            // Bottom border — clear separation
            using (var pen = new Pen(DesignSystem.Border, DesignSystem.Scale(1.2f)))
                g.DrawLine(pen, 0, rect.Bottom - 1, rect.Right, rect.Bottom - 1);

            // Sidebar divider line in header
            int sideW = pnlSidebar != null ? pnlSidebar.Width : DesignSystem.SidebarWidth;
            using (var pen = new Pen(DesignSystem.Border, 1f))
                g.DrawLine(pen, sideW, 0, sideW, rect.Bottom);
        }

        private void PnlHeaderRight_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            // Draw vertical divider between clock and avatar
            int divX = lblClock.Right + DesignSystem.Scale(10);
            int divTop = DesignSystem.Scale(14);
            int divBottom = pnlHeader.Height - DesignSystem.Scale(14);
            using (var pen = new Pen(DesignSystem.BorderLight, 1f))
                g.DrawLine(pen, divX, divTop, divX, divBottom);
        }

        private void PnlSidebar_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);
            var rect = new Rectangle(0, 0, pnlSidebar.Width, pnlSidebar.Height);

            // Sidebar background
            using (var brush = new SolidBrush(DesignSystem.SidebarBg))
                g.FillRectangle(brush, rect);

            // Right border — clear separation from content
            using (var pen = new Pen(DesignSystem.Border, DesignSystem.Scale(1.2f)))
                g.DrawLine(pen, rect.Right - 1, 0, rect.Right - 1, rect.Bottom);
        }

        // ─── Helpers ───────────────────────────────────────────────

        private SidebarMenuItem CreateMenuItem(string icon, string text)
        {
            var item = new SidebarMenuItem
            {
                Icon = icon,
                Text = text,
                Width = DesignSystem.SidebarWidth
            };
            item.ItemClicked += MenuItem_Click;
            return item;
        }

        private void InitializeUserControls()
        {
            ucDashboard = new UCDashboard { Dock = DockStyle.Fill };
            ucAttendance = new UCAttendance { Dock = DockStyle.Fill };
            ucEmployeeManagement = new UCEmployeeManagement { Dock = DockStyle.Fill };
            ucAttendanceReport = new UCAttendanceReport { Dock = DockStyle.Fill };
            ucSettings = new UCSettings { Dock = DockStyle.Fill };
        }

        private void StartClock()
        {
            clockTimer = new Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) => lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
            clockTimer.Start();
        }

        // ─── Navigation ────────────────────────────────────────────

        private void ShowUserControl(UserControl uc)
        {
            pnlContent.SuspendLayout();
            pnlContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(uc);
            pnlContent.ResumeLayout(true);
        }

        private void ActivateMenu(SidebarMenuItem item)
        {
            if (currentMenuItem != null) currentMenuItem.IsActive = false;
            currentMenuItem = item;
            currentMenuItem.IsActive = true;

            if (item == mnuDashboard)       ShowUserControl(ucDashboard);
            else if (item == mnuAttendance) ShowUserControl(ucAttendance);
            else if (item == mnuEmployees)  ShowUserControl(ucEmployeeManagement);
            else if (item == mnuReports)    ShowUserControl(ucAttendanceReport);
            else if (item == mnuSettings)   ShowUserControl(ucSettings);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ActivateMenu((SidebarMenuItem)sender);
        }

        // ─── Sidebar Collapse ──────────────────────────────────────

        private void BtnCollapse_Click(object sender, EventArgs e)
        {
            _sidebarCollapsed = !_sidebarCollapsed;
            _sidebarTargetW = _sidebarCollapsed ? DesignSystem.Scale(68) : DesignSystem.SidebarWidth;
            btnCollapse.Text = _sidebarCollapsed ? "▶" : "◀";

            foreach (Control c in pnlSidebar.Controls)
            {
                if (c is SidebarMenuItem mi) mi.IsCollapsed = _sidebarCollapsed;
            }
            _collapseTimer.Start();
        }

        private void CollapseTimer_Tick(object sender, EventArgs e)
        {
            int diff = _sidebarTargetW - pnlSidebar.Width;
            int step = Math.Max(1, Math.Abs(diff) / 4);
            if (Math.Abs(diff) <= 2)
            {
                pnlSidebar.Width = _sidebarTargetW;
                _collapseTimer.Stop();
            }
            else
            {
                pnlSidebar.Width += diff > 0 ? step : -step;
            }

            lblLogo.Width = pnlSidebar.Width - DesignSystem.Scale(44);
            btnCollapse.Location = new Point(
                pnlSidebar.Width - DesignSystem.Scale(36),
                (pnlHeader.Height - DesignSystem.Scale(30)) / 2);

            foreach (Control c in pnlSidebar.Controls)
            {
                if (c is SidebarMenuItem menu) menu.Width = pnlSidebar.Width;
            }
            pnlHeader.Invalidate();
        }

        // ─── Keyboard Shortcuts ────────────────────────────────────

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1: ActivateMenu(mnuDashboard); e.Handled = true; break;
                    case Keys.D2: ActivateMenu(mnuAttendance); e.Handled = true; break;
                    case Keys.D3: ActivateMenu(mnuEmployees); e.Handled = true; break;
                    case Keys.D4: ActivateMenu(mnuReports); e.Handled = true; break;
                    case Keys.D5: ActivateMenu(mnuSettings); e.Handled = true; break;
                }
            }
        }

        // ─── Public API ────────────────────────────────────────────

        public void SetOrganizationInfo(string orgName, string userName)
        {
            lblOrgName.Text = orgName;
            _userName = userName;
            lblUser.Text = userName;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clockTimer?.Stop(); clockTimer?.Dispose();
            _collapseTimer?.Stop(); _collapseTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
