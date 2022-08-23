using nkv.Automator.Models;
using nkv.Automator.Postman;
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
        public PGSQL_PHPAPI(NKVConfiguration config, string destinationFolderSeparator)
        {
            ConfigApp = config;
            ExceptionList = new List<Exception>();
            TemplateFolder = "PGSQLPHPAPITemplate";
            TemplateFolderSeparator = "\\";
            DestinationFolderSeparator = destinationFolderSeparator;
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
            CreateDBFile();
            MessageEvent?.Invoke(new NKVMessage("Database Config File Created"));
            if (!ConfigApp.AuthTableConfig.IsSkipAuth)
                CreateTokenFile("");
            CreateIndexFile();
            foreach (string table in selectedTable)
            {
                try
                {
                    MessageEvent?.Invoke(new NKVMessage("Processing for Table => "+ table));
                    var columnList = pgSQLDB.GetColumns(table);
                    string tableName = table;
                    string modelName = ti.ToTitleCase(table);
                    Directory.CreateDirectory(CreateDestinationPath(tableName));
                    CreateObjectFiles(tableName, modelName, columnList);
                }
                catch (Exception ex)
                {
                    ExceptionList.Add(ex);
                }
            }
            CreatePostmanJsonFile();
            MessageEvent?.Invoke(new NKVMessage("Postman import collection file generated"));
            CreateUploadFile();
            CompletedEvent?.Invoke(new NKVMessage("Thanks for using GetAutomator.com! Please check the generated code at : " +DestinationFolder));
            return true;
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
            return newName;
        }
        private void CreateUploadFile()
        {
            try
            {
                string path = CreateDestinationPath("files,readfile.php");
                string path2 = CreateDestinationPath("files,uploadfile.php");
                string path3 = CreateDestinationPath("files,uploadimage.php");
                string path4 = CreateDestinationPath("notification,send.php");

                string contents = File.ReadAllText(CreateTemplatePath("files,readfile.txt"));
                using (var txtFile = File.AppendText(path))
                {
                    txtFile.WriteLine(contents);
                }
                string content2 = File.ReadAllText(CreateTemplatePath("files,uploadfile.txt"));
                using (var txtFile = File.AppendText(path2))
                {
                    txtFile.WriteLine(content2);
                }

                string content3 = File.ReadAllText(CreateTemplatePath("files,uploadimage.txt"));
                using (var txtFile = File.AppendText(path3))
                {
                    txtFile.WriteLine(content3);
                }

                string content4 = File.ReadAllText(CreateTemplatePath("notification,send.txt"));
                using (var txtFile = File.AppendText(path4))
                {
                    txtFile.WriteLine(content4);
                }
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex+"",false));
                ExceptionList.Add(ex);
            }
        }
        private void CreateTokenFile(string primaryKey)
        {
            try
            {
                string path = CreateDestinationPath("token,token.php");
                string path2 = CreateDestinationPath("token,generate.php");
                string path3 = CreateDestinationPath("token,validatetoken.php");
                string path4 = CreateDestinationPath("jwt,BeforeValidException.php");
                string path5 = CreateDestinationPath("jwt,ExpiredException.php");
                string path6 = CreateDestinationPath("jwt,JWT.php");
                string path7 = CreateDestinationPath("jwt,SignatureInvalidException.php");

                string contents = File.ReadAllText(CreateTemplatePath("token,token.txt"));
                using (var txtFile = File.AppendText(path))
                {
                    txtFile.WriteLine(contents);
                }
                if (ConfigApp.AuthTableConfig.IsSkipAuth)
                {
                    string contents2 = File.ReadAllText(CreateTemplatePath("token,generate-simple.txt"));
                    using (var txtFile = File.AppendText(path2))
                    {
                        txtFile.WriteLine(contents2);
                    }

                }
                else
                {
                    string contents2 = File.ReadAllText(CreateTemplatePath("token,generate.txt"));
                    using (var txtFile = File.AppendText(path2))
                    {
                        contents2 = contents2.Replace("{userTable}", ConfigApp.AuthTableConfig.AuthTableName);
                        contents2 = contents2.Replace("{moduleName}", ti.ToTitleCase(ConfigApp.AuthTableConfig.AuthTableName));
                        contents2 = contents2.Replace("{userColumn}", ConfigApp.AuthTableConfig.UsernameColumnName);
                        contents2 = contents2.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                        contents2 = contents2.Replace("{primaryKey}", primaryKey);
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
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }
        }
        private void CreateDBFile()
        {
            try
            {
                string path = CreateDestinationPath("config,database.php");
                string contents = File.ReadAllText(CreateTemplatePath("config,database.txt"));
                using (var txtFile = File.AppendText(path))
                {
                    contents = contents.Replace("{hostName}", pgSQLDB.Host);
                    contents = contents.Replace("{userName}", pgSQLDB.Username);
                    contents = contents.Replace("{password}", pgSQLDB.Password);
                    contents = contents.Replace("{dbName}", pgSQLDB.DBName);
                    contents = contents.Replace("{port}", pgSQLDB.Port.ToString());
                    txtFile.WriteLine(contents);
                }
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }
        }

        private void CreateObjectFiles(string tableName, string moduleName, List<ColumnModel> columnList)
        {
            try
            {
                string loginValidation = "function login_validation(){";
                loginValidation = loginValidation + Environment.NewLine + "$query = {selectLoginQuery}";
                loginValidation = loginValidation + Environment.NewLine + "$paramArray = array();";
                loginValidation = loginValidation + Environment.NewLine + "array_push($paramArray,$this->{userColumn});";
                loginValidation = loginValidation + Environment.NewLine + "array_push($paramArray,$this->{passwordColumn});";
                loginValidation = loginValidation + Environment.NewLine + "$stmt = pg_query_params($this->conn,$query,$paramArray);";
                loginValidation = loginValidation + Environment.NewLine + "$row = pg_fetch_assoc($stmt);";
                loginValidation = loginValidation + "if($row) {" + Environment.NewLine;
                loginValidation = loginValidation + "{selectOneSetValueToObject}" + Environment.NewLine;
                loginValidation = loginValidation + Environment.NewLine + "}";
                loginValidation = loginValidation + Environment.NewLine + "}";
                string objectProperties = "";
                //string tableVariable = "\". $this->table_name .\"";
                string path = CreateDestinationPath("objects," + tableName + ".php");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                foreach (var c in columnList)
                {
                    objectProperties = objectProperties + Environment.NewLine + "public $" + c.Field + ";";
                    if (c.FKDetails != null)
                    {
                        var refColumns = pgSQLDB.GetColumns(c.FKDetails.REFERENCED_TABLE_NAME);
                        if (refColumns != null && refColumns.Count > 0)
                        {
                            var refColumnName = refColumns.Where(i => i.IsNull == "NO" && i.Key != "PRI" && i.TypeName.Contains("varchar")).FirstOrDefault();
                            if (refColumnName != null)
                            {
                                objectProperties = objectProperties + Environment.NewLine + "public $" + refColumnName.Field + ";";
                            }
                        }
                    }
                }
                string pKey = "";
                string selectOneQuery = "", searchQueryByColumn = "", searchCountQueryByColumn = "", selectOneBindValue = "", selectOneSetValues = "", deleteBind = "", deleteQuery = "", deleteSenitize = "", selectLoginQuery = "";
                var selectQuery = CreateSelectQuery(tableName, columnList, out selectOneQuery, out selectOneBindValue, out selectOneSetValues, out deleteQuery, out deleteSenitize, out deleteBind, out pKey, out selectLoginQuery, out searchCountQueryByColumn, out searchQueryByColumn);
                string sanitize = string.Empty;
                string bindValue = string.Empty;
                string loginFunction = "";
                if (!ConfigApp.AuthTableConfig.IsSkipAuth && tableName == ConfigApp.AuthTableConfig.AuthTableName)
                {
                    loginFunction = loginValidation;
                    selectLoginQuery = selectLoginQuery.Replace("{userColumn}", ConfigApp.AuthTableConfig.UsernameColumnName);
                    selectLoginQuery = selectLoginQuery.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                    loginFunction = loginFunction.Replace("{userColumn}", ConfigApp.AuthTableConfig.UsernameColumnName);
                    loginFunction = loginFunction.Replace("{passwordColumn}", ConfigApp.AuthTableConfig.PasswordColumnName);
                    loginFunction = loginFunction.Replace("{selectLoginQuery}", selectLoginQuery);
                    loginFunction = loginFunction.Replace("{tableName}", tableName);
                    loginFunction = loginFunction.Replace("{primaryKey}", pKey);
                    loginFunction = loginFunction.Replace("{objectProperties}", objectProperties);
                    loginFunction = loginFunction.Replace("{moduleName}", moduleName);
                    loginFunction = loginFunction.Replace("{selectOneSetValueToObject}", selectOneSetValues);
                    CreateTokenFile(pKey);
                }
                else
                {
                    loginFunction = "";
                }
                //bindValue = bindValue + "array_push($paramArray,$this->{primaryKey});";
                string updateQuery = string.Empty;
                var insertQuery = CreateInsertQuery(tableName, columnList, out sanitize, out bindValue, out updateQuery);
                updateQuery = updateQuery.Replace("{primaryKey}", pKey);
                string updateSanitize = sanitize;
                updateSanitize = updateSanitize + Environment.NewLine + "$this->" + pKey + "=htmlspecialchars(strip_tags($this->" + pKey + "));";
                string updateBindValue = bindValue;
                updateBindValue = updateBindValue + Environment.NewLine + "array_push($paramArray,$this->{primaryKey});";
                updateBindValue = updateBindValue.Replace("{primaryKey}", pKey);
                //"$stmt->bindParam(\":" + pKey + "\", $this->" + pKey + ");";
                #region CreateObjectFile
                string contents = File.ReadAllText(CreateTemplatePath("objects,objects.txt"));
                using (var txtFile = File.AppendText(path))
                {

                    contents = contents.Replace("{tableName}", tableName);
                    contents = contents.Replace("{objectProperties}", objectProperties);
                    contents = contents.Replace("{moduleName}", moduleName);

                    contents = contents.Replace("{loginFunction}", loginFunction);

                    contents = contents.Replace("{selectQuery}", selectQuery);
                    contents = contents.Replace("{selectOneQuery}", selectOneQuery);
                    contents = contents.Replace("{selectOneBindValue}", selectOneBindValue);
                    contents = contents.Replace("{selectOneSetValueToObject}", selectOneSetValues);

                    contents = contents.Replace("{insertQuery}", insertQuery);
                    contents = contents.Replace("{insertSanitize}", sanitize);
                    contents = contents.Replace("{insertBindValue}", bindValue);

                    contents = contents.Replace("{updateQuery}", updateQuery);
                    contents = contents.Replace("{updateSanitize}", updateSanitize);
                    contents = contents.Replace("{updateBindValue}", updateBindValue);

                    contents = contents.Replace("{deleteQuery}", deleteQuery);
                    contents = contents.Replace("{deleteBind}", deleteBind);
                    contents = contents.Replace("{deleteSenitize}", deleteSenitize);

                    contents = contents.Replace("{searchQueryByColumn}", searchQueryByColumn);
                    contents = contents.Replace("{searchCountQueryByColumn}", searchCountQueryByColumn);
                    contents = contents.Replace("{primaryKey}", pKey);
                    txtFile.WriteLine(contents);
                }

                CreateModalCreateFile(tableName, moduleName, columnList);
                CreateModalDeleteFile(tableName, pKey, moduleName);
                CreateModalReadFile(tableName, moduleName, pKey, columnList);
                CreateModalUpdateFile(tableName, moduleName, pKey, columnList);
                CreateModalUpdatePatchFile(tableName, moduleName, pKey, columnList);
                #endregion
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }

        }
        private string CreateSelectQuery(string tableName, List<ColumnModel> columnList, out string selectOneQuery, out string selectOneBindValue, out string selectOneSetValues, out string deleteQuery, out string deleteSanitize, out string deleteBind, out string pKey, out string selectLoginQuery, out string searchCountQueryByColumn, out string searchQueryByColumn)
        {

            try
            {
                var pathList = new List<string>();
                var bodyList = new List<PRequestBody>();
                var pathList2 = new List<string>();
                var bodyList2 = new List<PRequestBody>();
                var pathList3 = new List<string>();
                var bodyList3 = new List<PRequestBody>();
                var pathList4 = new List<string>();
                var bodyList4 = new List<PRequestBody>();
                var pathList5 = new List<string>();
                var bodyList5 = new List<PRequestBody>();
                var pathList6 = new List<string>();
                var bodyList6 = new List<PRequestBody>();
                var pathList7 = new List<string>();
                var bodyList7 = new List<PRequestBody>();
                pathList.Add(tableName);
                pathList.Add("read.php?pageno=1&pagesize=30");

                PostmanModel getAll = new PostmanModel();
                getAll.TableName = tableName;
                getAll.Name = ti.ToTitleCase(tableName) + " - GETALL";
                getAll.Method = "GET";
                getAll.Path = pathList;

                pathList2.Add(tableName);
                pathList2.Add("read_one.php?id=1");
                PostmanModel getById = new PostmanModel();
                getById.TableName = tableName;
                getById.Name = ti.ToTitleCase(tableName) + " - GETByID";
                getById.Method = "GET";
                getById.Path = pathList2;

                pathList3.Add(tableName);
                pathList3.Add("delete.php");
                PostmanModel deletePhp = new PostmanModel();
                deletePhp.TableName = tableName;
                deletePhp.Name = ti.ToTitleCase(tableName) + " - Delete";
                deletePhp.Method = "POST";
                deletePhp.Path = pathList3;

                pathList4.Add(tableName);
                pathList4.Add("create.php");
                PostmanModel addNew = new PostmanModel();
                addNew.TableName = tableName;
                addNew.Name = ti.ToTitleCase(tableName) + " - Add New";
                addNew.Method = "POST";
                addNew.Path = pathList4;

                pathList5.Add(tableName);
                pathList5.Add("update.php");
                PostmanModel updateData = new PostmanModel();
                updateData.TableName = tableName;
                updateData.Name = ti.ToTitleCase(tableName) + " - Update";
                updateData.Method = "POST";
                updateData.Path = pathList5;

                pathList6.Add(tableName);
                pathList6.Add("search_by_column.php?orAnd=OR&pageno=1&pagesize=30");
                PostmanModel searchByColumn = new PostmanModel();
                searchByColumn.TableName = tableName;
                searchByColumn.Name = ti.ToTitleCase(tableName) + " - SearchByColumn";
                searchByColumn.Method = "POST";
                searchByColumn.Path = pathList6;

                pathList7.Add(tableName);
                pathList7.Add("update_patch.php");
                PostmanModel p7 = new PostmanModel();
                p7.TableName = tableName;
                p7.Name = ti.ToTitleCase(tableName) + " - Update Patch";
                p7.Method = "POST";
                p7.Path = pathList7;


                pKey = "";
                List<string> searchColumList = new List<string>();
                string searchCondition = string.Empty;
                selectOneSetValues = "";
                searchCountQueryByColumn = string.Empty;
                searchQueryByColumn = string.Empty;
                string tableVariable = "\". $this->table_name .\"";
                selectOneQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE t.{primaryKey} = $1 offset 0 limit 1";
                searchQueryByColumn = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE \".$where.\" offset \".$offset.\" limit \". $this->no_of_records_per_page.\"";
                searchCountQueryByColumn = "SELECT count(1) as total FROM " + tableVariable + " t {joinQuery} WHERE \".$where.\"";

                selectLoginQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE t.{userColumn} =$1 AND t.{passwordColumn}=$2 offset 0 limit 1";
                selectOneBindValue = "$paramArray= array($this->{primaryKey});";
                var alpha = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "u", "v", "w", "x", "y", "z" };
                var selectQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery}  offset \".$offset.\" limit \". $this->no_of_records_per_page.\"";
                var joinString = string.Empty;
                var fkColumnsString = string.Empty;
                deleteQuery = "$query = \"DELETE FROM \" . $this->table_name . \" WHERE {primaryKey} = $1 \";";
                deleteSanitize = "$this->{primaryKey}=htmlspecialchars(strip_tags($this->{primaryKey}));";
                deleteBind = "$paramArray= array($this->{primaryKey});";
                var keyValue = "";

                var random = new Random();
                List<string> alphaList = alpha;
                bool isRepeat = false;
                foreach (var c in columnList)
                {
                    searchColumList.Add("t." + c.Field);
                    selectOneSetValues = selectOneSetValues + Environment.NewLine + "$this->" + c.Field + " = $row['" + c.Field + "'];";
                    if (c.FKDetails != null && !joinString.Contains(c.FKDetails.REFERENCED_COLUMN_NAME))
                    {
                        var asChar = string.Empty;
                        if (alphaList.Count == 0)
                        {
                            alphaList = alpha;
                            isRepeat = true;
                        }
                        int index = random.Next(alphaList.Count);
                        asChar = alphaList[index];
                        alphaList.Remove(asChar);
                        if (isRepeat)
                        {
                            asChar = asChar + asChar;
                        }
                        var refColumns = pgSQLDB.GetColumns(c.FKDetails.REFERENCED_TABLE_NAME);
                        if (refColumns != null && refColumns.Count > 0)
                        {
                            var refColumnName = refColumns.Where(i => i.IsNull == "NO" && i.Key != "PRI" && i.TypeName.Contains("varchar")).FirstOrDefault();
                            if (refColumnName != null)
                            {
                                joinString = joinString + " join " + c.FKDetails.REFERENCED_TABLE_NAME + " " + asChar + " on t." + c.Field + " = " + asChar + "." + c.FKDetails.REFERENCED_COLUMN_NAME + " ";
                                fkColumnsString = fkColumnsString + " " + asChar + "." + refColumnName.Field + ",";
                                searchColumList.Add(asChar + "." + refColumnName.Field);
                                selectOneSetValues = selectOneSetValues + Environment.NewLine + "$this->" + refColumnName.Field + " = $row['" + refColumnName.Field + "'];";
                            }
                        }
                    }
                    if (c.Extra == "auto_increment")
                    {
                        keyValue = c.Field;
                    }
                    if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                    {
                        bodyList.Add(new PRequestBody(c.Field, c.TypeName, c.IsNull == "NO" ? true : false, c.DefaultValue));
                        bodyList4.Add(new PRequestBody(c.Field, c.TypeName, c.IsNull == "NO" ? true : false, c.DefaultValue));
                        bodyList5.Add(new PRequestBody(c.Field, c.TypeName, c.IsNull == "NO" ? true : false, c.DefaultValue));
                        bodyList6.Add(new PRequestBody(c.Field, c.TypeName, c.IsNull == "NO" ? true : false, c.DefaultValue));
                    }
                }
                if (string.IsNullOrEmpty(keyValue))
                {
                    var priKey = columnList.Where(i => i.Key == "PRI").FirstOrDefault();
                    if (priKey != null)
                    {
                        keyValue = priKey.Field;
                    }
                    else
                    {
                        keyValue = "ERROR_NOPRIMARYKEYFOUND";
                    }
                }

                pKey = keyValue;
                bodyList3.Add(new PRequestBody(pKey, "int", false, "1"));
                bodyList5.Add(new PRequestBody(pKey, "int", false, "1"));
                foreach (var sField in searchColumList)
                {
                    if (string.IsNullOrEmpty(searchCondition))
                    {
                        searchCondition = sField + " LIKE ?";
                    }
                    else
                    {
                        searchCondition = searchCondition + " OR " + sField + " LIKE ? ";
                    }
                }

                selectQuery = selectQuery.Replace("{tableName}", tableName);
                selectQuery = selectQuery.Replace("{joinQuery}", joinString);
                selectQuery = selectQuery.Replace("{fkColumns}", fkColumnsString);

                selectOneQuery = selectOneQuery.Replace("{tableName}", tableName);
                selectOneQuery = selectOneQuery.Replace("{joinQuery}", joinString);
                selectOneQuery = selectOneQuery.Replace("{fkColumns}", fkColumnsString);
                selectOneQuery = selectOneQuery.Replace("{primaryKey}", pKey);
                selectOneQuery = "$query = \"" + selectOneQuery + "\";";

                selectLoginQuery = selectLoginQuery.Replace("{tableName}", tableName);
                selectLoginQuery = selectLoginQuery.Replace("{joinQuery}", joinString);
                selectLoginQuery = selectLoginQuery.Replace("{fkColumns}", fkColumnsString);
                selectLoginQuery = selectLoginQuery.Replace("{primaryKey}", pKey);
                selectLoginQuery = "$query = \"" + selectLoginQuery + "\";";

                searchQueryByColumn = searchQueryByColumn.Replace("{tableName}", tableName);
                searchQueryByColumn = searchQueryByColumn.Replace("{joinQuery}", joinString);
                searchQueryByColumn = searchQueryByColumn.Replace("{fkColumns}", fkColumnsString);
                searchQueryByColumn = searchQueryByColumn.Replace("{searchCondition}", searchCondition);
                searchQueryByColumn = "$query = \"" + searchQueryByColumn + "\";";

                searchCountQueryByColumn = searchCountQueryByColumn.Replace("{tableName}", tableName);
                searchCountQueryByColumn = searchCountQueryByColumn.Replace("{joinQuery}", joinString);
                searchCountQueryByColumn = searchCountQueryByColumn.Replace("{fkColumns}", fkColumnsString);
                searchCountQueryByColumn = searchCountQueryByColumn.Replace("{searchCondition}", searchCondition);
                searchCountQueryByColumn = "$query = \"" + searchCountQueryByColumn + "\";";


                selectOneBindValue = selectOneBindValue.Replace("{primaryKey}", pKey);

                deleteQuery = deleteQuery.Replace("{primaryKey}", pKey);
                deleteSanitize = deleteSanitize.Replace("{primaryKey}", pKey);
                deleteBind = deleteBind.Replace("{primaryKey}", pKey);
                getAll.Body = bodyList;
                getById.Body = bodyList2;
                deletePhp.Body = bodyList3;
                addNew.Body = bodyList4;
                updateData.Body = bodyList5;
                searchByColumn.Body = bodyList6;
                p7.Body = bodyList5;
                postmanJson.Add(getAll);
                postmanJson.Add(getById);
                postmanJson.Add(deletePhp);
                postmanJson.Add(addNew);
                postmanJson.Add(updateData);
                postmanJson.Add(searchByColumn);
                postmanJson.Add(p7);
                return "$query = \"" + selectQuery + "\";";
            }
            catch (Exception ex)
            {
                selectOneQuery = "";
                searchCountQueryByColumn = "";
                searchQueryByColumn = "";
                selectOneBindValue = "";
                selectOneSetValues = "";
                deleteQuery = "";
                deleteSanitize = "";
                deleteBind = "";
                selectLoginQuery = "";
                pKey = "";
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
                return "";
            }
        }
        private string CreateInsertQuery(string tableName, List<ColumnModel> columnList, out string sanitize, out string bindValue, out string updateQuery)
        {
            sanitize = string.Empty;
            bindValue = "$paramArray=array();" + Environment.NewLine;
            try
            {
                var alpha = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "u", "v", "w", "x", "y", "z" };

                string insertQuery = "INSERT INTO \".$this->table_name.\" ({columnName}) VALUES ({fieldParamCount});";
                updateQuery = "UPDATE \".$this->table_name.\" SET {fieldName} WHERE {primaryKey} = ${paramCount}";

                string columnName = string.Empty;
                string fieldParamCount = string.Empty;
                string fieldName = string.Empty;
                int paramCount = 1;
                foreach (var c in columnList)
                {
                    if (c.Extra == "auto_increment")
                    {

                    }
                    else if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                    {
                        columnName = columnName + "," + c.Field;
                        fieldParamCount = fieldParamCount + "," + "$" + paramCount;
                        fieldName = fieldName + "," + c.Field + "=$" + paramCount;
                        sanitize = sanitize + Environment.NewLine + "$this->" + c.Field + "=htmlspecialchars(strip_tags($this->" + c.Field + "));";
                        bindValue = bindValue + Environment.NewLine + "array_push($paramArray,$this->" + c.Field + ");";
                        paramCount++;
                    }
                }
                fieldName = fieldName.TrimStart(',').TrimEnd(',');
                columnName = columnName.TrimStart(',').TrimEnd(',');
                fieldParamCount = fieldParamCount.TrimStart('$').TrimEnd('$');
                fieldParamCount = fieldParamCount.TrimStart(',').TrimEnd(',');
                insertQuery = insertQuery.Replace("{columnName}", columnName);
                insertQuery = insertQuery.Replace("{fieldParamCount}", fieldParamCount);
                insertQuery = "$query =\"" + insertQuery + "\";";
                updateQuery = updateQuery.Replace("{fieldName}", fieldName);
                updateQuery = updateQuery.Replace("{paramCount}", paramCount.ToString());
                updateQuery = "$query =\"" + updateQuery + "\";";
                return insertQuery;
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                updateQuery = "";
                ExceptionList.Add(ex);
                return "";
            }
        }
        private void CreateModalCreateFile(string tableName, string moduleName, List<ColumnModel> columnList)
        {
            try
            {
                string path = CreateDestinationPath(tableName + ",create.php");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                string requiredFields = "";
                string setPropertyValue = "";

                foreach (var c in columnList)
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
                        if (c.IsNull != null && c.IsNull.ToLower() == "no")
                        {
                            if (string.IsNullOrEmpty(requiredFields))
                            {
                                requiredFields = "!empty($data->" + c.Field + ")";
                            }
                            else
                            {
                                requiredFields = requiredFields + Environment.NewLine + "&&" + "!empty($data->" + c.Field + ")";
                            }

                        }
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                    }
                }

                requiredFields = requiredFields.Trim('&').Trim('&');
                string contents = File.ReadAllText(CreateTemplatePath("modal,create.txt"));
                using (var txtFile = File.AppendText(path))
                {
                    if (string.IsNullOrEmpty(requiredFields))
                    {
                        requiredFields = "true";
                    }
                    contents = contents.Replace("{tableName}", tableName);
                    contents = contents.Replace("{moduleName}", moduleName);
                    contents = contents.Replace("{requiredFields}", requiredFields);
                    contents = contents.Replace("{setPropertyValue}", setPropertyValue);
                    txtFile.WriteLine(contents);
                }
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }
        }
        private void CreateModalReadFile(string tableName, string moduleName, string primaryKey, List<ColumnModel> columnList)
        {
            try
            {
                string path = CreateDestinationPath(tableName + ",read.php");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                string path_one = CreateDestinationPath(tableName + ",read_one.php");
                if (File.Exists(path_one))
                {
                    File.Delete(path_one);
                }
                string path_search = CreateDestinationPath(tableName + ",search_by_column.php");
                if (File.Exists(path_search))
                {
                    File.Delete(path_search);
                }
                if (string.IsNullOrEmpty(primaryKey))
                {
                    primaryKey = "ERROR_NO_PRIMARY_KEY_FOUND";
                }
                string setPropertyValue = "";
                string setPropertyValue1 = "";

                foreach (var c in columnList)
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

                setPropertyValue = setPropertyValue.Trim(',');
                setPropertyValue1 = setPropertyValue1.Trim(',');
                string contents = File.ReadAllText(CreateTemplatePath("modal,read.txt"));
                using (var txtFile = File.AppendText(path))
                {

                    contents = contents.Replace("{tableName}", tableName);
                    contents = contents.Replace("{moduleName}", moduleName);
                    contents = contents.Replace("{setPropertyValue}", setPropertyValue);
                    txtFile.WriteLine(contents);
                }

                string content_search = File.ReadAllText(CreateTemplatePath("modal,search_by_column.txt"));
                using (var txtFile = File.AppendText(path_search))
                {
                    content_search = content_search.Replace("{tableName}", tableName);
                    content_search = content_search.Replace("{moduleName}", moduleName);
                    content_search = content_search.Replace("{setPropertyValue}", setPropertyValue);
                    txtFile.WriteLine(content_search);
                }

                string contents_one = File.ReadAllText(CreateTemplatePath("modal,read_one.txt"));
                using (var txtFile = File.AppendText(path_one))
                {
                    contents_one = contents_one.Replace("{tableName}", tableName);
                    contents_one = contents_one.Replace("{moduleName}", moduleName);
                    contents_one = contents_one.Replace("{setPropertyValue}", setPropertyValue1);
                    contents_one = contents_one.Replace("{primaryKey}", primaryKey);

                    txtFile.WriteLine(contents_one);
                }
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }
        }
        private void CreateModalDeleteFile(string tableName, string primaryKey, string moduleName)
        {
            string path = CreateDestinationPath(tableName + ",delete.php");
            if (string.IsNullOrEmpty(primaryKey))
            {
                primaryKey = "ERROR_NO_PRIMARY_KEY_FOUND";
            }

            string contents = File.ReadAllText(CreateTemplatePath("modal,delete.txt"));
            using (var txtFile = File.AppendText(path))
            {

                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{moduleName}", moduleName);
                contents = contents.Replace("{primaryKey}", primaryKey);
                txtFile.WriteLine(contents);
            }
        }
        private void CreateModalUpdateFile(string tableName, string moduleName, string primaryKey, List<ColumnModel> columnList)
        {
            try
            {
                string path = CreateDestinationPath(tableName + ",update.php");
                if (string.IsNullOrEmpty(primaryKey))
                {
                    primaryKey = "ERROR_NO_PRIMARY_KEY_FOUND";
                }
                string requiredFields = "";
                string setPropertyValue = "";

                foreach (var c in columnList)
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
                                requiredFields = "!empty($data->" + c.Field + ")";
                            }
                            else
                            {
                                requiredFields = requiredFields + Environment.NewLine + "&&" + "!empty($data->" + c.Field + ")";
                            }

                        }
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                    }
                }

                requiredFields = requiredFields.Trim('&').Trim('&');
                string contents = File.ReadAllText(CreateTemplatePath("modal,update.txt"));
                using (var txtFile = File.AppendText(path))
                {
                    if (string.IsNullOrEmpty(requiredFields))
                    {
                        requiredFields = "true";
                    }

                    contents = contents.Replace("{tableName}", tableName);
                    contents = contents.Replace("{moduleName}", moduleName);
                    contents = contents.Replace("{requiredFields}", requiredFields);
                    contents = contents.Replace("{setPropertyValue}", setPropertyValue);
                    contents = contents.Replace("{primaryKey}", primaryKey);
                    txtFile.WriteLine(contents);
                }
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }
        }
        private void CreateModalUpdatePatchFile(string tableName, string moduleName, string primaryKey, List<ColumnModel> columnList)
        {
            try
            {
                string path = CreateDestinationPath(tableName + ",update_patch.php");
                if (string.IsNullOrEmpty(primaryKey))
                {
                    primaryKey = "ERROR_NO_PRIMARY_KEY_FOUND";
                }
                string requiredFields = "";
                string setPropertyValue = "";

                foreach (var c in columnList)
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
                                requiredFields = "!empty($data->" + c.Field + ")";
                            }
                            else
                            {
                                requiredFields = requiredFields + Environment.NewLine + "&&" + "!empty($data->" + c.Field + ")";
                            }

                        }
                        setPropertyValue = setPropertyValue + Environment.NewLine + "$" + tableName + "->" + c.Field + " = $data->" + c.Field + ";";
                    }
                }

                requiredFields = requiredFields.Trim('&').Trim('&');
                string contents = File.ReadAllText(CreateTemplatePath("modal,update_patch.txt"));
                using (var txtFile = File.AppendText(path))
                {
                    if (string.IsNullOrEmpty(requiredFields))
                    {
                        requiredFields = "true";
                    }

                    contents = contents.Replace("{tableName}", tableName);
                    contents = contents.Replace("{moduleName}", moduleName);
                    contents = contents.Replace("{requiredFields}", requiredFields);
                    contents = contents.Replace("{setPropertyValue}", setPropertyValue);
                    contents = contents.Replace("{primaryKey}", primaryKey);
                    txtFile.WriteLine(contents);
                }
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }
        }
        private void CreateIndexFile()
        {
            try
            {
                string path = CreateDestinationPath("index.php");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                string completeTableString = "";
                string moduleName = "";
                foreach (var table in SelectedTable)
                {
                    moduleName = ti.ToTitleCase(table);
                    string tableStrig = "<table class='table table-striped table-condensed'> <thead> <tr><th colspan='3'><div class='p-3 mb-2 bg-primary text-white'><h3><b>" + moduleName + "</b></h3></div><tr></th>";
                    tableStrig = tableStrig + "</thead><tbody>";
                    tableStrig = tableStrig + "<tr><td>Read</td><td>GET</td><td>/" + table + "/read.php?pageno=1&pagesize=30</td></tr>";
                    tableStrig = tableStrig + "<tr><td>Read One</td><td>GET</td><td>/" + table + "/read_one.php?id=1</td></tr>";
                    tableStrig = tableStrig + "<tr><td>Create</td><td>POST</td><td>/" + table + "/create.php</td></tr>";
                    tableStrig = tableStrig + "<tr><td>Update</td><td>POST</td><td>/" + table + "/update.php</td></tr>";
                    tableStrig = tableStrig + "<tr><td>Delete</td><td>POST</td><td>/" + table + "/delete.php</td></tr>";
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
            }
            catch (Exception ex)
            {
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }
        }
        private void CreatePostmanJsonFile()
        {
            try
            {
                var list = postmanJson;
                Exception ex = null;
                var postmanItemString = PostmanGenerator.GeneratePostmanJson(list, out ex);
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
                MessageEvent?.Invoke(new NKVMessage(ex + "", false));
                ExceptionList.Add(ex);
            }
        }


    }
}
