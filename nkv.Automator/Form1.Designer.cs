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
            this.registerTab = new System.Windows.Forms.TabPage();
            this.mysqlTab = new System.Windows.Forms.TabPage();
            this.communityTab = new System.Windows.Forms.TabPage();
            this.docsTab = new System.Windows.Forms.TabPage();
            this.msSqlTab = new System.Windows.Forms.TabPage();
            this.postgresqlTab = new System.Windows.Forms.TabPage();
            this.mongodbTab = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.registerTab);
            this.tabControl1.Controls.Add(this.mysqlTab);
            this.tabControl1.Controls.Add(this.msSqlTab);
            this.tabControl1.Controls.Add(this.mongodbTab);
            this.tabControl1.Controls.Add(this.postgresqlTab);
            this.tabControl1.Controls.Add(this.communityTab);
            this.tabControl1.Controls.Add(this.docsTab);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1241, 698);
            this.tabControl1.TabIndex = 0;
            // 
            // registerTab
            // 
            this.registerTab.Location = new System.Drawing.Point(4, 24);
            this.registerTab.Name = "registerTab";
            this.registerTab.Padding = new System.Windows.Forms.Padding(3);
            this.registerTab.Size = new System.Drawing.Size(1233, 670);
            this.registerTab.TabIndex = 0;
            this.registerTab.Text = "Register";
            this.registerTab.UseVisualStyleBackColor = true;
            // 
            // mysqlTab
            // 
            this.mysqlTab.Location = new System.Drawing.Point(4, 24);
            this.mysqlTab.Name = "mysqlTab";
            this.mysqlTab.Padding = new System.Windows.Forms.Padding(3);
            this.mysqlTab.Size = new System.Drawing.Size(1233, 670);
            this.mysqlTab.TabIndex = 1;
            this.mysqlTab.Text = "MySQL";
            this.mysqlTab.UseVisualStyleBackColor = true;
            // 
            // communityTab
            // 
            this.communityTab.Location = new System.Drawing.Point(4, 24);
            this.communityTab.Name = "communityTab";
            this.communityTab.Padding = new System.Windows.Forms.Padding(3);
            this.communityTab.Size = new System.Drawing.Size(1233, 670);
            this.communityTab.TabIndex = 2;
            this.communityTab.Text = "Community";
            this.communityTab.UseVisualStyleBackColor = true;
            // 
            // docsTab
            // 
            this.docsTab.Location = new System.Drawing.Point(4, 24);
            this.docsTab.Name = "docsTab";
            this.docsTab.Size = new System.Drawing.Size(1233, 670);
            this.docsTab.TabIndex = 3;
            this.docsTab.Text = "Documentation";
            this.docsTab.UseVisualStyleBackColor = true;
            // 
            // msSqlTab
            // 
            this.msSqlTab.Location = new System.Drawing.Point(4, 24);
            this.msSqlTab.Name = "msSqlTab";
            this.msSqlTab.Size = new System.Drawing.Size(1233, 670);
            this.msSqlTab.TabIndex = 4;
            this.msSqlTab.Text = "MS SQL";
            this.msSqlTab.UseVisualStyleBackColor = true;
            // 
            // postgresqlTab
            // 
            this.postgresqlTab.Location = new System.Drawing.Point(4, 24);
            this.postgresqlTab.Name = "postgresqlTab";
            this.postgresqlTab.Size = new System.Drawing.Size(1233, 670);
            this.postgresqlTab.TabIndex = 5;
            this.postgresqlTab.Text = "PostgreSQL";
            this.postgresqlTab.UseVisualStyleBackColor = true;
            // 
            // mongodbTab
            // 
            this.mongodbTab.Location = new System.Drawing.Point(4, 24);
            this.mongodbTab.Name = "mongodbTab";
            this.mongodbTab.Size = new System.Drawing.Size(1233, 670);
            this.mongodbTab.TabIndex = 6;
            this.mongodbTab.Text = "MongoDB";
            this.mongodbTab.UseVisualStyleBackColor = true;
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
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage registerTab;
        private System.Windows.Forms.TabPage mysqlTab;
        private System.Windows.Forms.TabPage communityTab;
        private System.Windows.Forms.TabPage docsTab;
        private System.Windows.Forms.TabPage msSqlTab;
        private System.Windows.Forms.TabPage mongodbTab;
        private System.Windows.Forms.TabPage postgresqlTab;
    }
}