using nkv.Automator.Models;
using System.Windows.Forms;

namespace nkv.Automator
{
    public partial class Form1 : Form
    {
        Validator v { get; set; } = null!;
        public Form1()
        {
            InitializeComponent();  
        }
        private void InitControls()
        {
            v = new Validator();
            systemTextbox.Text = Helper.GetSystemName();
            DisplayLicenceGrid(Helper.GetSystemName());


        }
        private void DisplayLicenceGrid(string systemName)
        {
            licenceDataGridView.DataSource = v.GetAllLicence(new LoginModel() { MacID = "DESKTOP-KS8GISK", UserEmail = "nisgyan@gmail.com" });
            licenceDataGridView.Columns["licenceID"].Visible = false;
            licenceDataGridView.Columns["ProductNumber"].Visible = false;
            licenceDataGridView.Columns["MacID"].Visible = false;
            licenceDataGridView.Columns["ValidTill"].Visible = false;
            licenceDataGridView.Columns["ProductID"].Visible = false;
            licenceDataGridView.Columns["ProductTitle"].Visible = false;
            licenceDataGridView.Columns["DatabaseTypeId"].Visible = false;
            licenceDataGridView.Columns["DatabaseTypeName"].Visible = false;
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
            InitControls();
        }
    }
}