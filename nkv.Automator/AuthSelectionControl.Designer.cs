namespace nkv.Automator
{
    partial class AuthSelectionControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.adminPasswordTextbox = new System.Windows.Forms.TextBox();
            this.orLabel = new System.Windows.Forms.Label();
            this.adminUsernameTextBox = new System.Windows.Forms.TextBox();
            this.authPasswordColumnCoumboBox = new System.Windows.Forms.ComboBox();
            this.authUserColumnComboBox = new System.Windows.Forms.ComboBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.authTableComboBox = new System.Windows.Forms.ComboBox();
            this.label43 = new System.Windows.Forms.Label();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.authSkipCheckBox = new System.Windows.Forms.CheckBox();
            this.IsEmailCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // adminPasswordTextbox
            // 
            this.adminPasswordTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.adminPasswordTextbox.Location = new System.Drawing.Point(375, 47);
            this.adminPasswordTextbox.Name = "adminPasswordTextbox";
            this.adminPasswordTextbox.Size = new System.Drawing.Size(164, 23);
            this.adminPasswordTextbox.TabIndex = 35;
            this.adminPasswordTextbox.Text = "admin123";
            // 
            // orLabel
            // 
            this.orLabel.AutoSize = true;
            this.orLabel.Location = new System.Drawing.Point(255, 82);
            this.orLabel.Name = "orLabel";
            this.orLabel.Size = new System.Drawing.Size(23, 15);
            this.orLabel.TabIndex = 31;
            this.orLabel.Text = "OR";
            // 
            // adminUsernameTextBox
            // 
            this.adminUsernameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.adminUsernameTextBox.Location = new System.Drawing.Point(114, 47);
            this.adminUsernameTextBox.Name = "adminUsernameTextBox";
            this.adminUsernameTextBox.Size = new System.Drawing.Size(164, 23);
            this.adminUsernameTextBox.TabIndex = 34;
            this.adminUsernameTextBox.Text = "admin";
            // 
            // authPasswordColumnCoumboBox
            // 
            this.authPasswordColumnCoumboBox.Enabled = false;
            this.authPasswordColumnCoumboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.authPasswordColumnCoumboBox.FormattingEnabled = true;
            this.authPasswordColumnCoumboBox.Location = new System.Drawing.Point(401, 132);
            this.authPasswordColumnCoumboBox.Name = "authPasswordColumnCoumboBox";
            this.authPasswordColumnCoumboBox.Size = new System.Drawing.Size(154, 23);
            this.authPasswordColumnCoumboBox.TabIndex = 29;
            // 
            // authUserColumnComboBox
            // 
            this.authUserColumnComboBox.Enabled = false;
            this.authUserColumnComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.authUserColumnComboBox.FormattingEnabled = true;
            this.authUserColumnComboBox.Location = new System.Drawing.Point(214, 134);
            this.authUserColumnComboBox.Name = "authUserColumnComboBox";
            this.authUserColumnComboBox.Size = new System.Drawing.Size(154, 23);
            this.authUserColumnComboBox.TabIndex = 28;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(304, 50);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(57, 15);
            this.passwordLabel.TabIndex = 33;
            this.passwordLabel.Text = "Password";
            // 
            // authTableComboBox
            // 
            this.authTableComboBox.Enabled = false;
            this.authTableComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.authTableComboBox.FormattingEnabled = true;
            this.authTableComboBox.Location = new System.Drawing.Point(25, 132);
            this.authTableComboBox.Name = "authTableComboBox";
            this.authTableComboBox.Size = new System.Drawing.Size(154, 23);
            this.authTableComboBox.TabIndex = 27;
            this.authTableComboBox.SelectedIndexChanged += new System.EventHandler(this.authTableComboBox_SelectedIndexChanged);
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(401, 108);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(103, 15);
            this.label43.TabIndex = 26;
            this.label43.Text = "Password Column";
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(31, 50);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(60, 15);
            this.usernameLabel.TabIndex = 32;
            this.usernameLabel.Text = "Username";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(214, 110);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(106, 15);
            this.label45.TabIndex = 25;
            this.label45.Text = "Username Column";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(25, 108);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(116, 15);
            this.label46.TabIndex = 24;
            this.label46.Text = "Authentication Table";
            // 
            // authSkipCheckBox
            // 
            this.authSkipCheckBox.AutoSize = true;
            this.authSkipCheckBox.Checked = true;
            this.authSkipCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.authSkipCheckBox.Location = new System.Drawing.Point(105, 13);
            this.authSkipCheckBox.Name = "authSkipCheckBox";
            this.authSkipCheckBox.Size = new System.Drawing.Size(345, 19);
            this.authSkipCheckBox.TabIndex = 30;
            this.authSkipCheckBox.Text = "I want to use static username/password for token generation";
            this.authSkipCheckBox.UseVisualStyleBackColor = true;
            this.authSkipCheckBox.CheckedChanged += new System.EventHandler(this.authSkipCheckBox_CheckedChanged);
            // 
            // IsEmailCheckBox
            // 
            this.IsEmailCheckBox.AutoSize = true;
            this.IsEmailCheckBox.Location = new System.Drawing.Point(326, 109);
            this.IsEmailCheckBox.Name = "IsEmailCheckBox";
            this.IsEmailCheckBox.Size = new System.Drawing.Size(63, 19);
            this.IsEmailCheckBox.TabIndex = 36;
            this.IsEmailCheckBox.Text = "IsEmail";
            this.IsEmailCheckBox.UseVisualStyleBackColor = true;
            // 
            // AuthSelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.IsEmailCheckBox);
            this.Controls.Add(this.adminPasswordTextbox);
            this.Controls.Add(this.orLabel);
            this.Controls.Add(this.adminUsernameTextBox);
            this.Controls.Add(this.authPasswordColumnCoumboBox);
            this.Controls.Add(this.authUserColumnComboBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.authTableComboBox);
            this.Controls.Add(this.label43);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.label45);
            this.Controls.Add(this.label46);
            this.Controls.Add(this.authSkipCheckBox);
            this.Name = "AuthSelectionControl";
            this.Size = new System.Drawing.Size(570, 172);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox adminPasswordTextbox;
        private System.Windows.Forms.Label orLabel;
        private System.Windows.Forms.TextBox adminUsernameTextBox;
        private System.Windows.Forms.ComboBox authPasswordColumnCoumboBox;
        private System.Windows.Forms.ComboBox authUserColumnComboBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.ComboBox authTableComboBox;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.CheckBox authSkipCheckBox;
        private System.Windows.Forms.CheckBox IsEmailCheckBox;
    }
}
