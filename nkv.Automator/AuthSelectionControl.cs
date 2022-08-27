using nkv.Automator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nkv.Automator
{
    public partial class AuthSelectionControl : UserControl
    {
        public AuthSelectionControl()
        {
            InitializeComponent();
        }
        public event EventHandler AuthTableSelectionChanged;
        public string AdminUsername { get { return adminUsernameTextBox.Text.Trim(); } }
        public string AdminPassword { get { return adminPasswordTextbox.Text.Trim(); } }
        public string AuthTableName { get { return (string)authTableComboBox.SelectedItem; } }
        public string AuthUserColumnName { get { return ((ColumnModel)authUserColumnComboBox.SelectedItem).Field; } }
        public string AuthPasswordColumnName { get { return ((ColumnModel)authPasswordColumnCoumboBox.SelectedItem).Field; } }
        public bool IsSkipAuth { get { return authSkipCheckBox.Checked; } }
        public bool IsEmail { get { return IsEmailCheckBox.Checked; } }
        public void SetUserAndPasswordColumn(List<ColumnModel> columns)
        {
            authUserColumnComboBox.DataSource = null;
            authUserColumnComboBox.DataSource = new List<ColumnModel>(columns);
            authUserColumnComboBox.DisplayMember = "Field";
            authPasswordColumnCoumboBox.DataSource = null;
            authPasswordColumnCoumboBox.DataSource = new List<ColumnModel>(columns);
            authPasswordColumnCoumboBox.DisplayMember = "Field";
        }
        public void SetTableList(List<string> tables)
        {
            authTableComboBox.DataSource = null;
            authTableComboBox.DataSource = tables;
        }

        private void authSkipCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAuthControls();
        }
        private void EnableDisableAuthControls()
        {
            adminUsernameTextBox.Enabled = IsSkipAuth;
            adminPasswordTextbox.Enabled = IsSkipAuth;
            authTableComboBox.Enabled = !IsSkipAuth;
            authUserColumnComboBox.Enabled = !IsSkipAuth;
            authPasswordColumnCoumboBox.Enabled = !IsSkipAuth;
        }

        private void authTableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AuthTableSelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
