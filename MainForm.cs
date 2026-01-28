using System;
using System.Drawing;
using System.Windows.Forms;
using FaceIDApp.UserControls;

namespace FaceIDApp
{
    public partial class MainForm : Form
    {
        // User Controls
        private UCDashboard ucDashboard;
        private UCAttendance ucAttendance;
        private UCEmployeeManagement ucEmployeeManagement;
        private UCFaceRegistration ucFaceRegistration;
        private UCAttendanceReport ucAttendanceReport;
        private UCSettings ucSettings;

        // Current active button
        private Button currentButton;

        public MainForm()
        {
            InitializeComponent();
            InitializeUserControls();
            SetupUI();
        }

        private void InitializeUserControls()
        {
            ucDashboard = new UCDashboard { Dock = DockStyle.Fill };
            ucAttendance = new UCAttendance { Dock = DockStyle.Fill };
            ucEmployeeManagement = new UCEmployeeManagement { Dock = DockStyle.Fill };
            ucFaceRegistration = new UCFaceRegistration { Dock = DockStyle.Fill };
            ucAttendanceReport = new UCAttendanceReport { Dock = DockStyle.Fill };
            ucSettings = new UCSettings { Dock = DockStyle.Fill };
        }

        private void SetupUI()
        {
            // Show Dashboard by default
            ShowUserControl(ucDashboard);
            ActivateButton(btnDashboard);
        }

        private void ShowUserControl(UserControl uc)
        {
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(uc);
        }

        private void ActivateButton(Button button)
        {
            if (currentButton != null)
            {
                currentButton.BackColor = Color.FromArgb(44, 62, 80);
                currentButton.ForeColor = Color.White;
            }

            currentButton = button;
            currentButton.BackColor = Color.FromArgb(41, 128, 185);
            currentButton.ForeColor = Color.White;
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucDashboard);
            ActivateButton(btnDashboard);
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucAttendance);
            ActivateButton(btnAttendance);
        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucEmployeeManagement);
            ActivateButton(btnEmployees);
        }

        private void btnFaceRegistration_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucFaceRegistration);
            ActivateButton(btnFaceRegistration);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucAttendanceReport);
            ActivateButton(btnReports);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucSettings);
            ActivateButton(btnSettings);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
