namespace nkv.Automator
{
    partial class TableSelectionControl
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
            this.selectAllTableCheckBox = new System.Windows.Forms.CheckBox();
            this.removeFromSelectedTableButton = new System.Windows.Forms.Button();
            this.addToSelectedTableButton = new System.Windows.Forms.Button();
            this.SelectedTableListBox = new System.Windows.Forms.ListBox();
            this.tableListBox = new System.Windows.Forms.ListBox();
            this.label40 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // selectAllTableCheckBox
            // 
            this.selectAllTableCheckBox.AutoSize = true;
            this.selectAllTableCheckBox.Location = new System.Drawing.Point(234, 89);
            this.selectAllTableCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.selectAllTableCheckBox.Name = "selectAllTableCheckBox";
            this.selectAllTableCheckBox.Size = new System.Drawing.Size(109, 19);
            this.selectAllTableCheckBox.TabIndex = 35;
            this.selectAllTableCheckBox.Text = "Select All Tables";
            this.selectAllTableCheckBox.UseVisualStyleBackColor = true;
            this.selectAllTableCheckBox.CheckedChanged += new System.EventHandler(this.selectAllTableCheckBox_CheckedChanged);
            // 
            // removeFromSelectedTableButton
            // 
            this.removeFromSelectedTableButton.Location = new System.Drawing.Point(234, 128);
            this.removeFromSelectedTableButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.removeFromSelectedTableButton.Name = "removeFromSelectedTableButton";
            this.removeFromSelectedTableButton.Size = new System.Drawing.Size(92, 22);
            this.removeFromSelectedTableButton.TabIndex = 34;
            this.removeFromSelectedTableButton.Text = "<< Remove";
            this.removeFromSelectedTableButton.UseVisualStyleBackColor = true;
            this.removeFromSelectedTableButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addToSelectedTableButton
            // 
            this.addToSelectedTableButton.Location = new System.Drawing.Point(234, 49);
            this.addToSelectedTableButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.addToSelectedTableButton.Name = "addToSelectedTableButton";
            this.addToSelectedTableButton.Size = new System.Drawing.Size(82, 22);
            this.addToSelectedTableButton.TabIndex = 33;
            this.addToSelectedTableButton.Text = "Add >>";
            this.addToSelectedTableButton.UseVisualStyleBackColor = true;
            this.addToSelectedTableButton.Click += new System.EventHandler(this.addSelectionButton_Click);
            // 
            // SelectedTableListBox
            // 
            this.SelectedTableListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectedTableListBox.FormattingEnabled = true;
            this.SelectedTableListBox.ItemHeight = 15;
            this.SelectedTableListBox.Location = new System.Drawing.Point(368, 26);
            this.SelectedTableListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SelectedTableListBox.Name = "SelectedTableListBox";
            this.SelectedTableListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.SelectedTableListBox.Size = new System.Drawing.Size(181, 152);
            this.SelectedTableListBox.TabIndex = 32;
            // 
            // tableListBox
            // 
            this.tableListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableListBox.FormattingEnabled = true;
            this.tableListBox.ItemHeight = 15;
            this.tableListBox.Location = new System.Drawing.Point(30, 26);
            this.tableListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableListBox.Name = "tableListBox";
            this.tableListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.tableListBox.Size = new System.Drawing.Size(181, 152);
            this.tableListBox.TabIndex = 31;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label40.Location = new System.Drawing.Point(234, 9);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(79, 15);
            this.label40.TabIndex = 30;
            this.label40.Text = "Select Tables";
            // 
            // TableSelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.selectAllTableCheckBox);
            this.Controls.Add(this.removeFromSelectedTableButton);
            this.Controls.Add(this.addToSelectedTableButton);
            this.Controls.Add(this.SelectedTableListBox);
            this.Controls.Add(this.tableListBox);
            this.Controls.Add(this.label40);
            this.Name = "TableSelectionControl";
            this.Size = new System.Drawing.Size(575, 199);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox selectAllTableCheckBox;
        private System.Windows.Forms.Button removeFromSelectedTableButton;
        private System.Windows.Forms.Button addToSelectedTableButton;
        private System.Windows.Forms.ListBox SelectedTableListBox;
        private System.Windows.Forms.ListBox tableListBox;
        private System.Windows.Forms.Label label40;
    }
}
