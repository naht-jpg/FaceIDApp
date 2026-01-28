namespace FaceIDApp.UserControls
{
    partial class UCSettings
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.grpDatabase = new System.Windows.Forms.GroupBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.grpWorkTime = new System.Windows.Forms.GroupBox();
            this.lblWorkStart = new System.Windows.Forms.Label();
            this.dtpWorkStart = new System.Windows.Forms.DateTimePicker();
            this.lblWorkEnd = new System.Windows.Forms.Label();
            this.dtpWorkEnd = new System.Windows.Forms.DateTimePicker();
            this.lblLateThreshold = new System.Windows.Forms.Label();
            this.nudLateThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblMinutes = new System.Windows.Forms.Label();
            this.grpCamera = new System.Windows.Forms.GroupBox();
            this.lblCameraSource = new System.Windows.Forms.Label();
            this.cboCameraSource = new System.Windows.Forms.ComboBox();
            this.lblConfidenceThreshold = new System.Windows.Forms.Label();
            this.nudConfidenceThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblPercent = new System.Windows.Forms.Label();
            this.chkAutoCapture = new System.Windows.Forms.CheckBox();
            this.grpSystem = new System.Windows.Forms.GroupBox();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkMinimizeToTray = new System.Windows.Forms.CheckBox();
            this.chkPlaySound = new System.Windows.Forms.CheckBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.grpDatabase.SuspendLayout();
            this.grpWorkTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLateThreshold)).BeginInit();
            this.grpCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConfidenceThreshold)).BeginInit();
            this.grpSystem.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(20);
            this.pnlHeader.Size = new System.Drawing.Size(900, 70);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 17);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "⚙️ Cài đặt hệ thống";
            // 
            // pnlContent
            // 
            this.pnlContent.AutoScroll = true;
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlContent.Controls.Add(this.grpSystem);
            this.pnlContent.Controls.Add(this.grpCamera);
            this.pnlContent.Controls.Add(this.grpWorkTime);
            this.pnlContent.Controls.Add(this.grpDatabase);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 70);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.pnlContent.Size = new System.Drawing.Size(900, 470);
            this.pnlContent.TabIndex = 1;
            // 
            // grpDatabase
            // 
            this.grpDatabase.BackColor = System.Drawing.Color.White;
            this.grpDatabase.Controls.Add(this.lblConnectionStatus);
            this.grpDatabase.Controls.Add(this.btnTestConnection);
            this.grpDatabase.Controls.Add(this.txtPassword);
            this.grpDatabase.Controls.Add(this.lblPassword);
            this.grpDatabase.Controls.Add(this.txtUsername);
            this.grpDatabase.Controls.Add(this.lblUsername);
            this.grpDatabase.Controls.Add(this.txtDatabase);
            this.grpDatabase.Controls.Add(this.lblDatabase);
            this.grpDatabase.Controls.Add(this.txtPort);
            this.grpDatabase.Controls.Add(this.lblPort);
            this.grpDatabase.Controls.Add(this.txtServer);
            this.grpDatabase.Controls.Add(this.lblServer);
            this.grpDatabase.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDatabase.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpDatabase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.grpDatabase.Location = new System.Drawing.Point(20, 15);
            this.grpDatabase.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.grpDatabase.Name = "grpDatabase";
            this.grpDatabase.Padding = new System.Windows.Forms.Padding(15);
            this.grpDatabase.Size = new System.Drawing.Size(860, 130);
            this.grpDatabase.TabIndex = 0;
            this.grpDatabase.TabStop = false;
            this.grpDatabase.Text = "🗄️ Cơ sở dữ liệu PostgreSQL";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblServer.Location = new System.Drawing.Point(15, 35);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(46, 17);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server:";
            // 
            // txtServer
            // 
            this.txtServer.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtServer.Location = new System.Drawing.Point(80, 32);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(150, 24);
            this.txtServer.TabIndex = 1;
            this.txtServer.Text = "localhost";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblPort.Location = new System.Drawing.Point(245, 35);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(34, 17);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtPort.Location = new System.Drawing.Point(285, 32);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(70, 24);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "5432";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblDatabase.Location = new System.Drawing.Point(370, 35);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(64, 17);
            this.lblDatabase.TabIndex = 4;
            this.lblDatabase.Text = "Database:";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtDatabase.Location = new System.Drawing.Point(440, 32);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(150, 24);
            this.txtDatabase.TabIndex = 5;
            this.txtDatabase.Text = "faceid_db";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblUsername.Location = new System.Drawing.Point(15, 70);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(57, 17);
            this.lblUsername.TabIndex = 6;
            this.lblUsername.Text = "User:";
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtUsername.Location = new System.Drawing.Point(80, 67);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(150, 24);
            this.txtUsername.TabIndex = 7;
            this.txtUsername.Text = "postgres";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblPassword.Location = new System.Drawing.Point(245, 70);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(67, 17);
            this.lblPassword.TabIndex = 8;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtPassword.Location = new System.Drawing.Point(320, 67);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(150, 24);
            this.txtPassword.TabIndex = 9;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnTestConnection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestConnection.FlatAppearance.BorderSize = 0;
            this.btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestConnection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Location = new System.Drawing.Point(490, 65);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(100, 28);
            this.btnTestConnection.TabIndex = 10;
            this.btnTestConnection.Text = "🔗 Test kết nối";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblConnectionStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.lblConnectionStatus.Location = new System.Drawing.Point(600, 68);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(240, 23);
            this.lblConnectionStatus.TabIndex = 11;
            this.lblConnectionStatus.Text = "⏳ Chưa kiểm tra kết nối";
            this.lblConnectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpWorkTime
            // 
            this.grpWorkTime.BackColor = System.Drawing.Color.White;
            this.grpWorkTime.Controls.Add(this.lblMinutes);
            this.grpWorkTime.Controls.Add(this.nudLateThreshold);
            this.grpWorkTime.Controls.Add(this.lblLateThreshold);
            this.grpWorkTime.Controls.Add(this.dtpWorkEnd);
            this.grpWorkTime.Controls.Add(this.lblWorkEnd);
            this.grpWorkTime.Controls.Add(this.dtpWorkStart);
            this.grpWorkTime.Controls.Add(this.lblWorkStart);
            this.grpWorkTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpWorkTime.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpWorkTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.grpWorkTime.Location = new System.Drawing.Point(20, 145);
            this.grpWorkTime.Name = "grpWorkTime";
            this.grpWorkTime.Padding = new System.Windows.Forms.Padding(15);
            this.grpWorkTime.Size = new System.Drawing.Size(860, 80);
            this.grpWorkTime.TabIndex = 1;
            this.grpWorkTime.TabStop = false;
            this.grpWorkTime.Text = "🕐 Thời gian làm việc";
            // 
            // lblWorkStart
            // 
            this.lblWorkStart.AutoSize = true;
            this.lblWorkStart.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblWorkStart.Location = new System.Drawing.Point(15, 40);
            this.lblWorkStart.Name = "lblWorkStart";
            this.lblWorkStart.Size = new System.Drawing.Size(72, 17);
            this.lblWorkStart.TabIndex = 0;
            this.lblWorkStart.Text = "Giờ bắt đầu:";
            // 
            // dtpWorkStart
            // 
            this.dtpWorkStart.CustomFormat = "HH:mm";
            this.dtpWorkStart.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.dtpWorkStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkStart.Location = new System.Drawing.Point(95, 37);
            this.dtpWorkStart.Name = "dtpWorkStart";
            this.dtpWorkStart.ShowUpDown = true;
            this.dtpWorkStart.Size = new System.Drawing.Size(80, 24);
            this.dtpWorkStart.TabIndex = 1;
            this.dtpWorkStart.Value = new System.DateTime(2026, 1, 28, 8, 0, 0, 0);
            // 
            // lblWorkEnd
            // 
            this.lblWorkEnd.AutoSize = true;
            this.lblWorkEnd.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblWorkEnd.Location = new System.Drawing.Point(195, 40);
            this.lblWorkEnd.Name = "lblWorkEnd";
            this.lblWorkEnd.Size = new System.Drawing.Size(78, 17);
            this.lblWorkEnd.TabIndex = 2;
            this.lblWorkEnd.Text = "Giờ kết thúc:";
            // 
            // dtpWorkEnd
            // 
            this.dtpWorkEnd.CustomFormat = "HH:mm";
            this.dtpWorkEnd.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.dtpWorkEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkEnd.Location = new System.Drawing.Point(280, 37);
            this.dtpWorkEnd.Name = "dtpWorkEnd";
            this.dtpWorkEnd.ShowUpDown = true;
            this.dtpWorkEnd.Size = new System.Drawing.Size(80, 24);
            this.dtpWorkEnd.TabIndex = 3;
            this.dtpWorkEnd.Value = new System.DateTime(2026, 1, 28, 17, 30, 0, 0);
            // 
            // lblLateThreshold
            // 
            this.lblLateThreshold.AutoSize = true;
            this.lblLateThreshold.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblLateThreshold.Location = new System.Drawing.Point(390, 40);
            this.lblLateThreshold.Name = "lblLateThreshold";
            this.lblLateThreshold.Size = new System.Drawing.Size(136, 17);
            this.lblLateThreshold.TabIndex = 4;
            this.lblLateThreshold.Text = "Ngưỡng tính đi trễ sau:";
            // 
            // nudLateThreshold
            // 
            this.nudLateThreshold.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.nudLateThreshold.Location = new System.Drawing.Point(535, 37);
            this.nudLateThreshold.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudLateThreshold.Name = "nudLateThreshold";
            this.nudLateThreshold.Size = new System.Drawing.Size(60, 24);
            this.nudLateThreshold.TabIndex = 5;
            this.nudLateThreshold.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // lblMinutes
            // 
            this.lblMinutes.AutoSize = true;
            this.lblMinutes.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblMinutes.Location = new System.Drawing.Point(600, 40);
            this.lblMinutes.Name = "lblMinutes";
            this.lblMinutes.Size = new System.Drawing.Size(36, 17);
            this.lblMinutes.TabIndex = 6;
            this.lblMinutes.Text = "phút";
            // 
            // grpCamera
            // 
            this.grpCamera.BackColor = System.Drawing.Color.White;
            this.grpCamera.Controls.Add(this.chkAutoCapture);
            this.grpCamera.Controls.Add(this.lblPercent);
            this.grpCamera.Controls.Add(this.nudConfidenceThreshold);
            this.grpCamera.Controls.Add(this.lblConfidenceThreshold);
            this.grpCamera.Controls.Add(this.cboCameraSource);
            this.grpCamera.Controls.Add(this.lblCameraSource);
            this.grpCamera.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCamera.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.grpCamera.Location = new System.Drawing.Point(20, 225);
            this.grpCamera.Name = "grpCamera";
            this.grpCamera.Padding = new System.Windows.Forms.Padding(15);
            this.grpCamera.Size = new System.Drawing.Size(860, 80);
            this.grpCamera.TabIndex = 2;
            this.grpCamera.TabStop = false;
            this.grpCamera.Text = "📷 Camera & Nhận diện";
            // 
            // lblCameraSource
            // 
            this.lblCameraSource.AutoSize = true;
            this.lblCameraSource.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCameraSource.Location = new System.Drawing.Point(15, 40);
            this.lblCameraSource.Name = "lblCameraSource";
            this.lblCameraSource.Size = new System.Drawing.Size(89, 17);
            this.lblCameraSource.TabIndex = 0;
            this.lblCameraSource.Text = "Nguồn camera:";
            // 
            // cboCameraSource
            // 
            this.cboCameraSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCameraSource.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboCameraSource.FormattingEnabled = true;
            this.cboCameraSource.Items.AddRange(new object[] {
            "Webcam mặc định",
            "Camera USB 0",
            "Camera USB 1",
            "IP Camera"});
            this.cboCameraSource.Location = new System.Drawing.Point(115, 37);
            this.cboCameraSource.Name = "cboCameraSource";
            this.cboCameraSource.Size = new System.Drawing.Size(180, 25);
            this.cboCameraSource.TabIndex = 1;
            // 
            // lblConfidenceThreshold
            // 
            this.lblConfidenceThreshold.AutoSize = true;
            this.lblConfidenceThreshold.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblConfidenceThreshold.Location = new System.Drawing.Point(320, 40);
            this.lblConfidenceThreshold.Name = "lblConfidenceThreshold";
            this.lblConfidenceThreshold.Size = new System.Drawing.Size(133, 17);
            this.lblConfidenceThreshold.TabIndex = 2;
            this.lblConfidenceThreshold.Text = "Độ chính xác tối thiểu:";
            // 
            // nudConfidenceThreshold
            // 
            this.nudConfidenceThreshold.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.nudConfidenceThreshold.Location = new System.Drawing.Point(460, 37);
            this.nudConfidenceThreshold.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudConfidenceThreshold.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudConfidenceThreshold.Name = "nudConfidenceThreshold";
            this.nudConfidenceThreshold.Size = new System.Drawing.Size(60, 24);
            this.nudConfidenceThreshold.TabIndex = 3;
            this.nudConfidenceThreshold.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblPercent.Location = new System.Drawing.Point(525, 40);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(21, 17);
            this.lblPercent.TabIndex = 4;
            this.lblPercent.Text = "%";
            // 
            // chkAutoCapture
            // 
            this.chkAutoCapture.AutoSize = true;
            this.chkAutoCapture.Checked = true;
            this.chkAutoCapture.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoCapture.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.chkAutoCapture.Location = new System.Drawing.Point(580, 39);
            this.chkAutoCapture.Name = "chkAutoCapture";
            this.chkAutoCapture.Size = new System.Drawing.Size(171, 21);
            this.chkAutoCapture.TabIndex = 5;
            this.chkAutoCapture.Text = "Tự động nhận diện liên tục";
            this.chkAutoCapture.UseVisualStyleBackColor = true;
            // 
            // grpSystem
            // 
            this.grpSystem.BackColor = System.Drawing.Color.White;
            this.grpSystem.Controls.Add(this.cboLanguage);
            this.grpSystem.Controls.Add(this.lblLanguage);
            this.grpSystem.Controls.Add(this.chkPlaySound);
            this.grpSystem.Controls.Add(this.chkMinimizeToTray);
            this.grpSystem.Controls.Add(this.chkAutoStart);
            this.grpSystem.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSystem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpSystem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.grpSystem.Location = new System.Drawing.Point(20, 305);
            this.grpSystem.Name = "grpSystem";
            this.grpSystem.Padding = new System.Windows.Forms.Padding(15);
            this.grpSystem.Size = new System.Drawing.Size(860, 80);
            this.grpSystem.TabIndex = 3;
            this.grpSystem.TabStop = false;
            this.grpSystem.Text = "💻 Hệ thống";
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.chkAutoStart.Location = new System.Drawing.Point(18, 40);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(174, 21);
            this.chkAutoStart.TabIndex = 0;
            this.chkAutoStart.Text = "Khởi động cùng Windows";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // chkMinimizeToTray
            // 
            this.chkMinimizeToTray.AutoSize = true;
            this.chkMinimizeToTray.Checked = true;
            this.chkMinimizeToTray.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMinimizeToTray.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.chkMinimizeToTray.Location = new System.Drawing.Point(210, 40);
            this.chkMinimizeToTray.Name = "chkMinimizeToTray";
            this.chkMinimizeToTray.Size = new System.Drawing.Size(161, 21);
            this.chkMinimizeToTray.TabIndex = 1;
            this.chkMinimizeToTray.Text = "Thu nhỏ xuống khay hệ thống";
            this.chkMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // chkPlaySound
            // 
            this.chkPlaySound.AutoSize = true;
            this.chkPlaySound.Checked = true;
            this.chkPlaySound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPlaySound.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.chkPlaySound.Location = new System.Drawing.Point(400, 40);
            this.chkPlaySound.Name = "chkPlaySound";
            this.chkPlaySound.Size = new System.Drawing.Size(167, 21);
            this.chkPlaySound.TabIndex = 2;
            this.chkPlaySound.Text = "Phát âm thanh khi chấm công";
            this.chkPlaySound.UseVisualStyleBackColor = true;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblLanguage.Location = new System.Drawing.Point(600, 41);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(60, 17);
            this.lblLanguage.TabIndex = 3;
            this.lblLanguage.Text = "Ngôn ngữ:";
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Items.AddRange(new object[] {
            "🇻🇳 Tiếng Việt",
            "🇬🇧 English"});
            this.cboLanguage.Location = new System.Drawing.Point(670, 38);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(150, 25);
            this.cboLanguage.TabIndex = 4;
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.White;
            this.pnlButtons.Controls.Add(this.btnReset);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 540);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(20);
            this.pnlButtons.Size = new System.Drawing.Size(900, 60);
            this.pnlButtons.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(650, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 40);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "💾 Lưu cài đặt";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(776, 10);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(110, 40);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "🔄 Mặc định";
            this.btnReset.UseVisualStyleBackColor = false;
            // 
            // UCSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UCSettings";
            this.Size = new System.Drawing.Size(900, 600);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.grpDatabase.ResumeLayout(false);
            this.grpDatabase.PerformLayout();
            this.grpWorkTime.ResumeLayout(false);
            this.grpWorkTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLateThreshold)).EndInit();
            this.grpCamera.ResumeLayout(false);
            this.grpCamera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConfidenceThreshold)).EndInit();
            this.grpSystem.ResumeLayout(false);
            this.grpSystem.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.GroupBox grpDatabase;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.GroupBox grpWorkTime;
        private System.Windows.Forms.Label lblWorkStart;
        private System.Windows.Forms.DateTimePicker dtpWorkStart;
        private System.Windows.Forms.Label lblWorkEnd;
        private System.Windows.Forms.DateTimePicker dtpWorkEnd;
        private System.Windows.Forms.Label lblLateThreshold;
        private System.Windows.Forms.NumericUpDown nudLateThreshold;
        private System.Windows.Forms.Label lblMinutes;
        private System.Windows.Forms.GroupBox grpCamera;
        private System.Windows.Forms.Label lblCameraSource;
        private System.Windows.Forms.ComboBox cboCameraSource;
        private System.Windows.Forms.Label lblConfidenceThreshold;
        private System.Windows.Forms.NumericUpDown nudConfidenceThreshold;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.CheckBox chkAutoCapture;
        private System.Windows.Forms.GroupBox grpSystem;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.CheckBox chkMinimizeToTray;
        private System.Windows.Forms.CheckBox chkPlaySound;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnReset;
    }
}
