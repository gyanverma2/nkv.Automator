using nkv.Automator.Generator.Models;
using nkv.Automator.Models;
using nkv.Automator.MySQL;
using nkv.Automator.PGSQL;
using nkv.Automator.Postman;
using nkv.Automator.Utility;
using System.Globalization;

namespace nkv.Automator.Generator.PGSQL
{
    public class PGSQL_NodeJSAPI
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
        public PGSQLDBHelper pgsqlDB { get; set; } = null!;
        public PGSQL_NodeJSAPI(NKVConfiguration config, bool isMultiTenant, string destinationFolderSeparator)
        {
            IsMultiTenant = isMultiTenant;
            ConfigApp = config;
            ExceptionList = new List<Exception>();
            TemplateFolder = "PGSQLNodeJSAPITemplate";
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
        public ReactJSInput<FinalQueryData> Automator(string projectName, List<string> selectedTable, PGSQLDBHelper pgSQLDB)
        {
            pgsqlDB = pgSQLDB;
            SelectedTable = selectedTable;
            ProjectName = projectName;
            string projectFolder = CreateDirectory();
            ReactJSInput<FinalQueryData> reactInput = new ReactJSInput<FinalQueryData>()
            {
                DestinationFolder = projectFolder,
                FinalDataDic = new Dictionary<string, FinalQueryData>(),
                PostmanJson = new List<PostmanModel>()
            };

            MessageEvent?.Invoke(new NKVMessage("Project Folder Created : " + projectFolder));
            MessageEvent?.Invoke(new NKVMessage("Generating NodeJS API"));
            CreateEnvFiles();
            MessageEvent?.Invoke(new NKVMessage("Env file created"));
            foreach (string tableName in selectedTable)
            {
                try
                {
                    MessageEvent?.Invoke(new NKVMessage("Processing for Table => " + tableName));
                    string modelName = Helper.ToPascalCase(tableName);
                    var finalData = pgsqlDB.BuildQueryNodeJS(tableName);
                    reactInput.FinalDataDic.Add(tableName, finalData);
                    CreateController(tableName, finalData);
                    CreateTableRoute(tableName, finalData);
                    CreateTableDTO(tableName, finalData);
                    CreateModelSQLFile(tableName, finalData);
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "add", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "patch", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "update", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "get", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "getbyid", finalData));
                    if (finalData.SelectByFKQuery.Count > 0)
                    {
                        foreach (var k in finalData.SelectByFKQuery)
                        {
                            postmanJson.Add(CreatePostmanJson(tableName, modelName, "getbyfk", finalData, k.ColumnName));
                        }
                    }
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "search", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "searchByColumn", finalData));
                    postmanJson.Add(CreatePostmanJson(tableName, modelName, "delete", finalData));
                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage("Exception on table " + tableName, false));
                    MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                }
            }
            CreateDatabaseFiles();
            CreateTokenFiles();
            MessageEvent?.Invoke(new NKVMessage("Jwt token controller created"));
            CreateUploadFiles();
            MessageEvent?.Invoke(new NKVMessage("Upload file controller created"));
            CreateMainRoute();
            MessageEvent?.Invoke(new NKVMessage("Routes file created"));
            CreateAppFiles();
            CreateMiddleware();
            CreateUtilFiles();
            CreatePackageJsonFiles();
            CreatePostmanJsonFile();
            reactInput.PostmanJson = postmanJson;
            MessageEvent?.Invoke(new NKVMessage("Postman json file created"));
            MessageEvent?.Invoke(new NKVMessage("----- NodeJS API Generated -----"));
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
            string targetDirectory = newName + "\\NodeJSAPI";
            Directory.CreateDirectory(newName);
            DestinationFolder = targetDirectory;
            Directory.CreateDirectory(targetDirectory);
            Directory.CreateDirectory(targetDirectory + "//controllers");
            Directory.CreateDirectory(targetDirectory + "//database");
            Directory.CreateDirectory(targetDirectory + "//dto");
            Directory.CreateDirectory(targetDirectory + "//middleware");
            Directory.CreateDirectory(targetDirectory + "//uploads");
            Directory.CreateDirectory(targetDirectory + "//routes");
            Directory.CreateDirectory(targetDirectory + "//utils");
            Directory.CreateDirectory(targetDirectory + "//models");
            Directory.CreateDirectory(targetDirectory + "//POSTMAN_IMPORT_FILE//");
            return destFile;
        }
        public void CreateController(string tableName, FinalQueryData data)
        {

            string path = CreateDestinationPath("controllers," + tableName + ".js");
            string primaryKeyInit = "";
            string primaryKeyParam = "";
            if (data.PrimaryKeys.Count > 0)
            {
                foreach (var p in data.PrimaryKeys)
                {
                    primaryKeyInit = primaryKeyInit + "const " + p.FieldName + " = " + "req.params." + p.FieldName + ";" + Environment.NewLine;
                    primaryKeyParam = primaryKeyParam + p.FieldName + ",";
                }
            }
            primaryKeyParam = primaryKeyParam.Trim(',');
            string contents = File.ReadAllText(CreateTemplatePath("controller.txt"));
            string extraFunction = "";
            if (data.SelectByFKQuery.Count > 0)
            {
                foreach (var k in data.SelectByFKQuery)
                {
                    string colNameModel = ti.ToTitleCase(k.ColumnName);
                    string f = "";
                    f = f + "exports.getBy" + colNameModel + " = async (req, res, next) => {" + Environment.NewLine;
                    f = f + "	try {" + Environment.NewLine;
                    f = f + "		const pageNo = await getPageNo(req);" + Environment.NewLine;
                    f = f + "		const pageSize = await getPageSize(req);" + Environment.NewLine;
                    f = f + "		const offset = (pageNo - 1) * pageSize;" + Environment.NewLine;
                    f = f + "		const " + k.ColumnName + " = req.params." + k.ColumnName + ";" + Environment.NewLine;
                    f = f + "		const totalCount = await model.getBy" + colNameModel + "Count(" + k.ColumnName + ");" + Environment.NewLine;
                    f = f + "		const data = await model.getBy" + colNameModel + "(offset, pageSize, " + k.ColumnName + ");" + Environment.NewLine;
                    f = f + "		if (!_.isEmpty(data)) {" + Environment.NewLine;
                    f = f + "			const result = {" + Environment.NewLine;
                    f = f + "				pageNo: pageNo," + Environment.NewLine;
                    f = f + "				pageSize: pageSize," + Environment.NewLine;
                    f = f + "				totalCount: totalCount," + Environment.NewLine;
                    f = f + "				records: data," + Environment.NewLine;
                    f = f + "			};" + Environment.NewLine;
                    f = f + "			res.status(StatusCodes.OK).send(result);" + Environment.NewLine;
                    f = f + "		} else {" + Environment.NewLine;
                    f = f + "			res.status(StatusCodes.NOT_FOUND).send({message : \"Not found.\"});" + Environment.NewLine;
                    f = f + "		}" + Environment.NewLine;
                    f = f + "	} catch (e) {" + Environment.NewLine;
                    f = f + "		console.log(`Error`, e);" + Environment.NewLine;
                    f = f + "		next(e);" + Environment.NewLine;
                    f = f + "	}" + Environment.NewLine;
                    f = f + "};" + Environment.NewLine;
                    extraFunction = extraFunction + Environment.NewLine + f;
                }
            }
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{primaryKeyInit}", primaryKeyInit);
                contents = contents.Replace("{primaryKeyParam}", primaryKeyParam);
                contents = contents.Replace("{extraFunction}", extraFunction);
                txtFile.WriteLine(contents);
            }
        }

        public void CreateTableRoute(string tableName, FinalQueryData data)
        {
            string path = CreateDestinationPath("routes," + tableName + ".js");
            string primaryKeyURL = "";
            if (data.PrimaryKeys.Count > 0)
            {
                foreach (var p in data.PrimaryKeys)
                {
                    primaryKeyURL = primaryKeyURL + "/:" + p.FieldName;
                }
            }
            primaryKeyURL = primaryKeyURL.Trim('/');
            string contents = File.ReadAllText(CreateTemplatePath("table-route.txt"));
            string extraFunction = "";
            if (data.SelectByFKQuery.Count > 0)
            {
                foreach (var k in data.SelectByFKQuery)
                {
                    string colNameModel = ti.ToTitleCase(k.ColumnName);
                    var f = "router.get('/getby/" + colNameModel + "/:" + k.ColumnName + "', checkAuth, controller.getBy" + colNameModel + ");";
                    if (!extraFunction.Contains(f))
                        extraFunction = extraFunction + Environment.NewLine + f;
                }
            }
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{extraFunction}", extraFunction);
                contents = contents.Replace("{primaryKeyURL}", primaryKeyURL);
                txtFile.WriteLine(contents);
            }
        }

        public void CreateMainRoute()
        {
            string path = CreateDestinationPath("routes,index.js");
            string tableRouteImport = "";
            string tableRouteUse = "";
            foreach (var t in SelectedTable)
            {
                tableRouteImport = tableRouteImport + "const " + t + "Router = require('./" + t + "');" + Environment.NewLine;
                tableRouteUse = tableRouteUse + "router.use('/" + t + "', " + t + "Router);" + Environment.NewLine;
            }
            string contents = File.ReadAllText(CreateTemplatePath("routes.txt"));

            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tableRouteImport}", tableRouteImport);
                contents = contents.Replace("{tableRouteUse}", tableRouteUse);
                txtFile.WriteLine(contents);
            }
        }

        public void CreateTableDTO(string tableName, FinalQueryData data)
        {
            string path = CreateDestinationPath("dto," + tableName + ".dto.js");
            string columnList = "";
            foreach (var c in data.SelectQueryData.ColumnList)
            {
                if (c.Key != "PRI")
                {
                    var isReq = c.IsNull != null && c.IsNull.ToLower() == "no" && c.TypeName != "timestamp" ? "true" : "false";
                    columnList = columnList + Environment.NewLine + "\"" + c.Field + "\" : { required: " + isReq + ", type: \"" + Helper.GetDataTypeTypeYup(c.TypeName) + "\"},";
                }
                else if (c.Key == "PRI" && c.Extra != "auto_increment")
                {
                    var isReq = c.IsNull != null && c.IsNull.ToLower() == "no" ? "true" : "false";
                    columnList = columnList + Environment.NewLine + "\"" + c.Field + "\" : { required: " + isReq + ", type: \"" + Helper.GetDataTypeTypeYup(c.TypeName) + "\"},";
                }
            }
            string contents = File.ReadAllText(CreateTemplatePath("dto.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{columnList}", columnList);
                txtFile.WriteLine(contents);
            }
        }

        public void CreateModelSQLFile(string tableName, FinalQueryData data)
        {
            string path = CreateDestinationPath("models," + tableName + ".js");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string primaryKeyParam = "";
            string primaryKeyInsertObjectParam = "";
            string updateValueWithPrimaryKey = "";
            foreach (var p in data.PrimaryKeys)
            {
                primaryKeyParam = primaryKeyParam + p.FieldName + ",";
                primaryKeyInsertObjectParam = primaryKeyInsertObjectParam + "newObject." + p.FieldName + ",";
                updateValueWithPrimaryKey = updateValueWithPrimaryKey + Environment.NewLine +  "updateValues.push(" + p.FieldName + ");";
            }
            primaryKeyParam = primaryKeyParam.Trim(',');
            primaryKeyInsertObjectParam = primaryKeyInsertObjectParam.Trim(',');
            string selectAllQuery = "const query = `" + data.SelectAllQuery + "`;";
            string selectByIDQuery = "const query = `" + data.SelectOneQuery + "`;";
            string insertQuery = "const query = `" + data.InsertQuery + "`;";
            string updateQuery = "let query = `" + data.UpdateQuery + "`;";
            string removeQuery = "const query = `" + data.DeleteQuery + "`;";
            string countAllQuery = "const query = `" + data.SelectAllRecordCountQuery + "`;";
            string searchQuery = "const query = `" + data.SearchQuery + "`;";
            string searchCountQuery = "const query = `" + data.SearchRecordCountQuery + "`;";
            string contents = File.ReadAllText(CreateTemplatePath("models.txt"));
            string extraFunction = "";
            if (data.SelectByFKQuery.Count > 0)
            {
                foreach (var k in data.SelectByFKQuery)
                {
                    string colNameModel = ti.ToTitleCase(k.ColumnName);
                    if (!extraFunction.Contains("exports.getBy" + colNameModel + " "))
                    {
                        string f = "";
                        f = f + "exports.getBy" + colNameModel + " = async (offset, pageSize, " + k.ColumnName + ") => {";
                        f = f + Environment.NewLine + "    const query = `" + k.SelectQuery + "`;";
                        f = f + Environment.NewLine + "    return getRows(query,[" + k.ColumnName + ",offset,pageSize]);";
                        f = f + Environment.NewLine + "}" + Environment.NewLine;
                        f = f + Environment.NewLine + "exports.getBy" + colNameModel + "Count = async (" + k.ColumnName + ") => {";
                        f = f + Environment.NewLine + "    const query = `" + k.SelectCountQuery + "`;";
                        f = f + Environment.NewLine + "    const result = await getRows(query,[" + k.ColumnName + "]);";
                        f = f + Environment.NewLine + "    if (result && result[0] && result[0].TotalCount && result[0].TotalCount > 0) {";
                        f = f + Environment.NewLine + "        return result[0].TotalCount;";
                        f = f + Environment.NewLine + "    } else {";
                        f = f + Environment.NewLine + "        return 0;";
                        f = f + Environment.NewLine + "    }";
                        f = f + Environment.NewLine + "}";
                        extraFunction = extraFunction + Environment.NewLine + f;
                    }
                }
            }
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{selectAllQuery}", selectAllQuery);
                contents = contents.Replace("{selectByIDQuery}", selectByIDQuery);
                contents = contents.Replace("{insertQuery}", insertQuery);
                contents = contents.Replace("{updateQuery}", updateQuery);
                contents = contents.Replace("{primaryKeyParam}", primaryKeyParam);
                contents = contents.Replace("{removeQuery}", removeQuery);
                contents = contents.Replace("{countAllQuery}", countAllQuery);
                contents = contents.Replace("{searchQuery}", searchQuery);
                contents = contents.Replace("{searchCountQuery}", searchCountQuery);
                contents = contents.Replace("{extraFunction}", extraFunction);
                contents = contents.Replace("{searchByColumnQuery}", data.SearchQueryByColumn);
                contents = contents.Replace("{primaryKeyInsertObjectParam}", primaryKeyInsertObjectParam);
                contents = contents.Replace("{updateValueWithPrimaryKey}", updateValueWithPrimaryKey);
                


                txtFile.WriteLine(contents);
            }
        }
        public void CreateMiddleware()
        {
            string path = CreateDestinationPath("middleware,bodyValidator.js");
            string pathUpdate = CreateDestinationPath("middleware,updateValidator.js");
            string path2 = CreateDestinationPath("middleware,jwtVerify.js");
            string contents = File.ReadAllText(CreateTemplatePath("bodyValidator.txt"));
            using (var txtFile = File.AppendText(path))
            {
                txtFile.WriteLine(contents);
            }


            string contents2 = File.ReadAllText(CreateTemplatePath("jwtVerify.txt"));
            using (var txtFile = File.AppendText(path2))
            {
                txtFile.WriteLine(contents2);
            }

            string contentsUpdate = File.ReadAllText(CreateTemplatePath("updateValidator.txt"));
            using (var txtFile = File.AppendText(pathUpdate))
            {
                txtFile.WriteLine(contentsUpdate);
            }
        }

        public void CreateDatabaseFiles()
        {
            string path = CreateDestinationPath("database,connection.js");
            string path2 = CreateDestinationPath("database,query.js");
            string contents = File.ReadAllText(CreateTemplatePath("database-connection.txt"));
            using (var txtFile = File.AppendText(path))
            {
                txtFile.WriteLine(contents);
            }
            string contents2 = File.ReadAllText(CreateTemplatePath("database-query.txt"));
            using (var txtFile = File.AppendText(path2))
            {
                txtFile.WriteLine(contents2);
            }
        }

        public void CreateUtilFiles()
        {
            string path = CreateDestinationPath("utils,helper.js");
            string contents = File.ReadAllText(CreateTemplatePath("helper.txt"));
            using (var txtFile = File.AppendText(path))
            {
                txtFile.WriteLine(contents);
            }
        }
        public void CreateAppFiles()
        {
            string path = CreateDestinationPath("app.js");
            string contents = File.ReadAllText(CreateTemplatePath("app.txt"));
            using (var txtFile = File.AppendText(path))
            {
                txtFile.WriteLine(contents);
            }
        }
        public void CreatePackageJsonFiles()
        {
            string path = CreateDestinationPath("package.json");
            string contents = File.ReadAllText(CreateTemplatePath("package.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{projectName}", ProjectName);
                txtFile.WriteLine(contents);
            }
        }
        public void CreateEnvFiles()
        {
            string path = CreateDestinationPath(".env");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string contents = File.ReadAllText(CreateTemplatePath("env.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{host}", pgsqlDB.Host);
                contents = contents.Replace("{dbUser}", pgsqlDB.Username);
                contents = contents.Replace("{dbPassword}", pgsqlDB.Password);
                contents = contents.Replace("{dbName}", pgsqlDB.DBName);
                contents = contents.Replace("{dbPort}", pgsqlDB.Port.ToString());
                txtFile.WriteLine(contents);
            }
        }
        public void CreateUploadFiles()
        {
            string path = CreateDestinationPath("controllers,upload.js");
            string contents = File.ReadAllText(CreateTemplatePath("upload-controller.txt"));
            using (var txtFile = File.AppendText(path))
            {
                txtFile.WriteLine(contents);
            }
        }
        public void CreateTokenFiles()
        {
            string path = CreateDestinationPath("controllers,token.js");
            string removePasswordColumn = "";
            string password = ConfigApp.AuthTableConfig.PasswordColumnName;
            string username = ConfigApp.AuthTableConfig.UsernameColumnName;
            string authTable = ConfigApp.AuthTableConfig.AuthTableName;
            bool isAuth = !ConfigApp.AuthTableConfig.IsSkipAuth;
            string userCheck = "";
            string passwordValue = "";
            if (isAuth)
            {
                removePasswordColumn = "delete userData." + password + ";";
                userCheck = "const query = `SELECT  t.* FROM " + authTable + " t  WHERE t." + username + "=$1 AND t." + password + "=$2 LIMIT 1 OFFSET 0`;" + Environment.NewLine;
                userCheck = userCheck + "return getRows(query,[username,password]); ";
                passwordValue = "md5(req.body.password)";
            }
            else
            {
                userCheck = "if(username==\"" + username + "\" && password == \"" + password + "\"){" + Environment.NewLine;
                userCheck = userCheck + "return [{user:username}];" + Environment.NewLine;
                userCheck = userCheck + "}";
                passwordValue = "req.body.password";
            }
            string contents = File.ReadAllText(CreateTemplatePath("token-controller.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{removePasswordColumn}", removePasswordColumn);
                contents = contents.Replace("{passwordValue}", passwordValue);

                txtFile.WriteLine(contents);
            }

            string modelPath = CreateDestinationPath("models,token.js");
            string contentModel = File.ReadAllText(CreateTemplatePath("token-model.txt"));
            using (var txtFile = File.AppendText(modelPath))
            {
                contentModel = contentModel.Replace("{userCheck}", userCheck);
                txtFile.WriteLine(contentModel);
            }
        }
        public PostmanModel CreatePostmanJson(string tableName, string modelName, string type, FinalQueryData data, string fkColumnName = "")
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
                        foreach (var pk in data.SelectQueryData.PrimaryKeys)
                        {
                            pathList.Add("${" + pk.FieldName + "}");
                        }
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
                case "patch":
                    p.Name = modelName + " - Update Only Passed Columns";
                    p.Method = "PATCH";
                    if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count > 1)
                    {
                        foreach (var pk in data.SelectQueryData.PrimaryKeys)
                        {
                            pathList.Add("${" + pk.FieldName + "}");
                        }
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
                        foreach (var pk in data.SelectQueryData.PrimaryKeys)
                        {
                            pathList.Add("${" + pk.FieldName + "}");
                        }
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
                    pathList.Add("?pageNo=1&pageSize=100");
                    p.Method = "GET";
                    break;
                case "getbyid":
                    p.Name = modelName + " - GetById";
                    if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count > 1)
                    {
                        foreach (var pk in data.SelectQueryData.PrimaryKeys)
                        {
                            pathList.Add("${" + pk.FieldName + "}");
                        }
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
                case "getbyfk":
                    p.Name = modelName + " - GetByFK_" + fkColumnName;
                    pathList.Add("getby");
                    pathList.Add(fkColumnName);
                    pathList.Add("${" + fkColumnName + "}");
                    pathList.Add("?pageNo=1&pageSize=30");
                    p.Method = "GET";
                    break;
                case "search":
                    p.Name = modelName + " - Search";
                    pathList.Add("search");
                    pathList.Add("${searchKey}");
                    pathList.Add("?pageNo=1&pageSize=30");
                    p.Method = "GET";
                    break;
                case "searchByColumn":
                    p.Name = modelName + " - SearchByColumn";
                    pathList.Add("search-by-column");
                    pathList.Add("?orAnd=OR&pageno=1&pagesize=30");
                    p.Method = "POST";
                    break;
            }
            p.Path = pathList;
            foreach (var c in data.SelectQueryData.ColumnList)
            {
                if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    bodyList.Add(new PRequestBody(c.Field, c.TypeName, (c.IsNull == "NO" ? true : false), c.DefaultValue));
                }
            }
            p.Body = bodyList;

            return p;
        }
        public void CreatePostmanJsonFile()
        {
            try
            {
                Exception exc = null;
                var postmanItemString = PostmanGenerator.GeneratePostmanJson(postmanJson, out exc);
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
            catch (Exception ex)
            {
                ExceptionList.Add(ex);
                MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
            }
        }
    }
}

