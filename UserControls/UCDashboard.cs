using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace FaceIDApp.UserControls
{
    public partial class UCDashboard : UserControl
    {
        public UCDashboard()
        {
            InitializeComponent();
            SetupUI();
            LoadSampleData();
        }

        private void SetupUI()
        {
            // Update date label
            lblDate.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy");

            // Style DataGridView
            dgvRecentActivity.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgvRecentActivity.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRecentActivity.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvRecentActivity.ColumnHeadersHeight = 40;
            dgvRecentActivity.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvRecentActivity.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvRecentActivity.RowTemplate.Height = 35;
            dgvRecentActivity.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
        }

        private void LoadSampleData()
        {
            // Sample data for demonstration
            dgvRecentActivity.Rows.Add("08:05:23", "NV001", "Nguyễn Văn An", "Phòng IT", "Check-in", "Đúng giờ");
            dgvRecentActivity.Rows.Add("08:12:45", "NV002", "Trần Thị Bình", "Phòng Kế toán", "Check-in", "Đi trễ");
            dgvRecentActivity.Rows.Add("08:00:12", "NV003", "Lê Văn Cường", "Phòng Nhân sự", "Check-in", "Đúng giờ");
            dgvRecentActivity.Rows.Add("07:58:33", "NV004", "Phạm Thị Dung", "Phòng Marketing", "Check-in", "Đúng giờ");
            dgvRecentActivity.Rows.Add("08:03:21", "NV005", "Hoàng Văn Em", "Phòng IT", "Check-in", "Đúng giờ");
            dgvRecentActivity.Rows.Add("17:30:15", "NV001", "Nguyễn Văn An", "Phòng IT", "Check-out", "Đúng giờ");
            dgvRecentActivity.Rows.Add("17:45:22", "NV003", "Lê Văn Cường", "Phòng Nhân sự", "Check-out", "Đúng giờ");

            // Style status column
            foreach (DataGridViewRow row in dgvRecentActivity.Rows)
            {
                if (row.Cells["colStatus"].Value != null)
                {
                    string status = row.Cells["colStatus"].Value.ToString();
                    if (status == "Đúng giờ")
                    {
                        row.Cells["colStatus"].Style.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                    else if (status == "Đi trễ")
                    {
                        row.Cells["colStatus"].Style.ForeColor = Color.FromArgb(231, 76, 60);
                    }
                }
            }
        }

        // Public methods to update statistics (will be called from backend later)
        public void UpdateTotalEmployees(int count)
        {
            lblTotalEmployeesValue.Text = count.ToString();
        }

        public void UpdatePresentToday(int count)
        {
            lblPresentTodayValue.Text = count.ToString();
        }

        public void UpdateLateToday(int count)
        {
            lblLateTodayValue.Text = count.ToString();
        }

        public void UpdateAbsentToday(int count)
        {
            lblAbsentTodayValue.Text = count.ToString();
        }
    }
}
