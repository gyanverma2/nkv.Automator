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
            this.msSqlTab = new System.Windows.Forms.TabPage();
            this.mongodbTab = new System.Windows.Forms.TabPage();
            this.postgresqlTab = new System.Windows.Forms.TabPage();
            this.communityTab = new System.Windows.Forms.TabPage();
            this.docsTab = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.licenceTab.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.licenceDataGridView)).BeginInit();
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
            this.tabControl1.Size = new System.Drawing.Size(1265, 722);
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
            this.licenceTab.Size = new System.Drawing.Size(1257, 691);
            this.licenceTab.TabIndex = 0;
            this.licenceTab.Text = "Licence";
            this.licenceTab.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            this.label6.Location = new System.Drawing.Point(76, 189);
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
            this.productComboBox.TabIndex = 11;
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
            this.registerBtn.TabIndex = 10;
            this.registerBtn.Text = "Register";
            this.registerBtn.UseVisualStyleBackColor = false;
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
            this.sourceComboBox.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(76, 132);
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
            this.systemTextbox.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(636, 132);
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
            this.emailTextbox.TabIndex = 3;
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
            this.purchaseCodeTextbox.TabIndex = 0;
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
            this.mysqlTab.Location = new System.Drawing.Point(4, 27);
            this.mysqlTab.Name = "mysqlTab";
            this.mysqlTab.Padding = new System.Windows.Forms.Padding(3);
            this.mysqlTab.Size = new System.Drawing.Size(1257, 691);
            this.mysqlTab.TabIndex = 1;
            this.mysqlTab.Text = "MySQL";
            this.mysqlTab.UseVisualStyleBackColor = true;
            // 
            // msSqlTab
            // 
            this.msSqlTab.Location = new System.Drawing.Point(4, 27);
            this.msSqlTab.Name = "msSqlTab";
            this.msSqlTab.Size = new System.Drawing.Size(1257, 691);
            this.msSqlTab.TabIndex = 4;
            this.msSqlTab.Text = "MS SQL";
            this.msSqlTab.UseVisualStyleBackColor = true;
            // 
            // mongodbTab
            // 
            this.mongodbTab.Location = new System.Drawing.Point(4, 27);
            this.mongodbTab.Name = "mongodbTab";
            this.mongodbTab.Size = new System.Drawing.Size(1257, 691);
            this.mongodbTab.TabIndex = 6;
            this.mongodbTab.Text = "MongoDB";
            this.mongodbTab.UseVisualStyleBackColor = true;
            // 
            // postgresqlTab
            // 
            this.postgresqlTab.Location = new System.Drawing.Point(4, 27);
            this.postgresqlTab.Name = "postgresqlTab";
            this.postgresqlTab.Size = new System.Drawing.Size(1257, 691);
            this.postgresqlTab.TabIndex = 5;
            this.postgresqlTab.Text = "PostgreSQL";
            this.postgresqlTab.UseVisualStyleBackColor = true;
            // 
            // communityTab
            // 
            this.communityTab.Location = new System.Drawing.Point(4, 27);
            this.communityTab.Name = "communityTab";
            this.communityTab.Padding = new System.Windows.Forms.Padding(3);
            this.communityTab.Size = new System.Drawing.Size(1257, 691);
            this.communityTab.TabIndex = 2;
            this.communityTab.Text = "Community";
            this.communityTab.UseVisualStyleBackColor = true;
            // 
            // docsTab
            // 
            this.docsTab.Location = new System.Drawing.Point(4, 27);
            this.docsTab.Name = "docsTab";
            this.docsTab.Size = new System.Drawing.Size(1257, 691);
            this.docsTab.TabIndex = 3;
            this.docsTab.Text = "Documentation";
            this.docsTab.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 722);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "NKV Automator : Your Trusted Code Generator";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.tabControl1.ResumeLayout(false);
            this.licenceTab.ResumeLayout(false);
            this.licenceTab.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.licenceDataGridView)).EndInit();
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
    }
}