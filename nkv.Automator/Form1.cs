using nkv.Automator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace nkv.Automator
{
    public partial class Form1 : Form
    {
        Validator v { get; set; } = null!;
        List<LicenceProductModel> LicenceProductList { get; set; } = null!;
        List<ProductModel> AllProductList { get; set; } = null!;
        public Form1()
        {
            InitializeComponent();
        }
        private void InitControls()
        {
            v = new Validator();
            systemTextbox.Text = Helper.GetSystemName();
            DisplayLicenceGrid(Helper.GetSystemName());
            DisplayProductList();

        }
        public List<LicenceProductModel> FilterLicence(DataTypeEnum dataType)
        {
            var listProduct= LicenceProductList.Where(x => x.DatabaseTypeId == (int)dataType).ToList();
            if (listProduct.Any())
            {
                return listProduct;
            }
            return new List<LicenceProductModel>() { new LicenceProductModel() { ProductID = 0, ProductNumber = "0", ProductTitle = "No Product Licence Found!" } };
        }
        private void DisplayLicenceGrid(string systemName)
        {
            LicenceProductList = v.GetAllLicence(new LoginModel() { MacID = "DESKTOP-KS8GISK", UserEmail = "nisgyan@gmail.com" });
            licenceDataGridView.DataSource = LicenceProductList;
            licenceDataGridView.Columns["licenceID"].Visible = false;
            licenceDataGridView.Columns["ProductNumber"].Visible = false;
            licenceDataGridView.Columns["MacID"].Visible = false;
            licenceDataGridView.Columns["ValidTill"].Visible = false;
            licenceDataGridView.Columns["ProductID"].Visible = false;
            licenceDataGridView.Columns["ProductTitle"].Visible = false;
            licenceDataGridView.Columns["DatabaseTypeId"].Visible = false;
            licenceDataGridView.Columns["DatabaseTypeName"].Visible = false;
            
            mysqlToolComboBox.DataSource = FilterLicence(DataTypeEnum.MySQL);
            mysqlToolComboBox.DisplayMember = "ProductTitle";

        }
        private void DisplayProductList()
        {
            AllProductList = v.GetAllProduct();
            productComboBox.DisplayMember = "ProductTitle";
            productComboBox.DataSource = AllProductList;
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
            InitControls();
        }
    }
}