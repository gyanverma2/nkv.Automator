using nkv.Automator.Models;
using nkv.Automator.PGSQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace nkv.Automator
{
    public partial class Form1 : Form
    {
        private readonly DataTypeEnum ActiveDataType = DataTypeEnum.PostgreSQL;
        private readonly ProductEnum ActiveProduct = ProductEnum.PGSQL_PHPAPI;
        private readonly string SoftwareVersion = "2.0.0";
        private readonly bool isAdminPanel = false;
        Validator v { get; set; } = null!;
        List<LicenceProductModel> LicenceProductList { get; set; } = null!;
        LicenceProductModel? ActiveLicence { get; set; } = null;
        List<ProductModel> AllProductList { get; set; } = null!;
        LogTextAreaControl LogTextAreaControl { get; set; } = null!;
        AuthSelectionControl AuthSelectionControl { get; set; } = null!;
        TableSelectionControl TableSelectionControl { get; set; } = null!;
        AdminPanelPermissionControlPanel PermissionControlPanel { get; set; } = null!;
        ProjectNameControl ProjectNameControl { get; set; } = null!;
        DataTypeEnum SelectedDataType { get; set; }
        PGSQLDBHelper pgSQLDb { get; set; } = null!;
        public Form1()
        {
            InitializeComponent();
            HideTabPages();
        }
        private void CreateEmailFile()
        {
            string path = "licence.temp";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var txtFile = File.AppendText(path))
            {
                string contents = emailTextbox.Text;
                txtFile.WriteLine(contents);
            }
        }
        private string ReadEmailFromFile()
        {
            var email = string.Empty;
            string path = "licence.temp";
            if (File.Exists(path))
            {
                email = File.ReadAllText(path);
                emailTextbox.Text = email;
            }
            return email;
        }
        private void Form1_Shown(object sender, System.EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
        private void HideTabPages()
        {
            switch (ActiveDataType)
            {
                case DataTypeEnum.MySQL:
                    tabControl1.TabPages.Remove(mongodbTab);
                    tabControl1.TabPages.Remove(postgresqlTab);
                    tabControl1.TabPages.Remove(msSqlTab);
                    break;
                case DataTypeEnum.MSSQL:
                    tabControl1.TabPages.Remove(mongodbTab);
                    tabControl1.TabPages.Remove(postgresqlTab);
                    tabControl1.TabPages.Remove(mysqlTab);
                    break;
                case DataTypeEnum.PostgreSQL:
                    tabControl1.TabPages.Remove(mongodbTab);
                    tabControl1.TabPages.Remove(msSqlTab);
                    tabControl1.TabPages.Remove(mysqlTab);
                    break;
                case DataTypeEnum.MongoDB:
                    tabControl1.TabPages.Remove(postgresqlTab);
                    tabControl1.TabPages.Remove(msSqlTab);
                    tabControl1.TabPages.Remove(mysqlTab);
                    break;
            }
        }
        private void ShowTabPage(LicenceProductModel? p)
        {
            if (p == null)
                return;
            switch (p.DatabaseTypeId)
            {
                case (int)DataTypeEnum.MySQL:
                    mysqlTab.Text = p.ProductName;
                    break;
                case (int)DataTypeEnum.MSSQL:
                    msSqlTab.Text = p.ProductName;
                    break;
                case (int)DataTypeEnum.PostgreSQL:
                    postgresqlTab.Text = p.ProductName;
                    break;
                case (int)DataTypeEnum.MongoDB:
                    mongodbTab.Text = p.ProductName;
                    break;
            }
        }
        private void InitControls()
        {
            v = new Validator();
            Invoke(new Action(() =>
            {
                var email = ReadEmailFromFile();
                systemTextbox.Text = Helper.GetSystemName();
                if (!string.IsNullOrEmpty(email))
                    DisplayLicenceGrid(Helper.GetSystemName());
                DisplayProductList();
                InitUserControls(ActiveDataType, isAdminPanel);
                sourceComboBox.SelectedIndex = 0;
            }));
        }
        private void InitUserControls(DataTypeEnum dataType, bool? isAdminPanel = null)
        {
            string logTextAreaName = "logTextAreaControlpgSQL";
            string authControlName = "authSelectionControlPgSQL";
            string tableSelectionName = "tableSelectionControlPgSQL";
            string adminPermissionName = "adminPanelPermissionControlPanelPgSQL";
            string projectControlName = "projectNameControlPgSQL";
            switch (dataType)
            {
                case DataTypeEnum.MySQL:
                    break;
                case DataTypeEnum.MSSQL:
                    break;
                case DataTypeEnum.PostgreSQL:
                    logTextAreaName = "logTextAreaControlpgSQL";
                    authControlName = "authSelectionControlPgSQL";
                    tableSelectionName = "tableSelectionControlPgSQL";
                    adminPermissionName = "adminPanelPermissionControlPanelPgSQL";
                    projectControlName = "projectNameControlPgSQL";
                    break;
                case DataTypeEnum.MongoDB:
                    break;
            };

            LogTextAreaControl = (LogTextAreaControl)Controls.Find(logTextAreaName, true).First();
            AuthSelectionControl = (AuthSelectionControl)Controls.Find(authControlName, true).First();
            AuthSelectionControl.AuthTableSelectionChanged += AuthSelectionControl_AuthTableSelectionChanged;
            TableSelectionControl = (TableSelectionControl)Controls.Find(tableSelectionName, true).First();
            PermissionControlPanel = (AdminPanelPermissionControlPanel)Controls.Find(adminPermissionName, true).First();
            ProjectNameControl = (ProjectNameControl)Controls.Find(projectControlName, true).First();
            if (isAdminPanel != null)
                Controls.Find(adminPermissionName, true).First().Visible = (bool)isAdminPanel;
        }

        private void AuthSelectionControl_AuthTableSelectionChanged(object? sender, EventArgs e)
        {
            switch (SelectedDataType)
            {
                case DataTypeEnum.MySQL:
                    break;
                case DataTypeEnum.MSSQL:
                    break;
                case DataTypeEnum.PostgreSQL:
                    AuthSelectionControl.SetUserAndPasswordColumn(pgSQLDb.GetColumns(AuthSelectionControl.AuthTableName));
                    break;
                case DataTypeEnum.MongoDB:
                    break;
            }
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
                    if (LogTextAreaControl != null)
                        LogTextAreaControl.SetLogText(msg, Color.Red, clearPrior);
                    else
                        MessageBox.Show(msg);
                }));


        }
        private void appendSuccess(string msg, bool clearPrior = false)
        {
            Invoke(new Action(() =>
            {
                Invoke(new Action(() =>
                {
                    if (LogTextAreaControl != null)
                        LogTextAreaControl.SetLogText(msg, Color.Green, clearPrior);
                    else
                        MessageBox.Show(msg);
                }));
            }));
        }
        #endregion
        public List<LicenceProductModel> FilterLicence(ProductEnum ActiveProduct)
        {
            var listProduct = LicenceProductList.Where(x => int.Parse(x.ProductNumber) == (int)ActiveProduct).ToList();
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
                LicenceProductList = v.GetAllLicence(new LoginModel() { MacID = systemName, UserEmail = emailTextbox.Text.Trim() });
                if (LicenceProductList != null && LicenceProductList.Count > 0)
                {
                    licenceDataGridView.DataSource = FilterLicence(ActiveProduct);
                    licenceDataGridView.Columns["licenceID"].Visible = false;
                    licenceDataGridView.Columns["ProductNumber"].Visible = false;
                    licenceDataGridView.Columns["MacID"].Visible = false;
                    licenceDataGridView.Columns["ValidTill"].Visible = false;
                    licenceDataGridView.Columns["ProductID"].Visible = false;
                    licenceDataGridView.Columns["ProductTitle"].Visible = false;
                    licenceDataGridView.Columns["DatabaseTypeId"].Visible = false;
                    licenceDataGridView.Columns["DatabaseTypeName"].Visible = false;
                    ActiveLicence = LicenceProductList.Where(i => int.Parse(i.ProductNumber) == (int)ActiveProduct && i.NumberOfDays >= 0).FirstOrDefault();
                    ShowTabPage(ActiveLicence);
                }
                else
                {
                    appendError("No Licence Found, Please register. Or Licence Management at : https://getautomator.com/app");
                }


            }
            catch (Exception ex)
            {
                appendError(ex.Message);
            }

        }
        private void DisplayProductList()
        {
            AllProductList = v.GetAllProduct();
            var productList = AllProductList.Where(i => int.Parse(i.ProductNumber) == (int)ActiveProduct).ToList();
            productComboBox.DisplayMember = "ProductTitle";
            productComboBox.DataSource = productList;
            if (productList.Any())
                productComboBox.SelectedIndex = 0;

        }
        private void automatorGenerateButton_Click(object sender, EventArgs e)
        {
            if (TableSelectionControl.SelectedTableList.Count == 0)
            {
                MessageBox.Show("Please select at least one table to generate.");
                return;
            }
            if (ActiveLicence != null && v.ClickCounter(ActiveLicence.PublicID))
            {
                backgroundWorker2.RunWorkerAsync();
            }
            else
            {
                appendError("Please vaidate your licence or register new to generate! Licence Management at https://getautomator.com/app");
                MessageBox.Show("Invalid Licence!");
                return;
            }
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Invoke(new Action(() =>
            {
                NKVConfiguration config = new NKVConfiguration()
                {
                    AuthTableConfig = new NKVAuthTableConfig
                    {

                        IsSkipAuth = AuthSelectionControl.IsSkipAuth,
                        AuthTableName = AuthSelectionControl.IsSkipAuth ? "" : AuthSelectionControl.AuthTableName,
                        UsernameColumnName = AuthSelectionControl.IsSkipAuth ? AuthSelectionControl.AdminUsername : AuthSelectionControl.AuthUserColumnName,
                        PasswordColumnName = AuthSelectionControl.IsSkipAuth ? AuthSelectionControl.AdminPassword : AuthSelectionControl.AuthPasswordColumnName,
                    },
                    AdminPanelConfig = new NKVAdminPanelPermissionConfig()
                    {
                        AdminPassword = PermissionControlPanel.AdminPassword,
                        AdminUsername = PermissionControlPanel.AdminUsername,
                        GuestPassword = PermissionControlPanel.GuestPassword,
                        GuestUsername = PermissionControlPanel.GuestUsername,
                        SuperAdminPassword = PermissionControlPanel.SuperAdminPassword,
                        SuperAdminUsername = PermissionControlPanel.SuperAdminUsername,
                        FileColumns = PermissionControlPanel.FileColumns,
                        ImageColumns = PermissionControlPanel.ImageColumns
                    }
                };
                switch (SelectedDataType)
                {
                    case DataTypeEnum.MySQL:
                        break;
                    case DataTypeEnum.MSSQL:
                        break;
                    case DataTypeEnum.PostgreSQL:
                        Process(config);
                        break;
                    case DataTypeEnum.MongoDB:
                        break;
                }
            }));
        }
        private void Process(NKVConfiguration config)
        {
            switch (ActiveProduct)
            {
                case ProductEnum.PGSQL_PHPAPI:
                    var pgSQLDb = new PGSQLDBHelper(hostPGTextBox.Text.Trim(), schemaNamePGTextBox.Text.Trim(), portPGTextbox.Text.Trim(), usernamePGTextBox.Text.Trim(), passwordPGTextBox.Text.Trim(), dbNamePGTextBox.Text.Trim());
                    if (pgSQLDb.Connect())
                    {
                        PGSQL_PHPAPI pgPHPAPI = new PGSQL_PHPAPI(config, multiTenantCheckBoxPgSQL.Checked, "//");
                        pgPHPAPI.MessageEvent += MessageEvent;
                        pgPHPAPI.CompletedEvent += CompletedEvent;
                        pgPHPAPI.Automator(ProjectNameControl.ProjectName, TableSelectionControl.SelectedTableList, pgSQLDb);
                    }
                    else
                    {
                        MessageBox.Show("Database Connection Failed, Please click on Test Connection to validate");
                    }
                    break;
            }
        }

        private void CompletedEvent(NKVMessage obj)
        {
            if (obj.IsSuccess)
                appendSuccess(obj.Message);
            else
                appendError(obj.Message);
            MessageBox.Show("Task Completed! Please check the generated project folder.");
        }

        private void MessageEvent(NKVMessage obj)
        {
            if (obj.IsSuccess)
                appendSuccess(obj.Message);
            else
                appendError(obj.Message);
        }

        private void testConPgSQLButton_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedDataType = DataTypeEnum.PostgreSQL;

                pgSQLDb = new PGSQLDBHelper(hostPGTextBox.Text.Trim(), schemaNamePGTextBox.Text.Trim(), portPGTextbox.Text.Trim(), usernamePGTextBox.Text.Trim(), passwordPGTextBox.Text.Trim(), dbNamePGTextBox.Text.Trim());
                if (pgSQLDb.Connect())
                {
                    MessageBox.Show("Connection Successful!");
                    appendSuccess("Connection Successful!", true);
                    var tableList = pgSQLDb.GetTables();
                    AuthSelectionControl.SetTableList(tableList);
                    TableSelectionControl.SetTableList(tableList);
                };
            }
            catch (Exception ex)
            {
                appendError(ex.Message, true);
            }
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {

            if (sourceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select purchase source");
                return;
            }
            if (string.IsNullOrEmpty(purchaseCodeTextbox.Text.Trim()))
            {
                MessageBox.Show("Please provide purchase code");
                return;
            }
            if (!Helper.IsValidEmail(emailTextbox.Text.Trim()))
            {
                MessageBox.Show("Please enter valid email");
                return;
            }
            registerBtn.Enabled = false;
            if (sourceComboBox.SelectedItem != null && productComboBox.SelectedItem != null)
            {
                var product = (ProductModel)productComboBox.SelectedItem;
                var registerData = new RegisterModel()
                {

                    LicenceNumber = purchaseCodeTextbox.Text.Trim(),
                    MacID = Helper.GetSystemName(),
                    SoftwareVersion = SoftwareVersion,
                    Source = sourceComboBox.SelectedItem.ToString(),
                    ToolName = product.ProductName,
                    UserEmail = emailTextbox.Text.Trim()
                };
                if (v.Register(registerData))
                {
                    CreateEmailFile();
                    appendSuccess("Thanks for registration!");
                    DisplayLicenceGrid(Helper.GetSystemName());
                }
                else
                {
                    appendError("Invalid licence, Please contact support or manage your licence at https://getautomator.com/app", true);
                }
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
    }
}