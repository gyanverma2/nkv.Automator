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
    public partial class AdminPanelPermissionControlPanel : UserControl
    {
        public AdminPanelPermissionControlPanel()
        {
            InitializeComponent();
        }
        public string SuperAdminUsername { get { return superAdminUsernameTextBox.Text.Trim(); } }
        public string SuperAdminPassword { get { return superAdminPasswordTextBox.Text.Trim(); } }
        public string AdminUsername { get { return adminUsernameTextBox.Text.Trim(); } }
        public string AdminPassword{ get { return adminPasswordTextbox.Text.Trim(); } }
        public string GuestUsername { get { return guestUsernameTextBox.Text.Trim(); } }
        public string GuestPassword { get { return guestPasswordTextbox.Text.Trim(); } }
        public List<string> ImageColumns { get { return imageColumnTextbox.Text.Trim().Split(',').ToList(); } }
        public List<string> FileColumns { get { return fileColumnTextbox.Text.Trim().Split(',').ToList(); } }
    }
}
