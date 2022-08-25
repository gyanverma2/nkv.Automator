using nkv.Automator.Generator.Models;
using nkv.Automator.Models;
using nkv.Automator.Postman;
using nkv.Automator.Utility;
using System.Globalization;

namespace nkv.Automator.PGSQL
{
    public class PGSQL_PHPAPI
    {
        List<PostmanModel> postmanJson = new List<PostmanModel>();
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        public string TemplateFolder { get; set; } = null!;
        public string TemplateFolderSeparator { get; set; } = null!;
        public string DestinationFolderSeparator { get; set; } = null!;
        public string DestinationFolder { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        public List<Exception> ExceptionList { get; set; }
        public List<string> SelectedTable { get; set; } = null!;
        public NKVConfiguration ConfigApp { get; set; } = null!;
        public PGSQLDBHelper pgSQLDB { get; set; } = null!;
        public Action<NKVMessage> MessageEvent { get; set; } = null!;
        public Action<NKVMessage> CompletedEvent { get; set; } = null!;
        public bool IsMultiTenant { get; set; }
        public PGSQL_PHPAPI(NKVConfiguration config, bool isMultiTenant, string destinationFolderSeparator)
        {
            IsMultiTenant = isMultiTenant;
            ConfigApp = config;
            ExceptionList = new List<Exception>();
            TemplateFolder = "PGSQLPHPAPITemplate";
            TemplateFolderSeparator = "\\";
            DestinationFolderSeparator = destinationFolderSeparator;
        }
        private string CreateDirectory()
        {
            string destFile = ProjectName;
            string newName = ProjectName;
            if (Directory.Exists(destFile))
            {
                int n = 0;
                do
                {
                    n++;
                    newName = string.Format("{0}({1})", destFile, n);
                }
                while (Directory.Exists(newName));
            }
            Directory.CreateDirectory(newName);
            Directory.CreateDirectory(newName + "//config//");
            Directory.CreateDirectory(newName + "//objects//");
            Directory.CreateDirectory(newName + "//token//");
            Directory.CreateDirectory(newName + "//jwt//");
            Directory.CreateDirectory(newName + "//POSTMAN_IMPORT_FILE//");
            Directory.CreateDirectory(newName + "//files//");
            Directory.CreateDirectory(newName + "//files//upload//");
            Directory.CreateDirectory(newName + "//notification//");
            Directory.CreateDirectory(newName + "//notification//PHPMailer");
            return newName;
        }
        public string CreateTemplatePath(string filePathString)
        {
            string path = TemplateFolder;
            foreach (var p in filePathString.Split(","))
            {
                if (!string.IsNullOrEmpty(p))
                    path = path + TemplateFolderSeparator + p;
            }
            return path;
        }
        public string CreateDestinationPath(string filePathString)
        {
            string path = DestinationFolder;
            foreach (var p in filePathString.Split(","))
            {
                if (!string.IsNullOrEmpty(p))
                    path = path + DestinationFolderSeparator + p;
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return path;
        }
        public bool Automator(string projectName, List<string> selectedTable, PGSQLDBHelper pgSql)
        {
            pgSQLDB = pgSql;
            SelectedTable = selectedTable;
            ProjectName = projectName;
            DestinationFolder = CreateDirectory();
            MessageEvent?.Invoke(new NKVMessage("Project Folder Created : " + DestinationFolder));
            CopyExistingFile();
            CreateDBFile();
            MessageEvent?.Invoke(new NKVMessage("Database Config File Created"));
            
            string primaryKeyAuth = "";
            foreach (string t in selectedTable)
            {
                try
                {
                    MessageEvent?.Invoke(new NKVMessage("Processing for Table => " + t));
                    string table = t;
                    bool isView = false;
                    if (table.StartsWith("View - "))
                    {
                        isView = true;
                        table = table.Replace("View - ", "");
                    }
                    Directory.CreateDirectory(CreateDestinationPath(table));
                    var columnList = pgSQLDB.GetColumns(table);
                    var finalData = pgSQLDB.BuildQuery(table);
                    string modelName = ti.ToTitleCase(table);
                    if(table == ConfigApp.AuthTableConfig.AuthTableName)
                    {
                        primaryKeyAuth = finalData.PrimaryKeyString;
                    }
                    CreateObjectFile(table, finalData, isView);
                    if (!isView)
                    {
                        CreateInsertFile(table, finalData);
                        CreateUpdateFile(table, finalData);
                        CreateUpdatePatchFile(table, finalData);
                        CreateModalDeleteFile(table, finalData);
                    }
                    CreateReadFile(table, finalData);
                    var pList = CreatePostmanJson(table, finalData, isView);
                    postmanJson.AddRange(pList);
                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage("Exception on table " +t,false));
                    MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                    ExceptionList.Add(ex);
                }
            }
            CreateAuthTokenFile(primaryKeyAuth);
            MessageEvent?.Invoke(new NKVMessage("Login created for jwt token"));
            CreatePostmanFile(postmanJson);
            MessageEvent?.Invoke(new NKVMessage("Postman import collection file generated"));
            CreateIndexFile();
            CompletedEvent?.Invoke(new NKVMessage("Thanks for using GetAutomator.com! Please check the generated code at : " + DestinationFolder));
            return true;
        }
        private string initDBStr()
        {
            if (IsMultiTenant)
            {
                var dbStr = "if (isset($decodedJWTData) && isset($decodedJWTData->tenant))" + Environment.NewLine;
                dbStr = dbStr + "{" + Environment.NewLine;
                dbStr = dbStr + "$database = new Database($decodedJWTData->tenant); " + Environment.NewLine;
                dbStr = dbStr + "}" + Environment.NewLine;
                dbStr = dbStr + "else " + Environment.NewLine;
                dbStr = dbStr + "{" + Environment.NewLine;
                dbStr = dbStr + "$database = new Database(); " + Environment.NewLine;
                dbStr = dbStr + "}" + Environment.NewLine;
                return dbStr;
            }
            else return "$database = new Database();";
        }
        private void CopyExistingFile()
        {
            string path = CreateDestinationPath("files,readfile.php");
            string path2 = CreateDestinationPath("files,uploadfile.php");
            string path3 = CreateDestinationPath("files,uploadimage.php");
            string path4 = CreateDestinationPath("notification,send.php");
            string path11 = CreateDestinationPath("notification,sendemail.php");
            string path5 = CreateDestinationPath("notification,PHPMailer,OAuth.php");
            string path6 = CreateDestinationPath("notification,PHPMailer,PHPMailer.php");
            string path7 = CreateDestinationPath("notification,PHPMailer,POP3.php");
            string path8 = CreateDestinationPath("notification,PHPMailer,SMTP.php");
            string path9 = CreateDestinationPath("notification,PHPMailer,Exception.php");
            string path10 = CreateDestinationPath("config,helper.php");
            string path12 = CreateDestinationPath("config,header.php");
            var initDatabaseModel = initDBStr();
            string contents = File.ReadAllText(CreateTemplatePath("files,readfile.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{initDatabaseModel}", initDatabaseModel);
                txtFile.WriteLine(contents);
            }
            string content2 = File.ReadAllText(CreateTemplatePath("files,uploadfile.txt"));
            using (var txtFile = File.AppendText(path2))
            {
                content2 = content2.Replace("{initDatabaseModel}", initDatabaseModel);
                txtFile.WriteLine(content2);
            }

            string content3 = File.ReadAllText(CreateTemplatePath("files,uploadimage.txt"));
            using (var txtFile = File.AppendText(path3))
            {
                content3 = content3.Replace("{initDatabaseModel}", initDatabaseModel);
                txtFile.WriteLine(content3);
            }

            string content4 = File.ReadAllText(CreateTemplatePath("notification,send.txt"));
            using (var txtFile = File.AppendText(path4))
            {
                content4 = content4.Replace("{initDatabaseModel}", initDatabaseModel);
                txtFile.WriteLine(content4);
            }

            string content11 = File.ReadAllText(CreateTemplatePath("notification,sendemail.txt"));
            using (var txtFile = File.AppendText(path11))
            {
                content11 = content11.Replace("{initDatabaseModel}", initDatabaseModel);
                txtFile.WriteLine(content11);
            }

            string content5 = File.ReadAllText(CreateTemplatePath("notification,PHPMailer,Exception.txt"));
            using (var txtFile = File.AppendText(path5))
            {
                txtFile.WriteLine(content5);
            }

            string content6 = File.ReadAllText(CreateTemplatePath("notification,PHPMailer,OAuth.txt"));
            using (var txtFile = File.AppendText(path6))
            {
                txtFile.WriteLine(content6);
            }

            string content7 = File.ReadAllText(CreateTemplatePath("notification,PHPMailer,PHPMailer.txt"));
            using (var txtFile = File.AppendText(path7))
            {
                txtFile.WriteLine(content7);
            }

            string content8 = File.ReadAllText(CreateTemplatePath("notification,PHPMailer,POP3.txt"));
            using (var txtFile = File.AppendText(path8))
            {
                txtFile.WriteLine(content8);
            }

            string content9 = File.ReadAllText(CreateTemplatePath("notification,PHPMailer,SMTP.txt"));
            using (var txtFile = File.AppendText(path9))
            {
                txtFile.WriteLine(content9);
            }


            string content10 = File.ReadAllText(CreateTemplatePath("config,helper.txt"));
            using (var txtFile = File.AppendText(path10))
            {
                txtFile.WriteLine(content10);
            }

            string content12 = File.ReadAllText(CreateTemplatePath("config,header.txt"));
            using (var txtFile = File.AppendText(path12))
            {
                txtFile.WriteLine(content12);
            }
        }

        public void CreateDBFile()
        {
            string path = CreateDestinationPath("config,database.php");
            string contents = File.ReadAllText(CreateTemplatePath("config,database.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{hostName}",pgSQLDB.Host);
                contents = contents.Replace("{userName}", pgSQLDB.Username);
                contents = contents.Replace("{password}", pgSQLDB.Password);
                contents = contents.Replace("{dbName}", pgSQLDB.DBName);
                contents = contents.Replace("{port}", pgSQLDB.Port.ToString());
                txtFile.WriteLine(contents);
            }
        }
        public void CreateAuthTokenFile(string primaryKey)
        {
            string path = CreateDestinationPath("token,token.php");
            string path2 = CreateDestinationPath("token,generate.php");
            string path3 = CreateDestinationPath("token,validatetoken.php");
            string path4 = CreateDestinationPath("jwt,BeforeValidException.php");
            string path5 = CreateDestinationPath("jwt,ExpiredException.php");
            string path6 = CreateDestinationPath("jwt,JWT.php");
            string path7 = CreateDestinationPath("jwt,SignatureInvalidException.php");
            var initDatabase = "$database = new Database();";
            var setMultiTenantToken = "";
            if (IsMultiTenant)
            {
                initDatabase = "$tenantHeader = get_header('x-tenant');" + Environment.NewLine;
                initDatabase = initDatabase + "if ($tenantHeader){" + Environment.NewLine;
                initDatabase = initDatabase + " $database = new Database($tenantHeader);" + Environment.NewLine;
                initDatabase = initDatabase + "}else" + Environment.NewLine;
                initDatabase = initDatabase + "{" + Environment.NewLine;
                initDatabase = initDatabase + "$database = new Database();" + Environment.NewLine;
                initDatabase = initDatabase + "}" + Environment.NewLine;
                setMultiTenantToken = "$token['tenant'] =$tenantHeader;";
            }

            string contents = File.ReadAllText(CreateTemplatePath("token,token.txt"));
            using (var txtFile = File.AppendText(path))
            {
                txtFile.WriteLine(contents);
            }
            if (!ConfigApp.AuthTableConfig.IsSkipAuth)
            {
                string contents2 = File.ReadAllText(CreateTemplatePath("token,generate.txt"));
                using (var txtFile = File.AppendText(path2))
                {
                    contents2 = contents2.Replace("{userTable}", ConfigApp.AuthTableConfig.AuthTableName);
                    contents2 = contents2.Replace("{moduleName}", ti.ToTitleCase(ConfigApp.AuthTableConfig.AuthTableName));
                    contents2 = contents2.Replace("{userColumn}", ConfigApp.AuthTableConfig.UsernameColumnName);
                    contents2 = contents2.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                    contents2 = contents2.Replace("{primaryKey}", primaryKey);
                    contents2 = contents2.Replace("{initDatabase}", initDatabase);
                    contents2 = contents2.Replace("{setMultiTenantToken}", setMultiTenantToken);
                    txtFile.WriteLine(contents2);
                }
            }
            else
            {
                string contents2 = File.ReadAllText(CreateTemplatePath("token,generate-simple.txt"));
                using (var txtFile = File.AppendText(path2))
                {
                    contents2 = contents2.Replace("{userColumn}", ConfigApp.AuthTableConfig.UsernameColumnName);
                    contents2 = contents2.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                    txtFile.WriteLine(contents2);
                }
            }

            string contents3 = File.ReadAllText(CreateTemplatePath("token,validatetoken.txt"));
            using (var txtFile = File.AppendText(path3))
            {
                txtFile.WriteLine(contents3);
            }

            string contents4 = File.ReadAllText(CreateTemplatePath("jwt,BeforeValidException.txt"));
            using (var txtFile = File.AppendText(path4))
            {
                txtFile.WriteLine(contents4);
            }

            string contents5 = File.ReadAllText(CreateTemplatePath("jwt,ExpiredException.txt"));
            using (var txtFile = File.AppendText(path5))
            {
                txtFile.WriteLine(contents5);
            }

            string contents6 = File.ReadAllText(CreateTemplatePath("jwt,JWT.txt"));
            using (var txtFile = File.AppendText(path6))
            {
                txtFile.WriteLine(contents6);
            }

            string contents7 = File.ReadAllText(CreateTemplatePath("jwt,SignatureInvalidException.txt"));
            using (var txtFile = File.AppendText(path7))
            {
                txtFile.WriteLine(contents7);
            }
        }
        private string GetLoginFunction()
        {
            string loginValidationFunction = "function login_validation(){ " + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "{selectLoginQuery}" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "$stmt = $this->conn->prepare($query);" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "{selectLoginBindValue}" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "$stmt->execute();" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "$row = $stmt->fetch(PDO::FETCH_ASSOC);" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "$num = $stmt->rowCount();" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "if($num>0){" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "{selectLoginSetValues}" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "}" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "else{" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "$this->{primaryKey}=null;" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "}" + Environment.NewLine;
            loginValidationFunction = loginValidationFunction + "}" + Environment.NewLine;
            return loginValidationFunction;
        }
        public void CreateObjectFile(string tableName, FinalDataPHP finalData, bool isView = false)
        {
            string path = CreateDestinationPath("objects,"+tableName + ".php");
            string loginFunction = "";
            if (!ConfigApp.AuthTableConfig.IsSkipAuth && tableName == ConfigApp.AuthTableConfig.AuthTableName)
            {
                loginFunction = GetLoginFunction();
                string selectLoginQuery = finalData.SelectLoginQuery.Replace("{userColumn}", ConfigApp.AuthTableConfig.UsernameColumnName);
                selectLoginQuery = selectLoginQuery.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                string selectLoginBindValue = finalData.SelectLoginBindValue.Replace("{userColumn}", ConfigApp.AuthTableConfig.UsernameColumnName);
                selectLoginBindValue = selectLoginBindValue.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                loginFunction = loginFunction.Replace("{selectLoginQuery}", selectLoginQuery);
                loginFunction = loginFunction.Replace("{selectLoginBindValue}", selectLoginBindValue);
                loginFunction = loginFunction.Replace("{tableName}", tableName);
                loginFunction = loginFunction.Replace("{primaryKey}", finalData.PrimaryKeyString);
                loginFunction = loginFunction.Replace("{objectProperties}", finalData.ObjectProperties);
                loginFunction = loginFunction.Replace("{moduleName}", finalData.TableModuleName);
                loginFunction = loginFunction.Replace("{selectLoginSetValues}", finalData.SelectLoginSetValues);
            }
            string templateObjectFile = "objects.txt";
            if (isView)
            {
                templateObjectFile = "objects_view.txt";
            }
            string contents = File.ReadAllText(CreateTemplatePath("objects," + templateObjectFile));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{primaryKey}", finalData.PrimaryKeyString);
                contents = contents.Replace("{objectProperties}", finalData.ObjectProperties);
                contents = contents.Replace("{moduleName}", finalData.TableModuleName);

                contents = contents.Replace("{selectQuery}", finalData.SelectAllQuery);
                contents = contents.Replace("{selectOneQuery}", finalData.SelectOneQuery);
                contents = contents.Replace("{selectOneBindValue}", finalData.SelectOneBindValue);
                contents = contents.Replace("{selectOneSetValueToObject}", finalData.SelectOneSetValues);
                contents = contents.Replace("{loginValidationFunction}", loginFunction);

                contents = contents.Replace("{searchQuery}", finalData.SearchQuery);
                contents = contents.Replace("{searchBindValue}", finalData.SearchBindValue);
                contents = contents.Replace("{searchCountQuery}", finalData.SearchCountQuery);

                contents = contents.Replace("{insertQuery}", finalData.InsertQuery);
                contents = contents.Replace("{insertSanitize}", finalData.InsertSanitize);
                contents = contents.Replace("{insertBindValue}", finalData.InsertBindValues);

                contents = contents.Replace("{updateQuery}", finalData.UpdateQuery);
                contents = contents.Replace("{updateSanitize}", finalData.UpdateSanitize);
                contents = contents.Replace("{updateBindValue}", finalData.UpdateBindValues);

                contents = contents.Replace("{deleteQuery}", finalData.DeleteQuery);
                contents = contents.Replace("{deleteBind}", finalData.DeleteBind);
                contents = contents.Replace("{deleteSenitize}", finalData.DeleteSanitize);


                contents = contents.Replace("{searchQueryByColumn}", finalData.SearchQueryByColumn);
                contents = contents.Replace("{searchCountQueryByColumn}", finalData.SearchCountQueryByColumn);
                if (finalData.FKQueryDic != null && finalData.FKQueryDic.Count > 0)
                {
                    string fkContent = "";
                    foreach (var eFK in finalData.FKQueryDic)
                    {
                        fkContent = fkContent + Environment.NewLine + eFK.Value + Environment.NewLine;
                    }
                    contents = contents.Replace("{extraFunctions}", fkContent);
                }
                else
                {
                    contents = contents.Replace("{extraFunctions}", "");
                }
                txtFile.WriteLine(contents);
            }
        }
        public void CreateInsertFile(string tableName,FinalDataPHP finalData)
        {
            string path = CreateDestinationPath(tableName + ",create.php");
            string requiredFields = "";
            string setPropertyValue = "";
            foreach (var c in finalData.SelectQueryData.ColumnList)
            {

                if (c.Extra == "auto_increment")
                {

                }
                else if (c.Extra == "CURRENT_TIMESTAMP")
                {
                    setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = date('Y-m-d H:i:s');";
                }
                else if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    if (!ConfigApp.AuthTableConfig.IsSkipAuth && ConfigApp.AuthTableConfig.AuthTableName == tableName && ConfigApp.AuthTableConfig.PasswordColumnName == c.Field)
                    {
                        setPropertyValue = setPropertyValue + Environment.NewLine + "if(!isEmpty($data->" + c.Field + ")) { ";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = md5($data->" + c.Field + ");";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "}";
                    }
                    else if (c.IsNull != null && c.IsNull.ToLower() == "no")
                    {
                        if (string.IsNullOrEmpty(requiredFields))
                        {
                            requiredFields = "!isEmpty($data->" + c.Field + ")";
                        }
                        else
                        {
                            requiredFields = requiredFields + Environment.NewLine + "&&" + "!isEmpty($data->" + c.Field + ")";
                        }

                        setPropertyValue = setPropertyValue + Environment.NewLine + "if(!isEmpty($data->" + c.Field + ")) { ";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "} else { ";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = '" + c.DefaultValue + "';";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "}";
                    }
                    else
                    {
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                    }

                }
            }
            string initDatabaseModel = initDBStr();
            requiredFields = requiredFields.Trim('&').Trim('&');
            string contents = File.ReadAllText(CreateTemplatePath("modal,create.txt"));
            using (var txtFile = File.AppendText(path))
            {
                if (string.IsNullOrEmpty(requiredFields))
                {
                    requiredFields = "true";
                }
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{moduleName}", finalData.TableModuleName);
                contents = contents.Replace("{requiredFields}", requiredFields);
                contents = contents.Replace("{setPropertyValue}", setPropertyValue);
                contents = contents.Replace("{initDatabaseModel}", initDatabaseModel);
                txtFile.WriteLine(contents);
            }
        }

        public void CreateUpdateFile(string tableName,FinalDataPHP finalData)
        {
            string path = CreateDestinationPath(tableName + ",update.php");
            string requiredFields = "";
            string setPropertyValue = "";
            var columns = finalData.SelectQueryData.ColumnList;
            foreach (var c in columns)
            {
                if (c.Extra == "auto_increment")
                {

                }
                else if (c.Extra == "CURRENT_TIMESTAMP")
                {

                }
                else if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    if (c.IsNull != null && c.IsNull.ToLower() == "no")
                    {
                        if (string.IsNullOrEmpty(requiredFields))
                        {
                            requiredFields = "!isEmpty($data->" + c.Field + ")";
                        }
                        else
                        {
                            requiredFields = requiredFields + Environment.NewLine + "&&" + "!isEmpty($data->" + c.Field + ")";
                        }
                        setPropertyValue = setPropertyValue + Environment.NewLine + "if(!isEmpty($data->" + c.Field + ")) { ";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "} else { ";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = '" + c.DefaultValue + "';";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "}";
                    }
                    else
                    {
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                    }
                }
            }
            string initDatabaseModel = initDBStr();
            requiredFields = requiredFields.Trim('&').Trim('&');
            string contents = File.ReadAllText(CreateTemplatePath("modal,update.txt"));
            using (var txtFile = File.AppendText(path))
            {
                if (string.IsNullOrEmpty(requiredFields))
                {
                    requiredFields = "true";
                }

                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{moduleName}", finalData.TableModuleName);
                contents = contents.Replace("{requiredFields}", requiredFields);
                contents = contents.Replace("{setPropertyValue}", setPropertyValue);
                contents = contents.Replace("{primaryKey}", finalData.PrimaryKeyString);
                contents = contents.Replace("{initDatabaseModel}", initDatabaseModel);

                txtFile.WriteLine(contents);
            }
        }
        public void CreateUpdatePatchFile(string tableName, FinalDataPHP finalData)
        {
            string path = CreateDestinationPath(tableName + ",update_patch.php");
            string requiredFields = "";
            string setPropertyValue = "";
            var columns = finalData.SelectQueryData.ColumnList;
            foreach (var c in columns)
            {
                if (c.Extra == "auto_increment")
                {

                }
                else if (c.Extra == "CURRENT_TIMESTAMP")
                {

                }
                else if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    if (c.IsNull != null && c.IsNull.ToLower() == "no")
                    {
                        if (string.IsNullOrEmpty(requiredFields))
                        {
                            requiredFields = "!isEmpty($data->" + c.Field + ")";
                        }
                        else
                        {
                            requiredFields = requiredFields + Environment.NewLine + "&&" + "!isEmpty($data->" + c.Field + ")";
                        }
                        setPropertyValue = setPropertyValue + Environment.NewLine + "if(!isEmpty($data->" + c.Field + ")) { ";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "} else { ";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = '" + c.DefaultValue + "';";
                        setPropertyValue = setPropertyValue + Environment.NewLine + "}";
                    }
                    else
                    {
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                    }
                }
            }
            string initDatabaseModel = initDBStr();
            requiredFields = requiredFields.Trim('&').Trim('&');
            string contents = File.ReadAllText(CreateTemplatePath("modal,update_patch.txt"));
            using (var txtFile = File.AppendText(path))
            {
                if (string.IsNullOrEmpty(requiredFields))
                {
                    requiredFields = "true";
                }

                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{moduleName}", finalData.TableModuleName);
                contents = contents.Replace("{requiredFields}", requiredFields);
                contents = contents.Replace("{setPropertyValue}", setPropertyValue);
                contents = contents.Replace("{primaryKey}", finalData.PrimaryKeyString);
                contents = contents.Replace("{initDatabaseModel}", initDatabaseModel);

                txtFile.WriteLine(contents);
            }

        }

        public void CreateModalDeleteFile(string tableName, FinalDataPHP finalData)
        {
            string path = CreateDestinationPath(tableName + ",delete.php");
            string primaryKeyList = "";
            foreach (var p in finalData.PrimaryKeys)
            {
                primaryKeyList = primaryKeyList + "${tableName}->" + p.FieldName + " = $data->" + p.FieldName + ";" + Environment.NewLine;
            }
            string initDatabaseModel = initDBStr();
            string contents = File.ReadAllText(CreateTemplatePath("modal,delete.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{primaryKeyList}", primaryKeyList);
                contents = contents.Replace("{initDatabaseModel}", initDatabaseModel);
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{moduleName}", finalData.TableModuleName);
                txtFile.WriteLine(contents);
            }
        }
        public void CreateReadFile(string tableName, FinalDataPHP finalData)
        {
            string moduleName = ti.ToTitleCase(tableName);
            string path = CreateDestinationPath(tableName + ",read.php");
            string path_search = CreateDestinationPath(tableName + ",search.php");
           
            string path_search_byColumn = CreateDestinationPath(tableName + ",search_by_column.php");
            
            string path_one = CreateDestinationPath(tableName + ",read_one.php");
            
            string setPropertyValue = "";
            string setPropertyValue1 = "";

            foreach (var c in finalData.SelectQueryData.ColumnList)
            {
                if (c.FKDetails != null)
                {
                    var refColumns = pgSQLDB.GetColumns(c.FKDetails.REFERENCED_TABLE_NAME);
                    if (refColumns != null && refColumns.Count > 0)
                    {
                        var refColumnName = refColumns.Where(i => i.IsNull == "NO" && i.Key != "PRI" && i.TypeName.Contains("varchar")).FirstOrDefault();
                        if (refColumnName != null)
                        {
                            string input2 = refColumnName.TypeName;
                            string output2 = "-1";
                            string dataType2 = input2;
                            if (input2.Contains("("))
                            {
                                output2 = input2.Split('(', ')')[1];
                                dataType2 = input2.Split('(', ')')[0];
                                int length2 = 0;
                                int.TryParse(output2, out length2);
                                if (length2 <= 100)
                                {
                                    setPropertyValue = setPropertyValue + Environment.NewLine + "\"" + refColumnName.Field + "\" => $" + refColumnName.Field + ",";
                                    setPropertyValue1 = setPropertyValue1 + Environment.NewLine + "\"" + refColumnName.Field + "\" => $" + tableName + "->" + refColumnName.Field + ",";
                                }
                                else
                                {
                                    setPropertyValue = setPropertyValue + Environment.NewLine + "\"" + refColumnName.Field + "\" => html_entity_decode($" + refColumnName.Field + "),";
                                    setPropertyValue1 = setPropertyValue1 + Environment.NewLine + "\"" + refColumnName.Field + "\" => html_entity_decode($" + tableName + "->" + refColumnName.Field + "),";
                                }
                            }
                            else
                            {
                                setPropertyValue = setPropertyValue + Environment.NewLine + "\"" + refColumnName.Field + "\" => $" + refColumnName.Field + ",";
                                setPropertyValue1 = setPropertyValue1 + Environment.NewLine + "\"" + refColumnName.Field + "\" => $" + tableName + "->" + refColumnName.Field + ",";
                            }
                        }
                    }
                }
                string input = c.TypeName;
                string output = "-1";
                string dataType = input;
                if (input.Contains("("))
                {
                    output = input.Split('(', ')')[1];
                    dataType = input.Split('(', ')')[0];
                    int length = 0;
                    int.TryParse(output, out length);
                    if (length <= 100)
                    {
                        setPropertyValue = setPropertyValue + Environment.NewLine + "\"" + c.Field + "\" => $" + c.Field + ",";
                        setPropertyValue1 = setPropertyValue1 + Environment.NewLine + "\"" + c.Field + "\" => $" + tableName + "->" + c.Field + ",";
                    }
                    else
                    {
                        setPropertyValue = setPropertyValue + Environment.NewLine + "\"" + c.Field + "\" => html_entity_decode($" + c.Field + "),";
                        setPropertyValue1 = setPropertyValue1 + Environment.NewLine + "\"" + c.Field + "\" => html_entity_decode($" + tableName + "->" + c.Field + "),";
                    }
                }
                else
                {
                    setPropertyValue = setPropertyValue + Environment.NewLine + "\"" + c.Field + "\" => $" + c.Field + ",";
                    setPropertyValue1 = setPropertyValue1 + Environment.NewLine + "\"" + c.Field + "\" => $" + tableName + "->" + c.Field + ",";
                }
            }
            string initDatabaseModel = initDBStr();
            setPropertyValue = setPropertyValue.Trim(',');
            setPropertyValue1 = setPropertyValue1.Trim(',');
            string contents = File.ReadAllText(CreateTemplatePath("modal,read.txt"));
            using (var txtFile = File.AppendText(path))
            {

                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{moduleName}", moduleName);
                contents = contents.Replace("{setPropertyValue}", setPropertyValue);
                contents = contents.Replace("{initDatabaseModel}", initDatabaseModel);

                txtFile.WriteLine(contents);
            }
            string content_search = File.ReadAllText(CreateTemplatePath("modal,search.txt"));
            using (var txtFile = File.AppendText(path_search))
            {

                content_search = content_search.Replace("{tableName}", tableName);
                content_search = content_search.Replace("{moduleName}", moduleName);
                content_search = content_search.Replace("{setPropertyValue}", setPropertyValue);
                content_search = content_search.Replace("{initDatabaseModel}", initDatabaseModel);
                txtFile.WriteLine(content_search);
            }
            string content_search_byColumn = File.ReadAllText(CreateTemplatePath("modal,search_by_column.txt"));
            using (var txtFile = File.AppendText(path_search_byColumn))
            {
                content_search_byColumn = content_search_byColumn.Replace("{tableName}", tableName);
                content_search_byColumn = content_search_byColumn.Replace("{moduleName}", moduleName);
                content_search_byColumn = content_search_byColumn.Replace("{setPropertyValue}", setPropertyValue);
                content_search_byColumn = content_search_byColumn.Replace("{initDatabaseModel}", initDatabaseModel);
                txtFile.WriteLine(content_search_byColumn);
            }
            string contents_one = File.ReadAllText(CreateTemplatePath("modal,read_one.txt"));
            using (var txtFile = File.AppendText(path_one))
            {
                contents_one = contents_one.Replace("{tableName}", tableName);
                contents_one = contents_one.Replace("{moduleName}", moduleName);
                contents_one = contents_one.Replace("{setPropertyValue}", setPropertyValue1);
                contents_one = contents_one.Replace("{primaryKey}", finalData.PrimaryKeyString);
                contents_one = contents_one.Replace("{initDatabaseModel}", initDatabaseModel);

                txtFile.WriteLine(contents_one);
            }
            if (finalData.FKQueryDic != null)
            {
                foreach (var fk in finalData.FKQueryDic)
                {
                    string pathFK = CreateDestinationPath(tableName +","+ "read_by_" + Helper.RemoveSpecialCharacters(fk.Key).ToLower() + ".php");
                    string contentsFK = File.ReadAllText(CreateTemplatePath("modal,onetomany.txt"));
                    using (var txtFile = File.AppendText(pathFK))
                    {

                        contentsFK = contentsFK.Replace("{tableName}", tableName);
                        contentsFK = contentsFK.Replace("{moduleName}", moduleName);
                        contentsFK = contentsFK.Replace("{setPropertyValue}", setPropertyValue);
                        contentsFK = contentsFK.Replace("{fkColumnName}", fk.Key);
                        contentsFK = contentsFK.Replace("{functionName}", "readBy" + Helper.RemoveSpecialCharacters(fk.Key));
                        contentsFK = contentsFK.Replace("{initDatabaseModel}", initDatabaseModel);
                        txtFile.WriteLine(contentsFK);
                    }
                }
            }
        }
        public List<PostmanModel> CreatePostmanJson(string tableName, FinalDataPHP finalData, bool isView = false)
        {
            var insertUpdateData = finalData.InsertUpdateQueryData;
            var bodyJson = new List<PRequestBody>();
            foreach (var c in insertUpdateData.InsertColumnList)
            {
                if (bodyJson.Where(i => i.PropName == c.FieldName).FirstOrDefault() == null)
                    bodyJson.Add(new PRequestBody(c.FieldName, c.DataType, c.isRequired, c.DefaultValue));
            }
            List<PostmanModel> postmanJson = new List<PostmanModel>();
            //ReadAll
            PostmanModel readAll = new PostmanModel();
            readAll.TableName = tableName;
            readAll.Name = ti.ToTitleCase(tableName) + " - GETALL";
            readAll.Method = "GET";
            readAll.Path = new List<string>()
            {
                tableName,
                "read.php?pageno=1&pagesize=30"
            };
            //Search
            PostmanModel psearch = new PostmanModel();
            psearch.TableName = tableName;
            psearch.Name = ti.ToTitleCase(tableName) + " - Search";
            psearch.Method = "GET";
            psearch.Path = new List<string>()
            {
                tableName,
                "search.php?key=abc&pageno=1&pagesize=30"
            };

            //ReadOne
            PostmanModel readOne = new PostmanModel();
            readOne.TableName = tableName;
            readOne.Name = ti.ToTitleCase(tableName) + " - GETByID";
            readOne.Method = "GET";
            readOne.Path = new List<string>()
            {
                tableName,
                "read_one.php?id=${id}"
            };
            //Delete
            PostmanModel delete = new PostmanModel();
            delete.TableName = tableName;
            delete.Name = ti.ToTitleCase(tableName) + " - Delete";
            delete.Method = "POST";
            delete.Path = new List<string>()
            {
                tableName,
                "delete.php"
            };
            delete.Body = new List<PRequestBody>();
            foreach (var p in finalData.PrimaryKeys)
            {
                delete.Body.Add(new PRequestBody(p.FieldName, "int", true, "1"));
            }

            //Create
            PostmanModel create = new PostmanModel();
            create.TableName = tableName;
            create.Name = ti.ToTitleCase(tableName) + " - Add New";
            create.Method = "POST";
            create.Path = new List<string>()
            {
                tableName,
                "create.php"
            };
            create.Body = bodyJson.Distinct().ToList();

            //Update
            PostmanModel update = new PostmanModel();
            update.TableName = tableName;
            update.Name = ti.ToTitleCase(tableName) + " - Update";
            update.Method = "POST";
            update.Path = new List<string>()
            {
                tableName,
                "update.php"
            };
            update.Body = bodyJson.Distinct().ToList();

            //UpdatePatch
            PostmanModel updatePatch = new PostmanModel();
            updatePatch.TableName = tableName;
            updatePatch.Name = ti.ToTitleCase(tableName) + " - Update Patch";
            updatePatch.Method = "POST";
            updatePatch.Path = new List<string>()
            {
                tableName,
                "update_patch.php"
            };
            updatePatch.Body = bodyJson.Distinct().ToList();
            foreach (var x in insertUpdateData.PrimaryKeys)
            {
                update.Body.Add(new PRequestBody(x.FieldName, x.DataType, true, ""));
                updatePatch.Body.Add(new PRequestBody(x.FieldName, x.DataType, true, ""));
            }
            //SearchByColumn
            PostmanModel searchByCol = new PostmanModel();
            searchByCol.TableName = tableName;
            searchByCol.Name = ti.ToTitleCase(tableName) + " - SearchByColumn";
            searchByCol.Method = "POST";
            searchByCol.Path = new List<string>()
            {
                tableName,
                "search_by_column.php?orAnd=OR&pageno=1&pagesize=30"
            };
            searchByCol.Body = bodyJson.Distinct().ToList();
            postmanJson.Add(readAll);
            postmanJson.Add(readOne);
            postmanJson.Add(psearch);
            postmanJson.Add(searchByCol);
            if (!isView)
            {
                postmanJson.Add(create);
                postmanJson.Add(update);
                postmanJson.Add(updatePatch);
                postmanJson.Add(delete);
            }
            return postmanJson;
        }
        public void CreatePostmanFile(List<PostmanModel> postmanList)
        {
            Exception ex;
            var postmanItemString = PostmanGenerator.GeneratePostmanJson(postmanList, out ex);
            string path = CreateDestinationPath("POSTMAN_IMPORT_FILE,postman_import_file.json");
            string path2 = CreateDestinationPath("POSTMAN_IMPORT_FILE,EnvironmentVariable_ImportThisAlso.postman_environment.json");
            string contents = File.ReadAllText(CreateTemplatePath("PostmanJson,Postmanjson_template.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{newGuid}", Guid.NewGuid().ToString());
                contents = contents.Replace("{projectName}", ProjectName);
                contents = contents.Replace("{itemListString}", postmanItemString);
                txtFile.WriteLine(contents);
            }
            string contents2 = File.ReadAllText(CreateTemplatePath("PostmanJson,EnvironmentVariable.postman_environment.txt"));
            using (var txtFile = File.AppendText(path2))
            {
                contents2 = contents2.Replace("{projectName}", ProjectName);
                txtFile.WriteLine(contents2);
            }
        }
        
        public void CreateIndexFile()
        {
            string pathHTaccess = CreateDestinationPath(".htaccess");
            string path = CreateDestinationPath("index.php");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string completeTableString = "";
          
            foreach (var t in SelectedTable)
            {
                bool isView = false;
                string table = t;
                if (table.StartsWith("View - "))
                {
                    isView = true;
                    table = table.Replace("View - ", "");
                }
                string moduleName = ti.ToTitleCase(table);
                string tableStrig = "<table class='table table-striped table-condensed'> <thead> <tr><th colspan='3'><div class='p-3 mb-2 bg-primary text-white'><h3><b>" + moduleName + "</b></h3></div><tr></th>";
                tableStrig = tableStrig + "</thead><tbody>";
                tableStrig = tableStrig + "<tr><td>Read</td><td>GET</td><td>/" + table + "/read.php?pageno=1&pagesize=30</td></tr>";
                tableStrig = tableStrig + "<tr><td>Read One</td><td>GET</td><td>/" + table + "/read_one.php?id=1</td></tr>";
                tableStrig = tableStrig + "<tr><td>Search</td><td>GET</td><td>/" + table + "/search.php?key=key&pageno=1&pagesize=30</td></tr>";
                tableStrig = tableStrig + "<tr><td>Dynamic Search</td><td>POST</td><td>/" + table + "/search_by_column.php</td></tr>";
                if (!isView)
                {
                    tableStrig = tableStrig + "<tr><td>Create</td><td>POST</td><td>/" + table + "/create.php</td></tr>";
                    tableStrig = tableStrig + "<tr><td>Update</td><td>POST</td><td>/" + table + "/update.php</td></tr>";
                    tableStrig = tableStrig + "<tr><td>Update Patch</td><td>POST</td><td>/" + table + "/update_patch.php</td></tr>";
                    tableStrig = tableStrig + "<tr><td>Delete</td><td>POST</td><td>/" + table + "/delete.php</td></tr>";
                }
                tableStrig = tableStrig + "</tbody></table>";
                completeTableString = completeTableString + Environment.NewLine + tableStrig;
            }
            string contents = File.ReadAllText(CreateTemplatePath("index.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{projectName}", ProjectName);
                contents = contents.Replace("{tableAPI}", completeTableString);
                txtFile.WriteLine(contents);
            }
            string contents2 = File.ReadAllText(CreateTemplatePath("htaccess.txt"));
            using (var txtFile = File.AppendText(pathHTaccess))
            {
                txtFile.WriteLine(contents2);
            }
        }
    }
}
