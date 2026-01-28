using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceIDApp.UserControls
{
    public partial class UCAttendanceReport : UserControl
    {
        public UCAttendanceReport()
        {
            InitializeComponent();
            SetupUI();
            LoadSampleData();
        }

        private void SetupUI()
        {
            // Style DataGridView
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvReport.ColumnHeadersHeight = 35;
            dgvReport.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvReport.RowTemplate.Height = 30;
            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);

            // Set default values
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            cboEmployee.SelectedIndex = 0;
            cboDepartment.SelectedIndex = 0;

            // Event handlers
            btnSearch.Click += BtnSearch_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnPrint.Click += BtnPrint_Click;
        }

        private void LoadSampleData()
        {
            // Sample report data
            dgvReport.Rows.Add("27/01/2026", "NV001", "Nguyễn Văn An", "Phòng IT", "08:05:23", "17:30:15", "8.5h", "Đúng giờ", "");
            dgvReport.Rows.Add("27/01/2026", "NV002", "Trần Thị Bình", "Phòng Kế toán", "08:12:45", "17:45:30", "8.5h", "Đi trễ", "Kẹt xe");
            dgvReport.Rows.Add("27/01/2026", "NV003", "Lê Văn Cường", "Phòng Nhân sự", "08:00:12", "17:30:22", "8.5h", "Đúng giờ", "");
            dgvReport.Rows.Add("26/01/2026", "NV001", "Nguyễn Văn An", "Phòng IT", "07:58:45", "17:35:10", "8.6h", "Đúng giờ", "");
            dgvReport.Rows.Add("26/01/2026", "NV002", "Trần Thị Bình", "Phòng Kế toán", "08:02:33", "17:30:45", "8.5h", "Đúng giờ", "");
            dgvReport.Rows.Add("26/01/2026", "NV003", "Lê Văn Cường", "Phòng Nhân sự", "08:15:55", "18:00:12", "8.7h", "Đi trễ", "Họp đột xuất");
            dgvReport.Rows.Add("25/01/2026", "NV001", "Nguyễn Văn An", "Phòng IT", "08:01:22", "17:32:45", "8.5h", "Đúng giờ", "");
            dgvReport.Rows.Add("25/01/2026", "NV002", "Trần Thị Bình", "Phòng Kế toán", "07:55:18", "17:28:33", "8.5h", "Đúng giờ", "");

            // Style status column
            foreach (DataGridViewRow row in dgvReport.Rows)
            {
                if (row.Cells["colStatus"].Value != null)
                {
                    string status = row.Cells["colStatus"].Value.ToString();
                    if (status == "Đúng giờ")
                    {
                        row.Cells["colStatus"].Style.ForeColor = Color.FromArgb(46, 204, 113);
                        row.Cells["colStatus"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                    else if (status == "Đi trễ")
                    {
                        row.Cells["colStatus"].Style.ForeColor = Color.FromArgb(231, 76, 60);
                        row.Cells["colStatus"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Tìm kiếm báo cáo từ {dtpFromDate.Value:dd/MM/yyyy} đến {dtpToDate.Value:dd/MM/yyyy}\n\nChức năng sẽ được tích hợp với backend.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel sẽ được tích hợp với backend.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng in báo cáo sẽ được tích hợp với backend.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Public methods for backend integration
        public void UpdateSummary(int totalDays, int presentDays, int lateDays, int absentDays)
        {
            lblTotalDaysValue.Text = totalDays.ToString();
            lblPresentDaysValue.Text = presentDays.ToString();
            lblLateDaysValue.Text = lateDays.ToString();
            lblAbsentDaysValue.Text = absentDays.ToString();
        }
    }
}
