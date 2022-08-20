using nkv.Automator.Models;
using nkv.Automator.PGSQL;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private void Form1_Shown(object sender, System.EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
        private void InitControls()
        {
            v = new Validator();
            Invoke(new Action(() =>
            {
                systemTextbox.Text = Helper.GetSystemName();
                DisplayLicenceGrid(Helper.GetSystemName());
                DisplayProductList();
            }));
            

        }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            InitControls();
        }
        #region UIHelper
        private void appendError(string msg, bool clearPrior = false)
        {
            
                Invoke(new Action(() =>
                {
                    if (clearPrior)
                    {
                        richTextBox1.Clear();
                    }

                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.SelectedText = " " + msg + "\r\n";
                    richTextBox1.Update();
                }));
            

        }
        private void appendSuccess(string msg, bool clearPrior = false)
        {
            Invoke(new Action(() =>
            {
                if (clearPrior)
                {
                    richTextBox1.Clear();
                }

                richTextBox1.SelectionColor = Color.Green;
                richTextBox1.SelectedText = " " + msg + "\r\n";
                richTextBox1.Update();
            }));
        }
        #endregion
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
            try
            {
                LicenceProductList = v.GetAllLicence(new LoginModel() { MacID = "DESKTOP-KS8GISK", UserEmail = "nisgyan@gmail.com" });
                if (LicenceProductList!=null && LicenceProductList.Count > 0)
                {
                    licenceDataGridView.DataSource = LicenceProductList;
                    licenceDataGridView.Columns["licenceID"].Visible = false;
                    licenceDataGridView.Columns["ProductNumber"].Visible = false;
                    licenceDataGridView.Columns["MacID"].Visible = false;
                    licenceDataGridView.Columns["ValidTill"].Visible = false;
                    licenceDataGridView.Columns["ProductID"].Visible = false;
                    licenceDataGridView.Columns["ProductTitle"].Visible = false;
                    licenceDataGridView.Columns["DatabaseTypeId"].Visible = false;
                    licenceDataGridView.Columns["DatabaseTypeName"].Visible = false;
                }
                else
                {
                    appendError("No Licence Found, Please register. Licence Management at : https://getautomator.com/app" );                }

                mysqlToolComboBox.DataSource = FilterLicence(DataTypeEnum.MySQL);
                mysqlToolComboBox.DisplayMember = "ProductTitle";
            }catch(Exception ex)
            {
                appendError(ex.Message);
            }

        }
        private void DisplayProductList()
        {
            AllProductList = v.GetAllProduct();
            productComboBox.DisplayMember = "ProductTitle";
            productComboBox.DataSource = AllProductList;
        }

        private void pgSQLGenerateButton_Click(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void testConPgSQLButton_Click(object sender, EventArgs e)
        {
            try
            {
                var p = new PGSQLDBHelper(hostPGTextBox.Text.Trim(), schemaNamePGTextBox.Text.Trim(), portPGTextbox.Text.Trim(), usernamePGTextBox.Text.Trim(), passwordPGTextBox.Text.Trim(), dbNamePGTextBox.Text.Trim());
                if (p.Connect())
                {
                    MessageBox.Show("Connection Successful!");
                    appendSuccess("Connection Successful!", true);
                };
            }catch(Exception ex)
            {
                appendError(ex.Message,true);
            }
        }
    }
}