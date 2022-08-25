namespace nkv.Automator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.licenceTab = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.productComboBox = new System.Windows.Forms.ComboBox();
            this.registerBtn = new System.Windows.Forms.Button();
            this.sourceComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.systemTextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.emailTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.purchaseCodeTextbox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.licenceDataGridView = new System.Windows.Forms.DataGridView();
            this.mysqlTab = new System.Windows.Forms.TabPage();
            this.projectNameControlMySql = new nkv.Automator.ProjectNameControl();
            this.logTextAreaControl2 = new nkv.Automator.LogTextAreaControl();
            this.adminPanelPermissionControlPanel2 = new nkv.Automator.AdminPanelPermissionControlPanel();
            this.tableSelectionControl2 = new nkv.Automator.TableSelectionControl();
            this.authSelectionControl2 = new nkv.Automator.AuthSelectionControl();
            this.mysqlGenerateButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.passwordMysqlTextBox = new System.Windows.Forms.TextBox();
            this.usernameMysqlTextBox = new System.Windows.Forms.TextBox();
            this.dbNameMysqlTextBox = new System.Windows.Forms.TextBox();
            this.portMysqlTextbox = new System.Windows.Forms.TextBox();
            this.hostMysqlTextBox = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.msSqlTab = new System.Windows.Forms.TabPage();
            this.mongodbTab = new System.Windows.Forms.TabPage();
            this.postgresqlTab = new System.Windows.Forms.TabPage();
            this.projectNameControlPgSQL = new nkv.Automator.ProjectNameControl();
            this.logTextAreaControlpgSQL = new nkv.Automator.LogTextAreaControl();
            this.adminPanelPermissionControlPanelPgSQL = new nkv.Automator.AdminPanelPermissionControlPanel();
            this.tableSelectionControlPgSQL = new nkv.Automator.TableSelectionControl();
            this.authSelectionControlPgSQL = new nkv.Automator.AuthSelectionControl();
            this.pgSQLGenerateButton = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.multiTenantCheckBoxPgSQL = new System.Windows.Forms.CheckBox();
            this.testConPgSQLButton = new System.Windows.Forms.Button();
            this.passwordPGTextBox = new System.Windows.Forms.TextBox();
            this.usernamePGTextBox = new System.Windows.Forms.TextBox();
            this.schemaNamePGTextBox = new System.Windows.Forms.TextBox();
            this.dbNamePGTextBox = new System.Windows.Forms.TextBox();
            this.portPGTextbox = new System.Windows.Forms.TextBox();
            this.hostPGTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.communityTab = new System.Windows.Forms.TabPage();
            this.docsTab = new System.Windows.Forms.TabPage();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.refreshButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.licenceTab.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.licenceDataGridView)).BeginInit();
            this.mysqlTab.SuspendLayout();
            this.panel3.SuspendLayout();
            this.postgresqlTab.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.licenceTab);
            this.tabControl1.Controls.Add(this.mysqlTab);
            this.tabControl1.Controls.Add(this.msSqlTab);
            this.tabControl1.Controls.Add(this.mongodbTab);
            this.tabControl1.Controls.Add(this.postgresqlTab);
            this.tabControl1.Controls.Add(this.communityTab);
            this.tabControl1.Controls.Add(this.docsTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabControl1.ItemSize = new System.Drawing.Size(52, 23);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(10, 4);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1557, 768);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 0;
            // 
            // licenceTab
            // 
            this.licenceTab.Controls.Add(this.panel2);
            this.licenceTab.Controls.Add(this.panel1);
            this.licenceTab.Location = new System.Drawing.Point(4, 27);
            this.licenceTab.Name = "licenceTab";
            this.licenceTab.Padding = new System.Windows.Forms.Padding(3);
            this.licenceTab.Size = new System.Drawing.Size(1549, 737);
            this.licenceTab.TabIndex = 0;
            this.licenceTab.Text = "Licence";
            this.licenceTab.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.refreshButton);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.productComboBox);
            this.panel2.Controls.Add(this.registerBtn);
            this.panel2.Controls.Add(this.sourceComboBox);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.systemTextbox);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.emailTextbox);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.purchaseCodeTextbox);
            this.panel2.Location = new System.Drawing.Point(8, 313);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1241, 282);
            this.panel2.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(83, 184);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "Product";
            // 
            // productComboBox
            // 
            this.productComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.productComboBox.FormattingEnabled = true;
            this.productComboBox.Items.AddRange(new object[] {
            "CodeCanyon",
            "GetAutomator.com"});
            this.productComboBox.Location = new System.Drawing.Point(166, 181);
            this.productComboBox.Name = "productComboBox";
            this.productComboBox.Size = new System.Drawing.Size(1004, 23);
            this.productComboBox.TabIndex = 4;
            // 
            // registerBtn
            // 
            this.registerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(60)))), ((int)(((byte)(81)))));
            this.registerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerBtn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.registerBtn.ForeColor = System.Drawing.Color.Transparent;
            this.registerBtn.Location = new System.Drawing.Point(1002, 221);
            this.registerBtn.Name = "registerBtn";
            this.registerBtn.Size = new System.Drawing.Size(168, 42);
            this.registerBtn.TabIndex = 5;
            this.registerBtn.Text = "Register";
            this.registerBtn.UseVisualStyleBackColor = false;
            this.registerBtn.Click += new System.EventHandler(this.registerBtn_Click);
            // 
            // sourceComboBox
            // 
            this.sourceComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sourceComboBox.FormattingEnabled = true;
            this.sourceComboBox.Items.AddRange(new object[] {
            "CodeCanyon",
            "GetAutomator.com"});
            this.sourceComboBox.Location = new System.Drawing.Point(745, 129);
            this.sourceComboBox.Name = "sourceComboBox";
            this.sourceComboBox.Size = new System.Drawing.Size(425, 23);
            this.sourceComboBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(83, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "System";
            // 
            // systemTextbox
            // 
            this.systemTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.systemTextbox.Enabled = false;
            this.systemTextbox.Location = new System.Drawing.Point(166, 129);
            this.systemTextbox.Name = "systemTextbox";
            this.systemTextbox.Size = new System.Drawing.Size(425, 23);
            this.systemTextbox.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(629, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Purchased From";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(83, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Email";
            // 
            // emailTextbox
            // 
            this.emailTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.emailTextbox.Location = new System.Drawing.Point(166, 56);
            this.emailTextbox.Name = "emailTextbox";
            this.emailTextbox.Size = new System.Drawing.Size(425, 23);
            this.emailTextbox.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(636, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Purchase Code";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(532, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Register New Licence";
            // 
            // purchaseCodeTextbox
            // 
            this.purchaseCodeTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.purchaseCodeTextbox.Location = new System.Drawing.Point(745, 56);
            this.purchaseCodeTextbox.Name = "purchaseCodeTextbox";
            this.purchaseCodeTextbox.Size = new System.Drawing.Size(425, 23);
            this.purchaseCodeTextbox.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.licenceDataGridView);
            this.panel1.Location = new System.Drawing.Point(8, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1241, 282);
            this.panel1.TabIndex = 0;
            // 
            // licenceDataGridView
            // 
            this.licenceDataGridView.AllowUserToAddRows = false;
            this.licenceDataGridView.AllowUserToDeleteRows = false;
            this.licenceDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.licenceDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.licenceDataGridView.Dock = System.Windows.Forms.DockStyle.Top;
            this.licenceDataGridView.Location = new System.Drawing.Point(0, 0);
            this.licenceDataGridView.Name = "licenceDataGridView";
            this.licenceDataGridView.ReadOnly = true;
            this.licenceDataGridView.RowTemplate.Height = 25;
            this.licenceDataGridView.Size = new System.Drawing.Size(1239, 276);
            this.licenceDataGridView.TabIndex = 0;
            // 
            // mysqlTab
            // 
            this.mysqlTab.Controls.Add(this.projectNameControlMySql);
            this.mysqlTab.Controls.Add(this.logTextAreaControl2);
            this.mysqlTab.Controls.Add(this.adminPanelPermissionControlPanel2);
            this.mysqlTab.Controls.Add(this.tableSelectionControl2);
            this.mysqlTab.Controls.Add(this.authSelectionControl2);
            this.mysqlTab.Controls.Add(this.mysqlGenerateButton);
            this.mysqlTab.Controls.Add(this.panel3);
            this.mysqlTab.Location = new System.Drawing.Point(4, 27);
            this.mysqlTab.Name = "mysqlTab";
            this.mysqlTab.Padding = new System.Windows.Forms.Padding(3);
            this.mysqlTab.Size = new System.Drawing.Size(1549, 737);
            this.mysqlTab.TabIndex = 1;
            this.mysqlTab.Text = "MySQL";
            this.mysqlTab.UseVisualStyleBackColor = true;
            // 
            // projectNameControlMySql
            // 
            this.projectNameControlMySql.Location = new System.Drawing.Point(8, 9);
            this.projectNameControlMySql.Name = "projectNameControlMySql";
            this.projectNameControlMySql.Size = new System.Drawing.Size(630, 40);
            this.projectNameControlMySql.TabIndex = 26;
            // 
            // logTextAreaControl2
            // 
            this.logTextAreaControl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logTextAreaControl2.Location = new System.Drawing.Point(8, 455);
            this.logTextAreaControl2.Name = "logTextAreaControl2";
            this.logTextAreaControl2.Size = new System.Drawing.Size(839, 191);
            this.logTextAreaControl2.TabIndex = 25;
            // 
            // adminPanelPermissionControlPanel2
            // 
            this.adminPanelPermissionControlPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.adminPanelPermissionControlPanel2.Location = new System.Drawing.Point(985, 55);
            this.adminPanelPermissionControlPanel2.Name = "adminPanelPermissionControlPanel2";
            this.adminPanelPermissionControlPanel2.Size = new System.Drawing.Size(527, 443);
            this.adminPanelPermissionControlPanel2.TabIndex = 24;
            // 
            // tableSelectionControl2
            // 
            this.tableSelectionControl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableSelectionControl2.Location = new System.Drawing.Point(398, 239);
            this.tableSelectionControl2.Name = "tableSelectionControl2";
            this.tableSelectionControl2.Size = new System.Drawing.Size(570, 199);
            this.tableSelectionControl2.TabIndex = 23;
            // 
            // authSelectionControl2
            // 
            this.authSelectionControl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.authSelectionControl2.Location = new System.Drawing.Point(398, 55);
            this.authSelectionControl2.Name = "authSelectionControl2";
            this.authSelectionControl2.Size = new System.Drawing.Size(570, 172);
            this.authSelectionControl2.TabIndex = 22;
            // 
            // mysqlGenerateButton
            // 
            this.mysqlGenerateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(60)))), ((int)(((byte)(81)))));
            this.mysqlGenerateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mysqlGenerateButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.mysqlGenerateButton.ForeColor = System.Drawing.Color.Transparent;
            this.mysqlGenerateButton.Location = new System.Drawing.Point(853, 552);
            this.mysqlGenerateButton.Name = "mysqlGenerateButton";
            this.mysqlGenerateButton.Size = new System.Drawing.Size(115, 94);
            this.mysqlGenerateButton.TabIndex = 18;
            this.mysqlGenerateButton.Text = "Generate";
            this.mysqlGenerateButton.UseVisualStyleBackColor = false;
            this.mysqlGenerateButton.Click += new System.EventHandler(this.automatorGenerateButton_Click);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.label16);
            this.panel3.Controls.Add(this.label17);
            this.panel3.Controls.Add(this.label18);
            this.panel3.Controls.Add(this.label19);
            this.panel3.Controls.Add(this.passwordMysqlTextBox);
            this.panel3.Controls.Add(this.usernameMysqlTextBox);
            this.panel3.Controls.Add(this.dbNameMysqlTextBox);
            this.panel3.Controls.Add(this.portMysqlTextbox);
            this.panel3.Controls.Add(this.hostMysqlTextBox);
            this.panel3.Controls.Add(this.label20);
            this.panel3.Controls.Add(this.label21);
            this.panel3.Location = new System.Drawing.Point(8, 55);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(378, 383);
            this.panel3.TabIndex = 15;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(60)))), ((int)(((byte)(81)))));
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(91, 292);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(256, 47);
            this.button1.TabIndex = 26;
            this.button1.Text = "Test Connection";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 251);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 15);
            this.label16.TabIndex = 25;
            this.label16.Text = "Password";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 201);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(60, 15);
            this.label17.TabIndex = 24;
            this.label17.Text = "Username";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(12, 149);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(57, 15);
            this.label18.TabIndex = 23;
            this.label18.Text = "DB Name";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(14, 100);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 15);
            this.label19.TabIndex = 22;
            this.label19.Text = "Port";
            // 
            // passwordMysqlTextBox
            // 
            this.passwordMysqlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordMysqlTextBox.Location = new System.Drawing.Point(88, 248);
            this.passwordMysqlTextBox.Name = "passwordMysqlTextBox";
            this.passwordMysqlTextBox.Size = new System.Drawing.Size(259, 23);
            this.passwordMysqlTextBox.TabIndex = 21;
            // 
            // usernameMysqlTextBox
            // 
            this.usernameMysqlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.usernameMysqlTextBox.Location = new System.Drawing.Point(88, 198);
            this.usernameMysqlTextBox.Name = "usernameMysqlTextBox";
            this.usernameMysqlTextBox.Size = new System.Drawing.Size(259, 23);
            this.usernameMysqlTextBox.TabIndex = 20;
            this.usernameMysqlTextBox.Text = "root";
            // 
            // dbNameMysqlTextBox
            // 
            this.dbNameMysqlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dbNameMysqlTextBox.Location = new System.Drawing.Point(88, 146);
            this.dbNameMysqlTextBox.Name = "dbNameMysqlTextBox";
            this.dbNameMysqlTextBox.Size = new System.Drawing.Size(259, 23);
            this.dbNameMysqlTextBox.TabIndex = 19;
            this.dbNameMysqlTextBox.Text = "atestdatabase";
            // 
            // portMysqlTextbox
            // 
            this.portMysqlTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.portMysqlTextbox.Location = new System.Drawing.Point(88, 97);
            this.portMysqlTextbox.Name = "portMysqlTextbox";
            this.portMysqlTextbox.Size = new System.Drawing.Size(100, 23);
            this.portMysqlTextbox.TabIndex = 18;
            this.portMysqlTextbox.Text = "3306";
            // 
            // hostMysqlTextBox
            // 
            this.hostMysqlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hostMysqlTextBox.Location = new System.Drawing.Point(88, 57);
            this.hostMysqlTextBox.Name = "hostMysqlTextBox";
            this.hostMysqlTextBox.Size = new System.Drawing.Size(259, 23);
            this.hostMysqlTextBox.TabIndex = 17;
            this.hostMysqlTextBox.Text = "localhost";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(14, 60);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(32, 15);
            this.label20.TabIndex = 16;
            this.label20.Text = "Host";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(149, 20);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(45, 15);
            this.label21.TabIndex = 15;
            this.label21.Text = "MySQL";
            // 
            // msSqlTab
            // 
            this.msSqlTab.Location = new System.Drawing.Point(4, 27);
            this.msSqlTab.Name = "msSqlTab";
            this.msSqlTab.Size = new System.Drawing.Size(1549, 737);
            this.msSqlTab.TabIndex = 4;
            this.msSqlTab.Text = "MS SQL";
            this.msSqlTab.UseVisualStyleBackColor = true;
            // 
            // mongodbTab
            // 
            this.mongodbTab.Location = new System.Drawing.Point(4, 27);
            this.mongodbTab.Name = "mongodbTab";
            this.mongodbTab.Size = new System.Drawing.Size(1549, 737);
            this.mongodbTab.TabIndex = 6;
            this.mongodbTab.Text = "MongoDB";
            this.mongodbTab.UseVisualStyleBackColor = true;
            // 
            // postgresqlTab
            // 
            this.postgresqlTab.Controls.Add(this.projectNameControlPgSQL);
            this.postgresqlTab.Controls.Add(this.logTextAreaControlpgSQL);
            this.postgresqlTab.Controls.Add(this.adminPanelPermissionControlPanelPgSQL);
            this.postgresqlTab.Controls.Add(this.tableSelectionControlPgSQL);
            this.postgresqlTab.Controls.Add(this.authSelectionControlPgSQL);
            this.postgresqlTab.Controls.Add(this.pgSQLGenerateButton);
            this.postgresqlTab.Controls.Add(this.panel8);
            this.postgresqlTab.Location = new System.Drawing.Point(4, 27);
            this.postgresqlTab.Name = "postgresqlTab";
            this.postgresqlTab.Size = new System.Drawing.Size(1549, 737);
            this.postgresqlTab.TabIndex = 5;
            this.postgresqlTab.Text = "PostgreSQL";
            this.postgresqlTab.UseVisualStyleBackColor = true;
            // 
            // projectNameControlPgSQL
            // 
            this.projectNameControlPgSQL.Location = new System.Drawing.Point(8, 8);
            this.projectNameControlPgSQL.Name = "projectNameControlPgSQL";
            this.projectNameControlPgSQL.Size = new System.Drawing.Size(630, 40);
            this.projectNameControlPgSQL.TabIndex = 34;
            // 
            // logTextAreaControlpgSQL
            // 
            this.logTextAreaControlpgSQL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logTextAreaControlpgSQL.Location = new System.Drawing.Point(8, 455);
            this.logTextAreaControlpgSQL.Name = "logTextAreaControlpgSQL";
            this.logTextAreaControlpgSQL.Size = new System.Drawing.Size(839, 191);
            this.logTextAreaControlpgSQL.TabIndex = 33;
            // 
            // adminPanelPermissionControlPanelPgSQL
            // 
            this.adminPanelPermissionControlPanelPgSQL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.adminPanelPermissionControlPanelPgSQL.Location = new System.Drawing.Point(1002, 54);
            this.adminPanelPermissionControlPanelPgSQL.Name = "adminPanelPermissionControlPanelPgSQL";
            this.adminPanelPermissionControlPanelPgSQL.Size = new System.Drawing.Size(527, 443);
            this.adminPanelPermissionControlPanelPgSQL.TabIndex = 32;
            // 
            // tableSelectionControlPgSQL
            // 
            this.tableSelectionControlPgSQL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableSelectionControlPgSQL.Location = new System.Drawing.Point(422, 238);
            this.tableSelectionControlPgSQL.Name = "tableSelectionControlPgSQL";
            this.tableSelectionControlPgSQL.Size = new System.Drawing.Size(574, 199);
            this.tableSelectionControlPgSQL.TabIndex = 31;
            // 
            // authSelectionControlPgSQL
            // 
            this.authSelectionControlPgSQL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.authSelectionControlPgSQL.Location = new System.Drawing.Point(422, 54);
            this.authSelectionControlPgSQL.Name = "authSelectionControlPgSQL";
            this.authSelectionControlPgSQL.Size = new System.Drawing.Size(574, 178);
            this.authSelectionControlPgSQL.TabIndex = 30;
            // 
            // pgSQLGenerateButton
            // 
            this.pgSQLGenerateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(60)))), ((int)(((byte)(81)))));
            this.pgSQLGenerateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pgSQLGenerateButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.pgSQLGenerateButton.ForeColor = System.Drawing.Color.Transparent;
            this.pgSQLGenerateButton.Location = new System.Drawing.Point(860, 552);
            this.pgSQLGenerateButton.Name = "pgSQLGenerateButton";
            this.pgSQLGenerateButton.Size = new System.Drawing.Size(136, 94);
            this.pgSQLGenerateButton.TabIndex = 28;
            this.pgSQLGenerateButton.Text = "Generate";
            this.pgSQLGenerateButton.UseVisualStyleBackColor = false;
            this.pgSQLGenerateButton.Click += new System.EventHandler(this.automatorGenerateButton_Click);
            // 
            // panel8
            // 
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.multiTenantCheckBoxPgSQL);
            this.panel8.Controls.Add(this.testConPgSQLButton);
            this.panel8.Controls.Add(this.passwordPGTextBox);
            this.panel8.Controls.Add(this.usernamePGTextBox);
            this.panel8.Controls.Add(this.schemaNamePGTextBox);
            this.panel8.Controls.Add(this.dbNamePGTextBox);
            this.panel8.Controls.Add(this.portPGTextbox);
            this.panel8.Controls.Add(this.hostPGTextBox);
            this.panel8.Controls.Add(this.label9);
            this.panel8.Controls.Add(this.label10);
            this.panel8.Controls.Add(this.label11);
            this.panel8.Controls.Add(this.label12);
            this.panel8.Controls.Add(this.label13);
            this.panel8.Controls.Add(this.label14);
            this.panel8.Controls.Add(this.label15);
            this.panel8.Location = new System.Drawing.Point(8, 54);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(398, 383);
            this.panel8.TabIndex = 21;
            // 
            // multiTenantCheckBoxPgSQL
            // 
            this.multiTenantCheckBoxPgSQL.AutoSize = true;
            this.multiTenantCheckBoxPgSQL.Location = new System.Drawing.Point(167, 275);
            this.multiTenantCheckBoxPgSQL.Name = "multiTenantCheckBoxPgSQL";
            this.multiTenantCheckBoxPgSQL.Size = new System.Drawing.Size(94, 19);
            this.multiTenantCheckBoxPgSQL.TabIndex = 21;
            this.multiTenantCheckBoxPgSQL.Text = "Multi-Tenant";
            this.multiTenantCheckBoxPgSQL.UseVisualStyleBackColor = true;
            // 
            // testConPgSQLButton
            // 
            this.testConPgSQLButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(60)))), ((int)(((byte)(81)))));
            this.testConPgSQLButton.FlatAppearance.BorderColor = System.Drawing.Color.MediumOrchid;
            this.testConPgSQLButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.testConPgSQLButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.testConPgSQLButton.ForeColor = System.Drawing.Color.White;
            this.testConPgSQLButton.Location = new System.Drawing.Point(121, 306);
            this.testConPgSQLButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.testConPgSQLButton.Name = "testConPgSQLButton";
            this.testConPgSQLButton.Size = new System.Drawing.Size(242, 53);
            this.testConPgSQLButton.TabIndex = 20;
            this.testConPgSQLButton.Text = "Test Connection";
            this.testConPgSQLButton.UseVisualStyleBackColor = false;
            this.testConPgSQLButton.Click += new System.EventHandler(this.testConPgSQLButton_Click);
            // 
            // passwordPGTextBox
            // 
            this.passwordPGTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordPGTextBox.Location = new System.Drawing.Point(121, 237);
            this.passwordPGTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.passwordPGTextBox.Name = "passwordPGTextBox";
            this.passwordPGTextBox.Size = new System.Drawing.Size(242, 23);
            this.passwordPGTextBox.TabIndex = 14;
            // 
            // usernamePGTextBox
            // 
            this.usernamePGTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.usernamePGTextBox.Location = new System.Drawing.Point(121, 200);
            this.usernamePGTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.usernamePGTextBox.Name = "usernamePGTextBox";
            this.usernamePGTextBox.Size = new System.Drawing.Size(242, 23);
            this.usernamePGTextBox.TabIndex = 15;
            this.usernamePGTextBox.Text = "postgres";
            // 
            // schemaNamePGTextBox
            // 
            this.schemaNamePGTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.schemaNamePGTextBox.Location = new System.Drawing.Point(121, 162);
            this.schemaNamePGTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.schemaNamePGTextBox.Name = "schemaNamePGTextBox";
            this.schemaNamePGTextBox.Size = new System.Drawing.Size(242, 23);
            this.schemaNamePGTextBox.TabIndex = 16;
            this.schemaNamePGTextBox.Text = "public";
            // 
            // dbNamePGTextBox
            // 
            this.dbNamePGTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dbNamePGTextBox.Location = new System.Drawing.Point(121, 124);
            this.dbNamePGTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dbNamePGTextBox.Name = "dbNamePGTextBox";
            this.dbNamePGTextBox.Size = new System.Drawing.Size(242, 23);
            this.dbNamePGTextBox.TabIndex = 17;
            // 
            // portPGTextbox
            // 
            this.portPGTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.portPGTextbox.Location = new System.Drawing.Point(121, 88);
            this.portPGTextbox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.portPGTextbox.Name = "portPGTextbox";
            this.portPGTextbox.Size = new System.Drawing.Size(242, 23);
            this.portPGTextbox.TabIndex = 18;
            this.portPGTextbox.Text = "5432";
            // 
            // hostPGTextBox
            // 
            this.hostPGTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hostPGTextBox.Location = new System.Drawing.Point(121, 50);
            this.hostPGTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.hostPGTextBox.Name = "hostPGTextBox";
            this.hostPGTextBox.Size = new System.Drawing.Size(242, 23);
            this.hostPGTextBox.TabIndex = 19;
            this.hostPGTextBox.Text = "127.0.0.1";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 237);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 15);
            this.label9.TabIndex = 8;
            this.label9.Text = "Password";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 200);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 15);
            this.label10.TabIndex = 9;
            this.label10.Text = "Username";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 162);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(84, 15);
            this.label11.TabIndex = 10;
            this.label11.Text = "Schema Name";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 124);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 15);
            this.label12.TabIndex = 11;
            this.label12.Text = "DB Name";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 88);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 15);
            this.label13.TabIndex = 12;
            this.label13.Text = "Port";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 50);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 15);
            this.label14.TabIndex = 13;
            this.label14.Text = "Host";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(168, 12);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(68, 15);
            this.label15.TabIndex = 7;
            this.label15.Text = "postgreSQL";
            // 
            // communityTab
            // 
            this.communityTab.Location = new System.Drawing.Point(4, 27);
            this.communityTab.Name = "communityTab";
            this.communityTab.Padding = new System.Windows.Forms.Padding(3);
            this.communityTab.Size = new System.Drawing.Size(1549, 737);
            this.communityTab.TabIndex = 2;
            this.communityTab.Text = "Community";
            this.communityTab.UseVisualStyleBackColor = true;
            // 
            // docsTab
            // 
            this.docsTab.Location = new System.Drawing.Point(4, 27);
            this.docsTab.Name = "docsTab";
            this.docsTab.Size = new System.Drawing.Size(1549, 737);
            this.docsTab.TabIndex = 3;
            this.docsTab.Text = "Documentation";
            this.docsTab.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            // 
            // refreshButton
            // 
            this.refreshButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(60)))), ((int)(((byte)(81)))));
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.refreshButton.ForeColor = System.Drawing.Color.Transparent;
            this.refreshButton.Location = new System.Drawing.Point(166, 221);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(168, 42);
            this.refreshButton.TabIndex = 13;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1557, 768);
            this.Controls.Add(this.tabControl1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Automator : Your Trusted Code Generator - Nishant Kumar Verma [Getautomator.com]";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.tabControl1.ResumeLayout(false);
            this.licenceTab.ResumeLayout(false);
            this.licenceTab.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.licenceDataGridView)).EndInit();
            this.mysqlTab.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.postgresqlTab.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage licenceTab;
        private System.Windows.Forms.TabPage mysqlTab;
        private System.Windows.Forms.TabPage communityTab;
        private System.Windows.Forms.TabPage docsTab;
        private System.Windows.Forms.TabPage msSqlTab;
        private System.Windows.Forms.TabPage mongodbTab;
        private System.Windows.Forms.TabPage postgresqlTab;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView licenceDataGridView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox purchaseCodeTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox emailTextbox;
        private System.Windows.Forms.ComboBox sourceComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox systemTextbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button registerBtn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox productComboBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button mysqlGenerateButton;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button testConPgSQLButton;
        private System.Windows.Forms.TextBox passwordPGTextBox;
        private System.Windows.Forms.TextBox usernamePGTextBox;
        private System.Windows.Forms.TextBox schemaNamePGTextBox;
        private System.Windows.Forms.TextBox dbNamePGTextBox;
        private System.Windows.Forms.TextBox portPGTextbox;
        private System.Windows.Forms.TextBox hostPGTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox passwordMysqlTextBox;
        private System.Windows.Forms.TextBox usernameMysqlTextBox;
        private System.Windows.Forms.TextBox dbNameMysqlTextBox;
        private System.Windows.Forms.TextBox portMysqlTextbox;
        private System.Windows.Forms.TextBox hostMysqlTextBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button pgSQLGenerateButton;
        private AuthSelectionControl authSelectionControlPgSQL;
        private TableSelectionControl tableSelectionControlPgSQL;
        private AdminPanelPermissionControlPanel adminPanelPermissionControlPanelPgSQL;
        private TableSelectionControl tableSelectionControl2;
        private AuthSelectionControl authSelectionControl2;
        private AdminPanelPermissionControlPanel adminPanelPermissionControlPanel2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private LogTextAreaControl logTextAreaControlpgSQL;
        private LogTextAreaControl logTextAreaControl2;
        private ProjectNameControl projectNameControlPgSQL;
        private ProjectNameControl projectNameControlMySql;
        private System.Windows.Forms.CheckBox multiTenantCheckBoxPgSQL;
        private System.Windows.Forms.Button refreshButton;
    }
}