using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FaceIDApp.UserControls
{
    public partial class UCFaceRegistration : UserControl
    {
        private int capturedCount = 0;
        private List<PictureBox> capturedPictures;

        public UCFaceRegistration()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // Initialize captured pictures list
            capturedPictures = new List<PictureBox> { pic1, pic2, pic3, pic4, pic5 };

            // Set default selection
            cboSelectEmployee.SelectedIndex = 0;

            // Event handlers
            cboSelectEmployee.SelectedIndexChanged += CboSelectEmployee_SelectedIndexChanged;
            btnStartCamera.Click += BtnStartCamera_Click;
            btnStopCamera.Click += BtnStopCamera_Click;
            btnCapture.Click += BtnCapture_Click;
            btnClearAll.Click += BtnClearAll_Click;
            btnRegister.Click += BtnRegister_Click;
        }

        private void CboSelectEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSelectEmployee.SelectedIndex > 0)
            {
                // Sample data - will be replaced with backend call
                string selectedText = cboSelectEmployee.SelectedItem.ToString();
                string employeeCode = selectedText.Split('-')[0].Trim();
                
                switch (employeeCode)
                {
                    case "NV001":
                        lblSelectedInfo.Text = "Phòng ban: Phòng IT\nTrạng thái Face ID: ✅ Đã đăng ký";
                        lblSelectedInfo.ForeColor = Color.FromArgb(46, 204, 113);
                        break;
                    case "NV002":
                        lblSelectedInfo.Text = "Phòng ban: Phòng Kế toán\nTrạng thái Face ID: ✅ Đã đăng ký";
                        lblSelectedInfo.ForeColor = Color.FromArgb(46, 204, 113);
                        break;
                    case "NV003":
                        lblSelectedInfo.Text = "Phòng ban: Phòng Nhân sự\nTrạng thái Face ID: ❌ Chưa đăng ký";
                        lblSelectedInfo.ForeColor = Color.FromArgb(231, 76, 60);
                        break;
                    default:
                        lblSelectedInfo.Text = "Phòng ban: ---\nTrạng thái Face ID: ❌ Chưa đăng ký";
                        lblSelectedInfo.ForeColor = Color.FromArgb(127, 140, 141);
                        break;
                }
            }
            else
            {
                lblSelectedInfo.Text = "Phòng ban: ---\nTrạng thái Face ID: ---";
                lblSelectedInfo.ForeColor = Color.FromArgb(127, 140, 141);
            }
        }

        private void BtnStartCamera_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng bật camera sẽ được tích hợp với backend.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnStopCamera_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng tắt camera sẽ được tích hợp với backend.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCapture_Click(object sender, EventArgs e)
        {
            if (cboSelectEmployee.SelectedIndex == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên trước khi chụp ảnh!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (capturedCount >= 5)
            {
                MessageBox.Show("Đã chụp đủ 5 ảnh. Vui lòng đăng ký hoặc xóa để chụp lại.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Simulate capture - will be replaced with actual camera capture
            capturedPictures[capturedCount].BackColor = Color.FromArgb(46, 204, 113);
            capturedCount++;
            UpdateProgress();

            MessageBox.Show($"Đã chụp ảnh thứ {capturedCount}. Chức năng thực sự sẽ được tích hợp với backend.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa tất cả ảnh đã chụp?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ClearAllCaptures();
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (cboSelectEmployee.SelectedIndex == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (capturedCount < 5)
            {
                MessageBox.Show($"Vui lòng chụp đủ 5 ảnh (hiện tại: {capturedCount}/5)!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Xác nhận đăng ký Face ID cho nhân viên này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Đăng ký Face ID thành công!\n\nChức năng thực sự sẽ được tích hợp với backend.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ClearAllCaptures();
                cboSelectEmployee.SelectedIndex = 0;
            }
        }

        private void ClearAllCaptures()
        {
            foreach (var pic in capturedPictures)
            {
                pic.Image = null;
                pic.BackColor = Color.FromArgb(236, 240, 241);
            }
            capturedCount = 0;
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            lblProgress.Text = $"Đã chụp: {capturedCount}/5 ảnh";
            progressBar.Value = capturedCount;
        }

        // Public methods for backend integration
        public void SetCameraImage(Image image)
        {
            picCamera.Image = image;
        }

        public void SetCapturedImage(int index, Image image)
        {
            if (index >= 0 && index < capturedPictures.Count)
            {
                capturedPictures[index].Image = image;
            }
        }
    }
}
