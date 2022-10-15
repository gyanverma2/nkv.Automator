using nkv.Automator.MySQL;
using nkv.Automator.PGSQL;
using nkv.Automator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using nkv.Automator.Generator.MySQL;
using nkv.Automator.MSSQL;
using nkv.Automator.Generator.MSSQL;

namespace nkv.Automator
{
    public partial class Form1 : Form
    {
        //----- Licence Management Start------//

        private readonly DataTypeEnum ActiveDataType = DataTypeEnum.MSSQL;
        private readonly ProductEnum ActiveProduct = ProductEnum.MSSQL_PHP_API;
        private readonly string SoftwareVersion = "3.0.0 - " + DataTypeEnum.MSSQL.ToString();
        private readonly bool IsAdminPanel = false;
        private readonly bool IsMultiTenant = true;

        //----- Licence Management End------//
        Validator v { get; set; } = null!;
        #region VariableDeclaration
        List<LicenceProductModel> LicenceProductList { get; set; } = null!;
        LicenceProductModel? ActiveLicence { get; set; } = null;
        List<ProductModel> AllProductList { get; set; } = null!;
        LogTextAreaControl LogTextAreaControl { get; set; } = null!;
        AuthSelectionControl AuthSelectionControl { get; set; } = null!;
        TableSelectionControl TableSelectionControl { get; set; } = null!;
        AdminPanelPermissionControlPanel PermissionControlPanel { get; set; } = null!;
        ProjectNameControl ProjectNameControl { get; set; } = null!;
        DataTypeEnum SelectedDataType { get; set; }
        #endregion
        public Form1()
        {
            InitializeComponent();
            HideTabPages();
            labelVersion.Text = "Version: " + SoftwareVersion;
        }
        #region FormInit
        private void CreateEmailFile(string email)
        {
            string path = "licence.temp";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var txtFile = File.AppendText(path))
            {
                string contents = email;
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
            ReadEmailFromFile();
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
        private void InitControls(string emailNew="")
        {
            v = new Validator();
            Invoke(new Action(() =>
            {
                v.MessageEvent += MessageBoxEvent;
                InitUserControls(ActiveDataType, IsAdminPanel, IsMultiTenant);
                systemTextbox.Text = Helper.GetSystemName();
                if (!string.IsNullOrEmpty(emailNew))
                {
                    CreateEmailFile(emailTextbox.Text.Trim());
                }
                DisplayProductList();
                sourceComboBox.SelectedIndex = 0;
                if (!string.IsNullOrEmpty(emailNew))
                    DisplayLicenceGrid(Helper.GetSystemName());
               
            }));
        }
        private void InitUserControls(DataTypeEnum dataType, bool? isAdminPanel = null, bool? isMultiTenant = null)
        {
            string logTextAreaName = "logTextAreaControlpgSQL";
            string authControlName = "authSelectionControlPgSQL";
            string tableSelectionName = "tableSelectionControlPgSQL";
            string adminPermissionName = "adminPanelPermissionControlPanelPgSQL";
            string projectControlName = "projectNameControlPgSQL";
            switch (dataType)
            {
                case DataTypeEnum.MySQL:
                    logTextAreaName = "logTextAreaControlMysql";
                    authControlName = "authSelectionControlMysql";
                    tableSelectionName = "tableSelectionControlMysql";
                    adminPermissionName = "adminPanelPermissionControlPanelMysql";
                    projectControlName = "projectNameControlMySql";
                    break;
                case DataTypeEnum.MSSQL:
                    logTextAreaName = "logTextAreaControlMSSQL";
                    authControlName = "authSelectionControlMSSQL";
                    tableSelectionName = "tableSelectionControlMSSQL";
                    adminPermissionName = "adminPanelPermissionControlPanelMSSQL";
                    projectControlName = "projectNameControlMSSQL";
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
            if (isMultiTenant != null)
                multiTenantCheckBoxMysql.Visible = (bool)isMultiTenant;
            switch (ActiveProduct)
            {
                case ProductEnum.MySQL_Laravel_API_React:
                case ProductEnum.MySQL_Laravel_API:
                    AuthSelectionControl.HideDefaultValueControl();
                    break;
                case ProductEnum.MySQL_NodeJSAPI_React_Windows:
                case ProductEnum.MySQL_NodeJSAPI_Windows:
                    
                    break;
            }

        }
        private void AuthSelectionControl_AuthTableSelectionChanged(object? sender, EventArgs e)
        {
            switch (SelectedDataType)
            {
                case DataTypeEnum.MySQL:
                    var mySQLDb = new MySQLDBHelper(hostMysqlTextBox.Text.Trim(), portMysqlTextbox.Text.Trim(), usernameMysqlTextBox.Text.Trim(), passwordMysqlTextBox.Text.Trim(), dbNameMysqlTextBox.Text.Trim());
                    if (mySQLDb.Connect() && AuthSelectionControl != null)
                    {
                        AuthSelectionControl.SetUserAndPasswordColumn(mySQLDb.GetTableColumns(AuthSelectionControl.AuthTableName));
                    }
                    break;
                case DataTypeEnum.MSSQL:
                    var msSQLDbPHP = new MSSQLDBHelper(hostTextboxMSSQL.Text.Trim(), portTextBoxMSSQL.Text.Trim(), usernameTextBoxMSSQL.Text.Trim(), passwordTextBoxMSSQL.Text.Trim(), dbNameTextBoxMSSQL.Text.Trim(), winAuthCheckBoxMSSQL.Checked);
                    if (msSQLDbPHP.Connect() && AuthSelectionControl != null)
                    {
                        AuthSelectionControl.SetUserAndPasswordColumn(msSQLDbPHP.GetTableColumns(AuthSelectionControl.AuthTableName));
                    }
                        break;
                case DataTypeEnum.PostgreSQL:
                    var pgSQLDb = new PGSQLDBHelper(hostPGTextBox.Text.Trim(), schemaNamePGTextBox.Text.Trim(), portPGTextbox.Text.Trim(), usernamePGTextBox.Text.Trim(), passwordPGTextBox.Text.Trim(), dbNamePGTextBox.Text.Trim());
                    if (pgSQLDb.Connect() && AuthSelectionControl != null)
                    {
                        AuthSelectionControl.SetUserAndPasswordColumn(pgSQLDb.GetColumns(AuthSelectionControl.AuthTableName));
                    }
                    break;
                case DataTypeEnum.MongoDB:
                    break;
            }
        }
        #endregion
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            InitControls(emailTextbox.Text.Trim());
        }
        #region UIHelper
        private void MessageBoxEvent(NKVMessage obj)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show(obj.Message);
            }));
        }
        private void CompletedEvent(NKVMessage obj)
        {
            Invoke(new Action(() =>
            {
                if (obj.IsSuccess)
                    appendSuccess(obj.Message);
                else
                    appendError(obj.Message);
            }));
        }
        private void MessageEvent(NKVMessage obj)
        {
            Invoke(new Action(() =>
            {
                if (obj.IsSuccess)
                    appendSuccess(obj.Message);
                else
                    appendError(obj.Message);
            }));
        }
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
                if (LogTextAreaControl != null)
                {
                    LogTextAreaControl.SetLogText(msg, Color.Green, clearPrior);
                }
                else
                    MessageBox.Show(msg);
            }));
        }
        #endregion
        #region LicenceManagement
        public List<LicenceProductModel> FilterLicence(ProductEnum ActiveProduct)
        {
            var listProduct = LicenceProductList.Where(x => int.Parse(x.ProductNumber) == (int)ActiveProduct).ToList();
            if (listProduct.Any())
            {
                return listProduct;
            }
            return new List<LicenceProductModel>() { new LicenceProductModel() { ProductID = 0, ProductNumber = "0", ProductName = "No Product Licence Found!" } };
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
                    licenceDataGridView.Columns["PublicId"].Visible = false;
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
                    MessageBox.Show("No Licence Found, Please register. Or Licence Management at : https://getautomator.com/app");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            if (sourceComboBox != null && sourceComboBox.SelectedItem != null && productComboBox.SelectedItem != null)
            {
                var product = (ProductModel)productComboBox.SelectedItem;
                var registerData = new RegisterModel()
                {

                    LicenceNumber = purchaseCodeTextbox.Text.Trim(),
                    MacID = Helper.GetSystemName(),
                    SoftwareVersion = SoftwareVersion,
                    Source = sourceComboBox.SelectedItem.ToString(),
                    ToolName = product.ProductName,
                    UserEmail = emailTextbox.Text.Trim(),
                    ItemName = product.ProductName,
                };
                if (v.Register(registerData))
                {
                    CreateEmailFile(emailTextbox.Text.Trim());
                    appendSuccess("Thanks for registration!");
                    DisplayLicenceGrid(Helper.GetSystemName());
                    registerBtn.Enabled = true;
                }
                else
                {
                    appendError("Invalid licence, Please contact support or manage your licence at https://getautomator.com/app", true);
                    registerBtn.Enabled = true;
                }
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
        #endregion
        #region GenerateButtonClick
        private void automatorGenerateButton_Click(object sender, EventArgs e)
        {
            if (TableSelectionControl.SelectedTableList.Count == 0)
            {
                MessageBox.Show("Please select at least one table to generate.");
                return;
            }

            if (ValidateLicenceOnGenerate())
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
        private bool ValidateLicenceOnGenerate()
        {
            if (ActiveLicence != null && v.ClickCounter(ActiveLicence.PublicID, TableSelectionControl.SelectedTableList.Count.ToString()))
            {
                return true;
            }
            return false;
        }
        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Invoke(new Action(() =>
            {

                NKVConfiguration config = new NKVConfiguration()
                {
                    AuthTableConfig = new NKVAuthTableConfig
                    {
                        IsEmail = AuthSelectionControl.IsEmail,
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
                        mysqlGenerateButton.Enabled = false;
                        mysqlGenerateButton.Text = "Processing...";
                        ProcessMySQL(config);
                        mysqlGenerateButton.Enabled = true;
                        mysqlGenerateButton.Text = "Generate";
                        break;
                    case DataTypeEnum.MSSQL:
                        mssqlGenerateButton.Enabled = false;
                        mssqlGenerateButton.Text = "Processing...";
                        ProcessMSSQL(config);
                        mssqlGenerateButton.Enabled = true;
                        mssqlGenerateButton.Text = "Generate";
                        break;
                    case DataTypeEnum.PostgreSQL:
                        pgSQLGenerateButton.Text = "Processing...";
                        pgSQLGenerateButton.Enabled = false;
                        ProcessPGSQL(config);
                        pgSQLGenerateButton.Enabled = true;
                        pgSQLGenerateButton.Text = "Generate";
                        break;
                    case DataTypeEnum.MongoDB:
                        break;
                }
            }));
        }
        private void ProcessMySQL(NKVConfiguration config)
        {
            switch (ActiveProduct)
            {
                case ProductEnum.MySQL_Laravel_API:
                case ProductEnum.MySQL_Laravel_API_React:
                    var mySQLDb = new MySQLDBHelper(hostMysqlTextBox.Text.Trim(), portMysqlTextbox.Text.Trim(), usernameMysqlTextBox.Text.Trim(), passwordMysqlTextBox.Text.Trim(), dbNameMysqlTextBox.Text.Trim());
                    if (mySQLDb.Connect())
                    {
                        MySQL_LaravelAPI laravelAPI = new MySQL_LaravelAPI(config, multiTenantCheckBoxMysql.Checked, "//");
                        laravelAPI.MessageEvent += MessageEvent;
                        laravelAPI.CompletedEvent += CompletedEvent;
                        var reactInput = laravelAPI.Automator(ProjectNameControl.ProjectName, TableSelectionControl.SelectedTableList, mySQLDb);
                        if (ActiveProduct == ProductEnum.MySQL_Laravel_API_React && reactInput != null && !string.IsNullOrEmpty(reactInput.DestinationFolder))
                        {
                            appendSuccess("----- Generating React App -----");
                            ReactTs_LaravelMySQL reactLaravel = new ReactTs_LaravelMySQL(ProjectNameControl.ProjectName, reactInput.DestinationFolder, "//");
                            reactLaravel.MessageEvent += MessageEvent;
                            reactLaravel.CompletedEvent += CompletedEvent;
                            reactLaravel.CreateReactAPP(reactInput);
                        }
                        MessageBox.Show("Task Completed! Please check the generated project folder.");
                    }
                    else
                    {
                        MessageBox.Show("Database Connection Failed, Please click on Test Connection to validate");
                    }
                    break;
                case ProductEnum.MySQL_NodeJSAPI_React_Windows:
                case ProductEnum.MySQL_NodeJSAPI_Windows:
                    var mySQLDbNode = new MySQLDBHelper(hostMysqlTextBox.Text.Trim(), portMysqlTextbox.Text.Trim(), usernameMysqlTextBox.Text.Trim(), passwordMysqlTextBox.Text.Trim(), dbNameMysqlTextBox.Text.Trim());
                    if (mySQLDbNode.Connect())
                    {
                        MySQL_NodeJSAPI nodejsAPI = new MySQL_NodeJSAPI(config, multiTenantCheckBoxMysql.Checked, "//");
                        nodejsAPI.MessageEvent += MessageEvent;
                        nodejsAPI.CompletedEvent += CompletedEvent;
                        var reactInput = nodejsAPI.Automator(ProjectNameControl.ProjectName, TableSelectionControl.SelectedTableList, mySQLDbNode);
                        if (ActiveProduct == ProductEnum.MySQL_NodeJSAPI_React_Windows && reactInput != null && !string.IsNullOrEmpty(reactInput.DestinationFolder))
                        {
                            appendSuccess("----- Generating React App -----");
                            ReactTs_LaravelMySQL reactLaravel = new ReactTs_LaravelMySQL(ProjectNameControl.ProjectName, reactInput.DestinationFolder, "//");
                            reactLaravel.MessageEvent += MessageEvent;
                            reactLaravel.CompletedEvent += CompletedEvent;
                            reactLaravel.CreateReactAPP(reactInput);
                        }
                        MessageBox.Show("Task Completed! Please check the generated project folder.");
                    }
                    else
                    {
                        MessageBox.Show("Database Connection Failed, Please click on Test Connection to validate");
                    }
                    break;
                case ProductEnum.MySql_PHPAPI:
                case ProductEnum.MySQl_PHP_React_Windows:
                case ProductEnum.MySQl_PHP_React_MacOS:
                    var mySQLDbPHP = new MySQLDBHelper(hostMysqlTextBox.Text.Trim(), portMysqlTextbox.Text.Trim(), usernameMysqlTextBox.Text.Trim(), passwordMysqlTextBox.Text.Trim(), dbNameMysqlTextBox.Text.Trim());
                    if (mySQLDbPHP.Connect())
                    {
                        MySQL_PHPAPI phpAPI = new MySQL_PHPAPI(config, multiTenantCheckBoxPgSQL.Checked, "//");
                        phpAPI.MessageEvent += MessageEvent;
                        phpAPI.CompletedEvent += CompletedEvent;
                        var reactInput = phpAPI.Automator(ProjectNameControl.ProjectName, TableSelectionControl.SelectedTableList, mySQLDbPHP);
                        if (ActiveProduct == ProductEnum.MySQl_PHP_React_Windows && reactInput != null && !string.IsNullOrEmpty(reactInput.DestinationFolder))
                        {
                            appendSuccess("----- Generating React App -----");
                            ReactTS_PHPPGSQL reactPGSqlTs = new ReactTS_PHPPGSQL(ProjectNameControl.ProjectName, reactInput.DestinationFolder, "//");
                            reactPGSqlTs.MessageEvent += MessageEvent;
                            reactPGSqlTs.CompletedEvent += CompletedEvent;
                            reactPGSqlTs.CreateReactAPP(reactInput);
                        }
                        MessageBox.Show("Task Completed! Please check the generated project folder.");
                    }
                    
                    else
                    {
                        MessageBox.Show("Database Connection Failed, Please click on Test Connection to validate");
                    }
                    break;
            }

        }
        private void ProcessMSSQL(NKVConfiguration config)
        {
            switch (ActiveProduct)
            {
               
                case ProductEnum.MSSQL_PHP_API:
                    var msSQLDbPHP = new MSSQLDBHelper(hostTextboxMSSQL.Text.Trim(), portTextBoxMSSQL.Text.Trim(), usernameTextBoxMSSQL.Text.Trim(), passwordTextBoxMSSQL.Text.Trim(), dbNameTextBoxMSSQL.Text.Trim(), winAuthCheckBoxMSSQL.Checked);
                    if (msSQLDbPHP.Connect())
                    {
                        MSSQL_PHPAPI phpAPI = new MSSQL_PHPAPI(config, false, "//");
                        phpAPI.MessageEvent += MessageEvent;
                        phpAPI.CompletedEvent += CompletedEvent;
                        var reactInput = phpAPI.Automator(ProjectNameControl.ProjectName, TableSelectionControl.SelectedTableList, msSQLDbPHP);
                        MessageBox.Show("Task Completed! Please check the generated project folder.");
                    }

                    else
                    {
                        MessageBox.Show("Database Connection Failed, Please click on Test Connection to validate");
                    }
                    break;
            }

        }
        private void ProcessPGSQL(NKVConfiguration config)
        {
            switch (ActiveProduct)
            {
                case ProductEnum.PGSQL_PHPAPI:
                case ProductEnum.PGSQL_PHPAPI_React_Windows:
                    var pgSQLDb = new PGSQLDBHelper(hostPGTextBox.Text.Trim(), schemaNamePGTextBox.Text.Trim(), portPGTextbox.Text.Trim(), usernamePGTextBox.Text.Trim(), passwordPGTextBox.Text.Trim(), dbNamePGTextBox.Text.Trim());
                    if (pgSQLDb.Connect())
                    {
                        PGSQL_PHPAPI pgPHPAPI = new PGSQL_PHPAPI(config, multiTenantCheckBoxPgSQL.Checked, "//");
                        pgPHPAPI.MessageEvent += MessageEvent;
                        pgPHPAPI.CompletedEvent += CompletedEvent;
                        var reactInput = pgPHPAPI.Automator(ProjectNameControl.ProjectName, TableSelectionControl.SelectedTableList, pgSQLDb);
                        if (ActiveProduct == ProductEnum.PGSQL_PHPAPI_React_Windows && reactInput != null && !string.IsNullOrEmpty(reactInput.DestinationFolder))
                        {
                            appendSuccess("----- Generating React App -----");
                            ReactTS_PHPPGSQL reactPGSqlTs = new ReactTS_PHPPGSQL(ProjectNameControl.ProjectName, reactInput.DestinationFolder, "//");
                            reactPGSqlTs.MessageEvent += MessageEvent;
                            reactPGSqlTs.CompletedEvent += CompletedEvent;
                            reactPGSqlTs.CreateReactAPP(reactInput);
                        }
                        MessageBox.Show("Task Completed! Please check the generated project folder.");
                    }
                    else
                    {
                        MessageBox.Show("Database Connection Failed, Please click on Test Connection to validate");
                    }
                    break;
            }
        }
        #endregion

        #region TestConnectionButton
        private void testConPgSQLButton_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedDataType = DataTypeEnum.PostgreSQL;

                var pgSQLDb = new PGSQLDBHelper(hostPGTextBox.Text.Trim(), schemaNamePGTextBox.Text.Trim(), portPGTextbox.Text.Trim(), usernamePGTextBox.Text.Trim(), passwordPGTextBox.Text.Trim(), dbNamePGTextBox.Text.Trim());
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
        private void testConnectionButtonMySql_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedDataType = DataTypeEnum.MySQL;
                var mysqlDb = new MySQLDBHelper(hostMysqlTextBox.Text.Trim(), portMysqlTextbox.Text.Trim(), usernameMysqlTextBox.Text.Trim(), passwordMysqlTextBox.Text.Trim(), dbNameMysqlTextBox.Text.Trim());
                if (mysqlDb.Connect())
                {
                    MessageBox.Show("Connection Successful!");
                    appendSuccess("Connection Successful!", true);
                    var tableList = mysqlDb.GetListOfTable();
                    AuthSelectionControl.SetTableList(tableList);
                    TableSelectionControl.SetTableList(tableList);
                };
            }
            catch (Exception ex)
            {
                appendError(ex.Message, true);
            }
        }
        private void testConnectionBtnMSSQL_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedDataType = DataTypeEnum.MSSQL;
                var mssqlDb = new MSSQLDBHelper(hostTextboxMSSQL.Text.Trim(), portTextBoxMSSQL.Text.Trim(), usernameTextBoxMSSQL.Text.Trim(), passwordTextBoxMSSQL.Text.Trim(), dbNameTextBoxMSSQL.Text.Trim(),winAuthCheckBoxMSSQL.Checked);
                if (mssqlDb.Connect())
                {
                    MessageBox.Show("Connection Successful!");
                    appendSuccess("Connection Successful!", true);
                    var tableList = mssqlDb.GetListOfTable();
                    AuthSelectionControl.SetTableList(tableList);
                    TableSelectionControl.SetTableList(tableList);
                };
            }
            catch (Exception ex)
            {
                appendError(ex.Message, true);
            }
        }

        private void winAuthCheckBoxMSSQL_CheckedChanged(object sender, EventArgs e)
        {
            usernameTextBoxMSSQL.Enabled = !winAuthCheckBoxMSSQL.Checked;
            passwordTextBoxMSSQL.Enabled = !winAuthCheckBoxMSSQL.Checked;
        }
        #endregion
        #region OpenURL
        private void youtubeButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenUrl("https://www.youtube.com/channel/UCeHZQzRKLzwXykuLb_RXhvw?sub_confirmation=1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void footerLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                OpenUrl("https://getautomator.com");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void discordButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenUrl("https://discord.gg/RzYRHJmwaE");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void OpenUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    System.Diagnostics.Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    System.Diagnostics.Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    System.Diagnostics.Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }



        #endregion

    }
}