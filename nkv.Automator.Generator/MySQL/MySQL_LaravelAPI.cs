using nkv.Automator.Generator.Models;
using nkv.Automator.Models;
using nkv.Automator.Postman;
using nkv.Automator.Utility;
using System.Globalization;

namespace nkv.Automator.MySQL
{
    public class MySQL_LaravelAPI
    {
        List<PostmanModel> postmanJson = new List<PostmanModel>();
        public Action<NKVMessage> MessageEvent { get; set; } = null!;
        public Action<NKVMessage> CompletedEvent { get; set; } = null!;
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        public string TemplateFolder { get; set; } = null!;
        public string TemplateFolderSeparator { get; set; } = null!;
        public string DestinationFolderSeparator { get; set; } = null!;
        public string DestinationFolder { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        public List<Exception> ExceptionList { get; set; }
        public List<string> SelectedTable { get; set; } = null!;
        public NKVConfiguration ConfigApp { get; set; } = null!;
        public bool IsMultiTenant { get; set; }
        public MySQLDBHelper mysqlDB { get; set; } = null!;
        public MySQL_LaravelAPI(NKVConfiguration config, bool isMultiTenant, string destinationFolderSeparator)
        {
            IsMultiTenant = isMultiTenant;
            ConfigApp = config;
            ExceptionList = new List<Exception>();
            TemplateFolder = "LaravelAPITemplate";
            TemplateFolderSeparator = "\\";
            DestinationFolderSeparator = destinationFolderSeparator;
        }
        private string CreateTemplatePath(string filePathString)
        {
            string path = TemplateFolder;
            foreach (var p in filePathString.Split(","))
            {
                if (!string.IsNullOrEmpty(p))
                    path = path + TemplateFolderSeparator + p;
            }
            return path;
        }
        private string CreateDestinationPath(string filePathString)
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

        public ReactJSInput<FinalQueryData> Automator(string projectName, List<string> selectedTable, MySQLDBHelper mySQLDB)
        {
            mysqlDB = mySQLDB;
            SelectedTable = selectedTable;
            ProjectName = projectName;
            string projectFolder = CreateDirectory();
            MessageEvent?.Invoke(new NKVMessage("Project Folder Created : " + projectFolder));
            MessageEvent?.Invoke(new NKVMessage("Generating Larvel Project"));
            CopyProject();
            ReactJSInput<FinalQueryData> reactInput = new ReactJSInput<FinalQueryData>()
            {
                DestinationFolder = projectFolder,
                FinalDataDic = new Dictionary<string, FinalQueryData>(),
                PostmanJson = new List<PostmanModel>()
            };
            foreach (string tableName in selectedTable)
            {
                try
                {
                    MessageEvent?.Invoke(new NKVMessage("Processing for Table => " + tableName));
                    string modelName = Helper.ToPascalCase(tableName);
                    var insertUpdateData = mysqlDB.GetInsertUpdateQueryData(tableName);
                    var finalData = mysqlDB.BuildLaravelQuery(tableName);
                    reactInput.FinalDataDic.Add(tableName, finalData);
                    if (tableName == ConfigApp.AuthTableConfig.AuthTableName)
                    {
                        GenerateAuthModels(tableName, insertUpdateData);
                    }
                    else
                    {
                        GenerateModels(tableName, insertUpdateData, finalData);
                    }
                    GenerateController(tableName, insertUpdateData, finalData);

                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "add", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "update", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "get", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "getbyid", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "search", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "delete", finalData));
                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage("Exception on table " + tableName, false));
                    MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                }
            }

            GenerateAuthFile(ConfigApp.AuthTableConfig.AuthTableName);
            MessageEvent?.Invoke(new NKVMessage("JWT Token Auth generated"));
            InsertUpdateQueryData authData = mysqlDB.GetInsertUpdateQueryData(ConfigApp.AuthTableConfig.AuthTableName);
            GenerateAuthController(ConfigApp.AuthTableConfig.AuthTableName, authData);
            GenerateRoutes();
            MessageEvent?.Invoke(new NKVMessage("Routes created"));
            CreatePostmanJsonFile(postmanJson, authData);
            MessageEvent?.Invoke(new NKVMessage("Postman import collection file generated"));
            reactInput.PostmanJson = postmanJson;
            GenerateEnvFile();
            MessageEvent?.Invoke(new NKVMessage("Env file created"));
            GenerateDatabaseFile();
            MessageEvent?.Invoke(new NKVMessage("Database file created"));
            MessageEvent?.Invoke(new NKVMessage("----- Laravel API Generated -----"));
            CompletedEvent?.Invoke(new NKVMessage("Thanks for using GetAutomator.com! Please check the generated code at : " + DestinationFolder));
            return reactInput;
        }
        public string CreateDirectory()
        {
            string destFile = ProjectName;
            string newName = destFile;
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

            destFile = newName;
            string targetDirectory = destFile + "\\LaravelAPI";
            Directory.CreateDirectory(newName);
            DestinationFolder = targetDirectory;
            return destFile;
        }
        public void CopyProject()
        {
            string sourceDirectory = TemplateFolder + @"\\LaravelProject";
            Directory.CreateDirectory(DestinationFolder + "\\POSTMAN_IMPORT_FILE");
            CopyDir.Copy(sourceDirectory, DestinationFolder, ProjectName.ToLower(), "nkv.LaravelProject");
            Directory.CreateDirectory(DestinationFolder + "\\app\\Models");
            Directory.CreateDirectory(DestinationFolder + "\\app\\Http\\Controllers\\Api\\Auth");
        }
        public void GenerateRoutes()
        {
            string path = CreateDestinationPath("routes,api.php");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var apiRoutes = "";
            foreach (var r in SelectedTable)
            {
                var model = Helper.ToPascalCase(r);
                apiRoutes = apiRoutes + Environment.NewLine + @"Route::get('/" + r + @"', 'App\Http\Controllers\Api\Auth\" + model + "Controller@index')->name('api.auth.index." + r + "');";
                apiRoutes = apiRoutes + Environment.NewLine + @"Route::get('/" + r + @"/{id}', 'App\Http\Controllers\Api\Auth\" + model + "Controller@show')->name('api.auth.show." + r + "');";
                apiRoutes = apiRoutes + Environment.NewLine + @"Route::post('/" + r + @"', 'App\Http\Controllers\Api\Auth\" + model + "Controller@store')->name('api.auth.store." + r + "');";
                apiRoutes = apiRoutes + Environment.NewLine + @"Route::put('/" + r + @"/{id}', 'App\Http\Controllers\Api\Auth\" + model + "Controller@update')->name('api.auth.update." + r + "');";
                apiRoutes = apiRoutes + Environment.NewLine + @"Route::delete('/" + r + @"/{id}', 'App\Http\Controllers\Api\Auth\" + model + "Controller@destroy')->name('api.auth.delete." + r + "');";
                apiRoutes = apiRoutes + Environment.NewLine + @"Route::get('/" + r + @"/search/{search}', 'App\Http\Controllers\Api\Auth\" + model + "Controller@search')->name('api.auth.search." + r + "');";
            }
            string contents = File.ReadAllText(CreateTemplatePath("api.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{apiRoutes}", apiRoutes);
                txtFile.WriteLine(contents);
            }
        }

        public void GenerateAuthFile(string authTable)
        {
            string authTableModel = ti.ToTitleCase(authTable);
            string path = CreateDestinationPath("config,auth.php");
            string contents = File.ReadAllText(CreateTemplatePath("auth.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{userModelName}", authTableModel);
                txtFile.WriteLine(contents);
            }
        }

        public void GenerateEnvFile()
        {
            string path = CreateDestinationPath(".env");
            string contents = File.ReadAllText(CreateTemplatePath("env.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{projectName}", ProjectName);
                contents = contents.Replace("{dbhost}", mysqlDB.Host);
                contents = contents.Replace("{dbport}", mysqlDB.Port.ToString());
                contents = contents.Replace("{dbname}", mysqlDB.DBName);
                contents = contents.Replace("{dbusername}", mysqlDB.Username);
                contents = contents.Replace("{dbpassword}", mysqlDB.Password);
                txtFile.WriteLine(contents);
            }

        }

        public void GenerateDatabaseFile()
        {
            string path = CreateDestinationPath("config,database.php");
            string contents = File.ReadAllText(CreateTemplatePath("database.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{projectName}", ProjectName);
                contents = contents.Replace("{dbhost}", mysqlDB.Host);
                contents = contents.Replace("{dbport}", mysqlDB.Port.ToString());
                contents = contents.Replace("{dbname}", mysqlDB.DBName);
                contents = contents.Replace("{dbusername}", mysqlDB.Username);
                contents = contents.Replace("{dbpassword}", mysqlDB.Password);
                txtFile.WriteLine(contents);
            }

        }
        public void GenerateModels(string tableName, InsertUpdateQueryData data, FinalQueryData finalData)
        {
            var model = Helper.ToPascalCase(tableName);
            string path2 = CreateDestinationPath("app,Models,model.txt");
            string path = CreateDestinationPath("app,Models," + model + ".php");
            var castProperty = "";
            var requiredColumns = "";
            var foreignKeyRelationship = "";
            var fkFunction = "public function $fkTableNameFun()" + Environment.NewLine + "{" + Environment.NewLine + "return $this->hasOne($FKTableName::class,'$FKey','$LocalKey')->select(['$FKey','$FieldName']);" + Environment.NewLine + "}";

            if (finalData != null && finalData.SelectQueryData != null && finalData.SelectQueryData.FKColumnData != null)
            {

                foreach (var c in finalData.SelectQueryData.FKColumnData)
                {
                    if (!foreignKeyRelationship.Contains(c.TableName2))
                    {
                        if (c.FieldName2 != null)
                        {
                            var fk = fkFunction;
                            fk = fk.Replace("$fkTableNameFun", c.TableName2);
                            fk = fk.Replace("$FKTableName", Helper.ToPascalCase(c.TableName2));
                            fk = fk.Replace("$FKey", c.FieldName1);
                            fk = fk.Replace("$FieldName", c.FieldName2);
                            fk = fk.Replace("$LocalKey", c.LocalField);
                            foreignKeyRelationship = foreignKeyRelationship + Environment.NewLine + fk;
                        }
                    }
                }
            }
            foreach (var c in data.InsertColumnList)
            {
                requiredColumns = requiredColumns + "'" + c.FieldName + "',";
            }
            var tCol = data.ColumnList.Where(i => i.DefaultValue == "CURRENT_TIMESTAMP" || i.TypeName.ToLower().Contains("timestamp")).ToList();
            if (tCol != null && tCol.Count > 0)
            {
                foreach (var r in tCol)
                {
                    castProperty = castProperty + Environment.NewLine + "'" + r.Field + "' => 'datetime',";
                }
            }
            foreach (var c in data.ColumnList)
            {
                var dType = Helper.GetDataTypePHP(c.TypeName);
                if (dType == "boolean")
                {
                    castProperty = castProperty + Environment.NewLine + "'" + c.Field + "' => 'boolean',";
                }
            }
            string primaryKey = string.Empty;
            if (data.PrimaryKeys != null && data.PrimaryKeys.Count > 1)
            {
                //foreach (var r in data.PrimaryKeys)
                //{
                //    primaryKey = primaryKey + "'" + r.FieldName + "',";
                //}
                //primaryKey = "[" + primaryKey.Trim(',') + "]";
                primaryKey = "'" + data.PrimaryKeys[0].FieldName + "'";
            }
            else if (data.PrimaryKeys != null && data.PrimaryKeys.Count == 1)
            {
                primaryKey = "'" + data.PrimaryKeys[0].FieldName + "'";
            }
            else
            {
                primaryKey = "'id'";
            }
            string contents = File.ReadAllText(CreateTemplatePath("model.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{requiredProperty}", requiredColumns.Trim(','));
                contents = contents.Replace("{castProperty}", castProperty);
                contents = contents.Replace("{foreignKeyRelationship}", foreignKeyRelationship);

                contents = contents.Replace("{modelName}", model);
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{primaryKey}", primaryKey);
                txtFile.WriteLine(contents);
            }
        }
        public void GenerateAuthModels(string tableName, InsertUpdateQueryData data)
        {
            var model = Helper.ToPascalCase(tableName);
            string path = CreateDestinationPath("app,Models," + model + ".php");
            string castProperty = "";
            string requiredColumns = "";
            string hiddenProperty = "'" + ConfigApp.AuthTableConfig.PasswordColumnName + "'";
            var tCol = data.ColumnList.Where(i => i.DefaultValue == "CURRENT_TIMESTAMP").ToList();

            if (tCol != null && tCol.Count > 0)
            {
                foreach (var r in tCol)
                {
                    castProperty = castProperty + Environment.NewLine + "'" + r.Field + "' => 'datetime',";
                }
            }


            foreach (var c in data.InsertColumnList)
            {
                requiredColumns = requiredColumns + "'" + c.FieldName + "',";
            }

            string primaryKey = string.Empty;
            if (data.PrimaryKeys != null && data.PrimaryKeys.Count > 1)
            {
                //foreach (var r in data.PrimaryKeys)
                //{
                //    primaryKey = primaryKey + "'" + r.FieldName + "',";
                //}
                //primaryKey = "[" + primaryKey.Trim(',') + "]";
                primaryKey = "'" + data.PrimaryKeys[0].FieldName + "'";
            }
            else if (data.PrimaryKeys != null && data.PrimaryKeys.Count == 1)
            {
                primaryKey = "'" + data.PrimaryKeys[0].FieldName + "'";
            }
            else
            {
                primaryKey = "'id'";
            }
            foreach (var c in data.ColumnList)
            {
                var dType = Helper.GetDataTypePHP(c.TypeName);
                if (dType == "boolean")
                {
                    castProperty = castProperty + Environment.NewLine + "'" + c.Field + "' => 'boolean',";
                }
            }

            string contents = File.ReadAllText(CreateTemplatePath("authmodel.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{requiredProperty}", requiredColumns.Trim(','));
                contents = contents.Replace("{castProperty}", castProperty);
                contents = contents.Replace("{hiddenProperty}", hiddenProperty);
                contents = contents.Replace("{foreignKeyRelationship}", "");

                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{modelName}", model);
                contents = contents.Replace("{primaryKey}", primaryKey);
                contents = contents.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                txtFile.WriteLine(contents);
            }
        }
        public void GenerateController(string tableName, InsertUpdateQueryData data, FinalQueryData finalData)
        {
            var model = Helper.ToPascalCase(tableName);

            string path = CreateDestinationPath("app,Http,Controllers,Api,Auth," + model + "Controller.php");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var updateProperty = string.Empty;
            foreach (var c in data.UpdateColumnList)
            {
                updateProperty = updateProperty + "$" + tableName + "->" + c.FieldName + " = $input['" + c.FieldName + "'];";
            }
            string primaryKey = string.Empty;
            if (data.PrimaryKeys != null && data.PrimaryKeys.Count > 1)
            {
                primaryKey = data.PrimaryKeys[0].FieldName;
            }
            else if (data.PrimaryKeys != null && data.PrimaryKeys.Count == 1)
            {
                primaryKey = data.PrimaryKeys[0].FieldName;
            }
            else
            {
                primaryKey = "id";
            }
            var requiredColumns = "";
            foreach (var c in data.ColumnList)
            {
                requiredColumns = requiredColumns + "'" + c.Field + "',";
            }
            var SelectAllQuery = "";
            //$state = State::with(['country'])->paginate($request->paginator);
            var selectArray = new List<string>();
            var withOperator = "::";
            if (finalData != null && finalData.SelectQueryData != null && finalData.SelectQueryData.FKColumnData != null)
            {
                foreach (var c in finalData.SelectQueryData.FKColumnData)
                {
                    if (!SelectAllQuery.Contains(c.TableName2))
                    {
                        if (c.FieldName2 != null)
                        {
                            selectArray.Add("'" + c.TableName2 + "'");
                        }
                    }
                }
            }
            if (selectArray.Count() > 0)
            {
                SelectAllQuery = model + "::with([" + string.Join(",", selectArray) + "])->paginate($request->paginator)";
                withOperator = "::with([" + string.Join(",", selectArray) + "])->";
            }
            else
            {
                SelectAllQuery = model + "::paginate($request->paginator, ['*'], 'page', $request->page)";
            }

            //::with(['country'])->
            string contents = File.ReadAllText(CreateTemplatePath("controller.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{requiredProperty}", requiredColumns.Trim(','));
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{updateProperty}", updateProperty);
                contents = contents.Replace("{modelName}", model);
                contents = contents.Replace("{primaryKey}", primaryKey);
                contents = contents.Replace("{SelectAllQuery}", SelectAllQuery);
                contents = contents.Replace("{withOperator}", withOperator);
                txtFile.WriteLine(contents);
            }

        }

        public void GenerateAuthController(string tableName, InsertUpdateQueryData data)
        {
            var model = Helper.ToPascalCase(tableName);

            string path = CreateDestinationPath("app,Http,Controllers,AuthController.php");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var requiredProperty = "";
            string passwordValidation = "'password' => 'required|string|min:6',";
            string usernameValidation = "";
            if (ConfigApp.AuthTableConfig.IsEmail)
            {
                usernameValidation = "'" + ConfigApp.AuthTableConfig.UsernameColumnName + "' => 'required|email',";
            }
            else
            {
                usernameValidation = "'" + ConfigApp.AuthTableConfig.UsernameColumnName + "' => 'required|string|min:6',";
            }
            foreach (var c in data.ColumnList)
            {
                if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    string required = "";
                    if (c.IsNull == "NO")
                    {
                        required = "required|";
                    }
                    if (c.Field == ConfigApp.AuthTableConfig.UsernameColumnName)
                    {
                        if (ConfigApp.AuthTableConfig.IsEmail)
                        {
                            requiredProperty = requiredProperty + Environment.NewLine + "'" + c.Field + "' => '" + required + "string|email|max:100',";
                        }
                        else
                        {
                            requiredProperty = requiredProperty + Environment.NewLine + "'" + c.Field + "' => '" + required + "string|between:2,100',";
                        }
                    }
                    else if (c.Field == ConfigApp.AuthTableConfig.PasswordColumnName)
                    {
                        requiredProperty = requiredProperty + Environment.NewLine + "'" + ConfigApp.AuthTableConfig.PasswordColumnName + "' => '" + required + "string|confirmed|min:6',";
                    }
                    else
                    {
                        var dType = Helper.GetDataTypePHP(c.TypeName);
                        if (dType.ToLower() == "datetime")
                        {
                            requiredProperty = requiredProperty + Environment.NewLine + "'" + c.Field + "' => '" + required + "date_format:Y-m-d H:i:s',";
                        }
                        else
                        {
                            requiredProperty = requiredProperty + Environment.NewLine + "'" + c.Field + "' => '" + required + "" + dType + "',";
                        }
                    }
                }

            }

            string contents = File.ReadAllText(CreateTemplatePath("authcontroller.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{modelName}", model);
                contents = contents.Replace("{usernameValidation}", usernameValidation);
                contents = contents.Replace("{requiredProperty}", requiredProperty);
                contents = contents.Replace("{passwordValidation}", passwordValidation);
                contents = contents.Replace("{userColumn}", ConfigApp.AuthTableConfig.UsernameColumnName);
                contents = contents.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                txtFile.WriteLine(contents);
            }

        }

        public PostmanModel CreatePostmanJson(string tableName, string modelName, string type, FinalQueryData data)
        {
            PostmanModel p = new PostmanModel();
            var pathList = new List<string>();
            var bodyList = new List<PRequestBody>();
            pathList.Add(tableName);
            p.TableName = tableName;

            switch (type)
            {
                case "add":
                    p.Name = modelName + " - Add";
                    p.Method = "POST";
                    break;
                case "update":
                    p.Name = modelName + " - Update";
                    p.Method = "PUT";
                    if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count > 1)
                    {
                        pathList.Add("${" + data.SelectQueryData.PrimaryKeys[0].FieldName + "}");
                    }
                    else if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count == 1)
                    {
                        string pKeyOne = data.SelectQueryData.PrimaryKeys[0].FieldName;
                        pathList.Add("${" + pKeyOne + "}");
                    }
                    else
                    {
                        string pKeyOne = data.SelectQueryData.ColumnList[0].Field;
                        pathList.Add("${" + pKeyOne + "}");
                    }
                    break;
                case "delete":
                    p.Name = modelName + " - Delete";
                    p.Method = "DELETE";
                    if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count > 1)
                    {
                        pathList.Add("${" + data.SelectQueryData.PrimaryKeys[0].FieldName + "}");
                    }
                    else if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count == 1)
                    {
                        string pKeyOne = data.SelectQueryData.PrimaryKeys[0].FieldName;
                        pathList.Add("${" + pKeyOne + "}");
                    }
                    else
                    {
                        string pKeyOne = data.SelectQueryData.ColumnList[0].Field;
                        pathList.Add("${" + pKeyOne + "}");
                    }
                    break;
                case "get":
                    p.Name = modelName + " - GetAll";
                    pathList.Add("?page=1&paginator=100");
                    p.Method = "GET";
                    break;
                case "getbyid":
                    p.Name = modelName + " - GetById";
                    if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count > 1)
                    {
                        pathList.Add("${" + data.SelectQueryData.PrimaryKeys[0].FieldName + "}");
                    }
                    else if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count == 1)
                    {
                        string pKeyOne = data.SelectQueryData.PrimaryKeys[0].FieldName;
                        pathList.Add("${" + pKeyOne + "}");
                    }
                    else
                    {
                        string pKeyOne = data.SelectQueryData.ColumnList[0].Field;
                        pathList.Add("${" + pKeyOne + "}");
                    }
                    // pathList.Add("?pageNo=1&pageSize=30");
                    p.Method = "GET";
                    break;
                case "search":
                    p.Name = modelName + " - Search";
                    pathList.Add("search");
                    pathList.Add("${searchKey}");
                    pathList.Add("?page=1&paginator=30");
                    p.Method = "GET";
                    break;
            }
            p.Path = pathList;
            foreach (var c in data.SelectQueryData.ColumnList)
            {
                if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    if (bodyList.FirstOrDefault(i => i.PropName == c.Field) == null)
                        bodyList.Add(new PRequestBody(c.Field, c.TypeName, (c.IsNull == "NO" ? true : false), c.DefaultValue));
                }
            }
            if (type == "update" || type == "add")
            {
                if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count > 1)
                {
                    foreach (var pKey in data.SelectQueryData.PrimaryKeys)
                    {
                        if (bodyList.FirstOrDefault(i => i.PropName == pKey.FieldName) == null)
                            bodyList.Add(new PRequestBody(pKey.FieldName, pKey.DataType, false, null));
                    }
                }
                else if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count == 1)
                {
                    var pkey = data.SelectQueryData.PrimaryKeys[0];
                    if (bodyList.FirstOrDefault(i => i.PropName == pkey.FieldName) == null)
                        bodyList.Add(new PRequestBody(pkey.FieldName, pkey.DataType, false, null));
                }
                else
                {
                    var c = data.SelectQueryData.ColumnList[0];
                    if (bodyList.FirstOrDefault(i => i.PropName == c.Field) == null)
                        bodyList.Add(new PRequestBody(c.Field, c.TypeName, (c.IsNull == "NO" ? true : false), c.DefaultValue));
                }
            }
            p.Body = bodyList;
            return p;
        }
        public void CreatePostmanJsonFile(List<PostmanModel> postmanModels, InsertUpdateQueryData authTableData)
        {
            try
            {
                var exc = new Exception();
                var postmanItemString = PostmanGenerator.GeneratePostmanJson(postmanModels, out exc);
                string path = CreateDestinationPath("POSTMAN_IMPORT_FILE,postman_import_file.json");
                string path2 = CreateDestinationPath("POSTMAN_IMPORT_FILE,EnvironmentVariable_ImportThisAlso.postman_environment.json");
                var bodyList = new List<PRequestBody>();
                foreach (var c in authTableData.InsertColumnList)
                {
                    bodyList.Add(new PRequestBody(c.FieldName, c.DataType, false, null));
                }
                bodyList.Add(new PRequestBody(ConfigApp.AuthTableConfig.PasswordColumnName + "_confirmation", "string", false, null));

                string bodyString = "{";
                foreach (var f in bodyList)
                {
                    bodyString = bodyString + "\\\"" + f.PropName + "\\\":\\\"\\\"" + ",";
                }
                bodyString = bodyString.Trim(',');
                bodyString = bodyString + "}";

                string contents = File.ReadAllText(CreateTemplatePath("PostmanJson,Postmanjson_template.txt"));
                using (var txtFile = File.AppendText(path))
                {
                    contents = contents.Replace("{newGuid}", Guid.NewGuid().ToString());
                    contents = contents.Replace("{projectName}", ProjectName);
                    contents = contents.Replace("{itemListString}", postmanItemString);
                    contents = contents.Replace("{userRequiredProperty}", bodyString);

                    txtFile.WriteLine(contents);
                }

                string contents2 = File.ReadAllText(CreateTemplatePath("PostmanJson,EnvironmentVariable.postman_environment.txt"));
                using (var txtFile = File.AppendText(path2))
                {
                    contents2 = contents2.Replace("{projectName}", ProjectName);
                    txtFile.WriteLine(contents2);
                }
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                ExceptionList.Add(ex);
            }
        }

    }
}
