namespace FaceIDApp.UserControls
{
    partial class UCDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlStats = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlTotalEmployees = new System.Windows.Forms.Panel();
            this.lblTotalEmployeesValue = new System.Windows.Forms.Label();
            this.lblTotalEmployeesTitle = new System.Windows.Forms.Label();
            this.pnlPresentToday = new System.Windows.Forms.Panel();
            this.lblPresentTodayValue = new System.Windows.Forms.Label();
            this.lblPresentTodayTitle = new System.Windows.Forms.Label();
            this.pnlLateToday = new System.Windows.Forms.Panel();
            this.lblLateTodayValue = new System.Windows.Forms.Label();
            this.lblLateTodayTitle = new System.Windows.Forms.Label();
            this.pnlAbsentToday = new System.Windows.Forms.Panel();
            this.lblAbsentTodayValue = new System.Windows.Forms.Label();
            this.lblAbsentTodayTitle = new System.Windows.Forms.Label();
            this.pnlRecentActivity = new System.Windows.Forms.Panel();
            this.dgvRecentActivity = new System.Windows.Forms.DataGridView();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmployeeCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmployeeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDepartment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblRecentActivityTitle = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlStats.SuspendLayout();
            this.pnlTotalEmployees.SuspendLayout();
            this.pnlPresentToday.SuspendLayout();
            this.pnlLateToday.SuspendLayout();
            this.pnlAbsentToday.SuspendLayout();
            this.pnlRecentActivity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentActivity)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblDate);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(30, 31, 30, 31);
            this.pnlHeader.Size = new System.Drawing.Size(1350, 123);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblDate
            // 
            this.lblDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblDate.ForeColor = System.Drawing.Color.Gray;
            this.lblDate.Location = new System.Drawing.Point(900, 38);
            this.lblDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(420, 35);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "Thứ Hai, 28/01/2026";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 31);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(263, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📊 Dashboard";
            // 
            // pnlStats
            // 
            this.pnlStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlStats.Controls.Add(this.pnlTotalEmployees);
            this.pnlStats.Controls.Add(this.pnlPresentToday);
            this.pnlStats.Controls.Add(this.pnlLateToday);
            this.pnlStats.Controls.Add(this.pnlAbsentToday);
            this.pnlStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStats.Location = new System.Drawing.Point(0, 123);
            this.pnlStats.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Padding = new System.Windows.Forms.Padding(22, 23, 22, 23);
            this.pnlStats.Size = new System.Drawing.Size(1350, 231);
            this.pnlStats.TabIndex = 1;
            // 
            // pnlTotalEmployees
            // 
            this.pnlTotalEmployees.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.pnlTotalEmployees.Controls.Add(this.lblTotalEmployeesValue);
            this.pnlTotalEmployees.Controls.Add(this.lblTotalEmployeesTitle);
            this.pnlTotalEmployees.Location = new System.Drawing.Point(26, 28);
            this.pnlTotalEmployees.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTotalEmployees.Name = "pnlTotalEmployees";
            this.pnlTotalEmployees.Size = new System.Drawing.Size(300, 169);
            this.pnlTotalEmployees.TabIndex = 0;
            // 
            // lblTotalEmployeesValue
            // 
            this.lblTotalEmployeesValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalEmployeesValue.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblTotalEmployeesValue.ForeColor = System.Drawing.Color.White;
            this.lblTotalEmployeesValue.Location = new System.Drawing.Point(0, 0);
            this.lblTotalEmployeesValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalEmployeesValue.Name = "lblTotalEmployeesValue";
            this.lblTotalEmployeesValue.Size = new System.Drawing.Size(300, 115);
            this.lblTotalEmployeesValue.TabIndex = 0;
            this.lblTotalEmployeesValue.Text = "150";
            this.lblTotalEmployeesValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalEmployeesTitle
            // 
            this.lblTotalEmployeesTitle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblTotalEmployeesTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTotalEmployeesTitle.ForeColor = System.Drawing.Color.White;
            this.lblTotalEmployeesTitle.Location = new System.Drawing.Point(0, 115);
            this.lblTotalEmployeesTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalEmployeesTitle.Name = "lblTotalEmployeesTitle";
            this.lblTotalEmployeesTitle.Size = new System.Drawing.Size(300, 54);
            this.lblTotalEmployeesTitle.TabIndex = 1;
            this.lblTotalEmployeesTitle.Text = "👥 Tổng nhân viên";
            this.lblTotalEmployeesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlPresentToday
            // 
            this.pnlPresentToday.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.pnlPresentToday.Controls.Add(this.lblPresentTodayValue);
            this.pnlPresentToday.Controls.Add(this.lblPresentTodayTitle);
            this.pnlPresentToday.Location = new System.Drawing.Point(334, 28);
            this.pnlPresentToday.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlPresentToday.Name = "pnlPresentToday";
            this.pnlPresentToday.Size = new System.Drawing.Size(300, 169);
            this.pnlPresentToday.TabIndex = 1;
            // 
            // lblPresentTodayValue
            // 
            this.lblPresentTodayValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPresentTodayValue.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblPresentTodayValue.ForeColor = System.Drawing.Color.White;
            this.lblPresentTodayValue.Location = new System.Drawing.Point(0, 0);
            this.lblPresentTodayValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPresentTodayValue.Name = "lblPresentTodayValue";
            this.lblPresentTodayValue.Size = new System.Drawing.Size(300, 115);
            this.lblPresentTodayValue.TabIndex = 0;
            this.lblPresentTodayValue.Text = "142";
            this.lblPresentTodayValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPresentTodayTitle
            // 
            this.lblPresentTodayTitle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblPresentTodayTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPresentTodayTitle.ForeColor = System.Drawing.Color.White;
            this.lblPresentTodayTitle.Location = new System.Drawing.Point(0, 115);
            this.lblPresentTodayTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPresentTodayTitle.Name = "lblPresentTodayTitle";
            this.lblPresentTodayTitle.Size = new System.Drawing.Size(300, 54);
            this.lblPresentTodayTitle.TabIndex = 1;
            this.lblPresentTodayTitle.Text = "✅ Có mặt hôm nay";
            this.lblPresentTodayTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlLateToday
            // 
            this.pnlLateToday.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.pnlLateToday.Controls.Add(this.lblLateTodayValue);
            this.pnlLateToday.Controls.Add(this.lblLateTodayTitle);
            this.pnlLateToday.Location = new System.Drawing.Point(642, 28);
            this.pnlLateToday.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlLateToday.Name = "pnlLateToday";
            this.pnlLateToday.Size = new System.Drawing.Size(300, 169);
            this.pnlLateToday.TabIndex = 2;
            // 
            // lblLateTodayValue
            // 
            this.lblLateTodayValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLateTodayValue.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblLateTodayValue.ForeColor = System.Drawing.Color.White;
            this.lblLateTodayValue.Location = new System.Drawing.Point(0, 0);
            this.lblLateTodayValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLateTodayValue.Name = "lblLateTodayValue";
            this.lblLateTodayValue.Size = new System.Drawing.Size(300, 115);
            this.lblLateTodayValue.TabIndex = 0;
            this.lblLateTodayValue.Text = "5";
            this.lblLateTodayValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLateTodayTitle
            // 
            this.lblLateTodayTitle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblLateTodayTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblLateTodayTitle.ForeColor = System.Drawing.Color.White;
            this.lblLateTodayTitle.Location = new System.Drawing.Point(0, 115);
            this.lblLateTodayTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLateTodayTitle.Name = "lblLateTodayTitle";
            this.lblLateTodayTitle.Size = new System.Drawing.Size(300, 54);
            this.lblLateTodayTitle.TabIndex = 1;
            this.lblLateTodayTitle.Text = "⚠️ Đi trễ hôm nay";
            this.lblLateTodayTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlAbsentToday
            // 
            this.pnlAbsentToday.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.pnlAbsentToday.Controls.Add(this.lblAbsentTodayValue);
            this.pnlAbsentToday.Controls.Add(this.lblAbsentTodayTitle);
            this.pnlAbsentToday.Location = new System.Drawing.Point(950, 28);
            this.pnlAbsentToday.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlAbsentToday.Name = "pnlAbsentToday";
            this.pnlAbsentToday.Size = new System.Drawing.Size(300, 169);
            this.pnlAbsentToday.TabIndex = 3;
            // 
            // lblAbsentTodayValue
            // 
            this.lblAbsentTodayValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAbsentTodayValue.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblAbsentTodayValue.ForeColor = System.Drawing.Color.White;
            this.lblAbsentTodayValue.Location = new System.Drawing.Point(0, 0);
            this.lblAbsentTodayValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAbsentTodayValue.Name = "lblAbsentTodayValue";
            this.lblAbsentTodayValue.Size = new System.Drawing.Size(300, 115);
            this.lblAbsentTodayValue.TabIndex = 0;
            this.lblAbsentTodayValue.Text = "3";
            this.lblAbsentTodayValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAbsentTodayTitle
            // 
            this.lblAbsentTodayTitle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblAbsentTodayTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblAbsentTodayTitle.ForeColor = System.Drawing.Color.White;
            this.lblAbsentTodayTitle.Location = new System.Drawing.Point(0, 115);
            this.lblAbsentTodayTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAbsentTodayTitle.Name = "lblAbsentTodayTitle";
            this.lblAbsentTodayTitle.Size = new System.Drawing.Size(300, 54);
            this.lblAbsentTodayTitle.TabIndex = 1;
            this.lblAbsentTodayTitle.Text = "❌ Vắng mặt";
            this.lblAbsentTodayTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlRecentActivity
            // 
            this.pnlRecentActivity.BackColor = System.Drawing.Color.White;
            this.pnlRecentActivity.Controls.Add(this.dgvRecentActivity);
            this.pnlRecentActivity.Controls.Add(this.lblRecentActivityTitle);
            this.pnlRecentActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRecentActivity.Location = new System.Drawing.Point(0, 354);
            this.pnlRecentActivity.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlRecentActivity.Name = "pnlRecentActivity";
            this.pnlRecentActivity.Padding = new System.Windows.Forms.Padding(30, 31, 30, 31);
            this.pnlRecentActivity.Size = new System.Drawing.Size(1350, 569);
            this.pnlRecentActivity.TabIndex = 2;
            // 
            // dgvRecentActivity
            // 
            this.dgvRecentActivity.AllowUserToAddRows = false;
            this.dgvRecentActivity.AllowUserToDeleteRows = false;
            this.dgvRecentActivity.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecentActivity.BackgroundColor = System.Drawing.Color.White;
            this.dgvRecentActivity.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRecentActivity.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvRecentActivity.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvRecentActivity.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecentActivity.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colEmployeeCode,
            this.colEmployeeName,
            this.colDepartment,
            this.colAction,
            this.colStatus});
            this.dgvRecentActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecentActivity.EnableHeadersVisualStyles = false;
            this.dgvRecentActivity.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.dgvRecentActivity.Location = new System.Drawing.Point(30, 84);
            this.dgvRecentActivity.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvRecentActivity.Name = "dgvRecentActivity";
            this.dgvRecentActivity.ReadOnly = true;
            this.dgvRecentActivity.RowHeadersVisible = false;
            this.dgvRecentActivity.RowHeadersWidth = 62;
            this.dgvRecentActivity.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecentActivity.Size = new System.Drawing.Size(1290, 454);
            this.dgvRecentActivity.TabIndex = 1;
            // 
            // colTime
            // 
            this.colTime.HeaderText = "Thời gian";
            this.colTime.MinimumWidth = 8;
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            // 
            // colEmployeeCode
            // 
            this.colEmployeeCode.HeaderText = "Mã NV";
            this.colEmployeeCode.MinimumWidth = 8;
            this.colEmployeeCode.Name = "colEmployeeCode";
            this.colEmployeeCode.ReadOnly = true;
            // 
            // colEmployeeName
            // 
            this.colEmployeeName.HeaderText = "Họ tên";
            this.colEmployeeName.MinimumWidth = 8;
            this.colEmployeeName.Name = "colEmployeeName";
            this.colEmployeeName.ReadOnly = true;
            // 
            // colDepartment
            // 
            this.colDepartment.HeaderText = "Phòng ban";
            this.colDepartment.MinimumWidth = 8;
            this.colDepartment.Name = "colDepartment";
            this.colDepartment.ReadOnly = true;
            // 
            // colAction
            // 
            this.colAction.HeaderText = "Hành động";
            this.colAction.MinimumWidth = 8;
            this.colAction.Name = "colAction";
            this.colAction.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Trạng thái";
            this.colStatus.MinimumWidth = 8;
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // lblRecentActivityTitle
            // 
            this.lblRecentActivityTitle.AutoSize = true;
            this.lblRecentActivityTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRecentActivityTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblRecentActivityTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblRecentActivityTitle.Location = new System.Drawing.Point(30, 31);
            this.lblRecentActivityTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRecentActivityTitle.Name = "lblRecentActivityTitle";
            this.lblRecentActivityTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.lblRecentActivityTitle.Size = new System.Drawing.Size(469, 53);
            this.lblRecentActivityTitle.TabIndex = 0;
            this.lblRecentActivityTitle.Text = "📋 Hoạt động chấm công gần đây";
            // 
            // UCDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.Controls.Add(this.pnlRecentActivity);
            this.Controls.Add(this.pnlStats);
            this.Controls.Add(this.pnlHeader);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UCDashboard";
            this.Size = new System.Drawing.Size(1350, 923);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlStats.ResumeLayout(false);
            this.pnlTotalEmployees.ResumeLayout(false);
            this.pnlPresentToday.ResumeLayout(false);
            this.pnlLateToday.ResumeLayout(false);
            this.pnlAbsentToday.ResumeLayout(false);
            this.pnlRecentActivity.ResumeLayout(false);
            this.pnlRecentActivity.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentActivity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.FlowLayoutPanel pnlStats;
        private System.Windows.Forms.Panel pnlTotalEmployees;
        private System.Windows.Forms.Label lblTotalEmployeesValue;
        private System.Windows.Forms.Label lblTotalEmployeesTitle;
        private System.Windows.Forms.Panel pnlPresentToday;
        private System.Windows.Forms.Label lblPresentTodayValue;
        private System.Windows.Forms.Label lblPresentTodayTitle;
        private System.Windows.Forms.Panel pnlLateToday;
        private System.Windows.Forms.Label lblLateTodayValue;
        private System.Windows.Forms.Label lblLateTodayTitle;
        private System.Windows.Forms.Panel pnlAbsentToday;
        private System.Windows.Forms.Label lblAbsentTodayValue;
        private System.Windows.Forms.Label lblAbsentTodayTitle;
        private System.Windows.Forms.Panel pnlRecentActivity;
        private System.Windows.Forms.Label lblRecentActivityTitle;
        private System.Windows.Forms.DataGridView dgvRecentActivity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmployeeCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmployeeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDepartment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
    }
}
