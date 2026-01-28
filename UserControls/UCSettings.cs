using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceIDApp.UserControls
{
    public partial class UCSettings : UserControl
    {
        public UCSettings()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // Set default values
            cboCameraSource.SelectedIndex = 0;
            cboLanguage.SelectedIndex = 0;

            // Event handlers
            btnTestConnection.Click += BtnTestConnection_Click;
            btnSave.Click += BtnSave_Click;
            btnReset.Click += BtnReset_Click;
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            // Simulate connection test - will be replaced with actual PostgreSQL connection
            lblConnectionStatus.Text = "⏳ Đang kiểm tra...";
            lblConnectionStatus.ForeColor = Color.FromArgb(243, 156, 18);

            // Simulate async operation
            System.Threading.Tasks.Task.Delay(1500).ContinueWith(t =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (!string.IsNullOrEmpty(txtServer.Text) && !string.IsNullOrEmpty(txtDatabase.Text))
                    {
                        lblConnectionStatus.Text = "✅ Kết nối thành công!";
                        lblConnectionStatus.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                    else
                    {
                        lblConnectionStatus.Text = "❌ Kết nối thất bại!";
                        lblConnectionStatus.ForeColor = Color.FromArgb(231, 76, 60);
                    }
                });
            });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(txtServer.Text) || string.IsNullOrEmpty(txtDatabase.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin kết nối cơ sở dữ liệu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Đã lưu cài đặt thành công!\n\nChức năng lưu cấu hình sẽ được tích hợp với backend.", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn khôi phục cài đặt mặc định?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ResetToDefaults();
                MessageBox.Show("Đã khôi phục cài đặt mặc định!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetToDefaults()
        {
            // Database
            txtServer.Text = "localhost";
            txtPort.Text = "5432";
            txtDatabase.Text = "faceid_db";
            txtUsername.Text = "postgres";
            txtPassword.Text = "";
            lblConnectionStatus.Text = "⏳ Chưa kiểm tra kết nối";
            lblConnectionStatus.ForeColor = Color.FromArgb(127, 140, 141);

            // Work time
            dtpWorkStart.Value = DateTime.Today.AddHours(8);
            dtpWorkEnd.Value = DateTime.Today.AddHours(17).AddMinutes(30);
            nudLateThreshold.Value = 15;

            // Camera
            cboCameraSource.SelectedIndex = 0;
            nudConfidenceThreshold.Value = 80;
            chkAutoCapture.Checked = true;

            // System
            chkAutoStart.Checked = false;
            chkMinimizeToTray.Checked = true;
            chkPlaySound.Checked = true;
            cboLanguage.SelectedIndex = 0;
        }

        // Public methods for backend integration
        public string GetConnectionString()
        {
            return $"Host={txtServer.Text};Port={txtPort.Text};Database={txtDatabase.Text};Username={txtUsername.Text};Password={txtPassword.Text}";
        }

        public void SetConnectionStatus(bool isConnected, string message)
        {
            if (isConnected)
            {
                lblConnectionStatus.Text = $"✅ {message}";
                lblConnectionStatus.ForeColor = Color.FromArgb(46, 204, 113);
            }
            else
            {
                lblConnectionStatus.Text = $"❌ {message}";
                lblConnectionStatus.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }
    }
}
