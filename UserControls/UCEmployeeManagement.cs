using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceIDApp.UserControls
{
    public partial class UCEmployeeManagement : UserControl
    {
        public UCEmployeeManagement()
        {
            InitializeComponent();
            SetupUI();
            LoadSampleData();
        }

        private void SetupUI()
        {
            // Style DataGridView
            dgvEmployees.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgvEmployees.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEmployees.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvEmployees.ColumnHeadersHeight = 35;
            dgvEmployees.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvEmployees.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvEmployees.RowTemplate.Height = 30;
            dgvEmployees.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);

            // Set default filter
            cboFilterDepartment.SelectedIndex = 0;

            // Event handlers
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnSearch.Click += BtnSearch_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            btnRegisterFace.Click += BtnRegisterFace_Click;
            dgvEmployees.SelectionChanged += DgvEmployees_SelectionChanged;
        }

        private void LoadSampleData()
        {
            // Sample employee data
            dgvEmployees.Rows.Add("1", "NV001", "Nguyễn Văn An", "Phòng IT", "Developer", "0901234567", "✅ Đã đăng ký", "Đang làm việc");
            dgvEmployees.Rows.Add("2", "NV002", "Trần Thị Bình", "Phòng Kế toán", "Kế toán viên", "0902345678", "✅ Đã đăng ký", "Đang làm việc");
            dgvEmployees.Rows.Add("3", "NV003", "Lê Văn Cường", "Phòng Nhân sự", "HR Manager", "0903456789", "❌ Chưa đăng ký", "Đang làm việc");
            dgvEmployees.Rows.Add("4", "NV004", "Phạm Thị Dung", "Phòng Marketing", "Marketing Executive", "0904567890", "✅ Đã đăng ký", "Đang làm việc");
            dgvEmployees.Rows.Add("5", "NV005", "Hoàng Văn Em", "Phòng IT", "Tester", "0905678901", "❌ Chưa đăng ký", "Nghỉ việc");
        }

        private void DgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                var row = dgvEmployees.SelectedRows[0];
                txtEmployeeCode.Text = row.Cells["colCode"].Value?.ToString();
                txtFullName.Text = row.Cells["colFullName"].Value?.ToString();
                
                string dept = row.Cells["colDepartment"].Value?.ToString();
                int deptIndex = cboDepartment.FindString(dept);
                if (deptIndex >= 0)
                    cboDepartment.SelectedIndex = deptIndex;
                
                txtPosition.Text = row.Cells["colPosition"].Value?.ToString();
                txtPhone.Text = row.Cells["colPhone"].Value?.ToString();
            }
        }

        // Event handlers - placeholders for backend integration
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            txtEmployeeCode.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Chức năng xóa sẽ được tích hợp với backend.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng làm mới sẽ được tích hợp với backend.", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Tìm kiếm: {txtSearch.Text}\nChức năng sẽ được tích hợp với backend.", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmployeeCode.Text) || string.IsNullOrEmpty(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Chức năng lưu sẽ được tích hợp với backend.", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnRegisterFace_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần đăng ký Face ID!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // This will navigate to Face Registration UC
            MessageBox.Show("Chuyển sang màn hình đăng ký Face ID...", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearForm()
        {
            txtEmployeeCode.Text = "";
            txtFullName.Text = "";
            cboDepartment.SelectedIndex = -1;
            txtPosition.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            dtpDateOfBirth.Value = DateTime.Now.AddYears(-25);
            dtpHireDate.Value = DateTime.Now;
            chkIsActive.Checked = true;
            picEmployeePhoto.Image = null;
        }
    }
}
