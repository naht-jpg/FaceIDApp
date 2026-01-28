using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceIDApp.UserControls
{
    public partial class UCAttendance : UserControl
    {
        private Timer timerDateTime;

        public UCAttendance()
        {
            InitializeComponent();
            SetupUI();
            SetupTimer();
            LoadSampleData();
        }

        private void SetupUI()
        {
            // Style DataGridView
            dgvTodayAttendance.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgvTodayAttendance.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTodayAttendance.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvTodayAttendance.ColumnHeadersHeight = 35;
            dgvTodayAttendance.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvTodayAttendance.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvTodayAttendance.RowTemplate.Height = 30;
            dgvTodayAttendance.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);

            // Event handlers
            btnStartCamera.Click += BtnStartCamera_Click;
            btnStopCamera.Click += BtnStopCamera_Click;
            btnCapture.Click += BtnCapture_Click;
            btnCheckIn.Click += BtnCheckIn_Click;
            btnCheckOut.Click += BtnCheckOut_Click;
        }

        private void SetupTimer()
        {
            timerDateTime = new Timer();
            timerDateTime.Interval = 1000;
            timerDateTime.Tick += TimerDateTime_Tick;
            timerDateTime.Start();
        }

        private void TimerDateTime_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy");
        }

        private void LoadSampleData()
        {
            // Sample attendance data
            dgvTodayAttendance.Rows.Add("1", "NV001", "Nguyễn Văn An", "08:05:23", "17:30:15", "Đúng giờ");
            dgvTodayAttendance.Rows.Add("2", "NV002", "Trần Thị Bình", "08:12:45", "--:--:--", "Đi trễ");
            dgvTodayAttendance.Rows.Add("3", "NV003", "Lê Văn Cường", "08:00:12", "17:45:22", "Đúng giờ");
        }

        // Event handlers - placeholders for backend integration
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
            MessageBox.Show("Chức năng chụp ảnh sẽ được tích hợp với backend.", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCheckIn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Check-in sẽ được tích hợp với backend.", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCheckOut_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Check-out sẽ được tích hợp với backend.", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Public methods for backend integration
        public void UpdateEmployeeInfo(string name, string code, string department, string position)
        {
            lblEmployeeName.Text = name;
            lblEmployeeCode.Text = $"Mã NV: {code}";
            lblDepartment.Text = $"🏢 Phòng ban: {department}";
            lblPosition.Text = $"💼 Chức vụ: {position}";
        }

        public void UpdateStatus(string status, Color color)
        {
            lblStatus.Text = status;
            lblStatus.ForeColor = color;
        }

        public void UpdateCheckInTime(DateTime time)
        {
            lblCheckInTime.Text = $"⏰ Giờ vào: {time:HH:mm:ss}";
        }

        public void SetCameraImage(Image image)
        {
            picCamera.Image = image;
        }

        public void SetEmployeePhoto(Image image)
        {
            picEmployeePhoto.Image = image;
        }
    }
}
