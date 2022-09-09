using MySql.Data.MySqlClient;
using nkv.Automator.Generator.Models;
using nkv.Automator.Models;
using nkv.Automator.Utility;
using System.Globalization;

namespace nkv.Automator.MySQL
{
    public class MySQLDBHelper
    {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        public string ConnectionString { get; set; }
        public MySqlConnection dbCon { get; set; } = null!;
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DBName { get; set; }
        Dictionary<string, List<ColumnModel>> dicColumn = new Dictionary<string, List<ColumnModel>>();
        Dictionary<string, List<PrimaryKeyClass>> dicPrimaryKey = new Dictionary<string, List<PrimaryKeyClass>>();
        public MySQLDBHelper(string host, string port, string username, string password, string dbName)
        {
            Host = host;
            Port = int.Parse(port);
            Username = username;
            Password = password;
            DBName = dbName;
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = host;
            builder.Port = Convert.ToUInt32(port);
            builder.UserID = username;
            builder.Password = password;
            builder.Database = dbName;
            builder.SslMode = MySqlSslMode.Preferred;
            builder.CharacterSet = "utf8";
            ConnectionString = builder.ToString();
        }
        public bool Connect()
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            dbCon = connection;
            return true;
        }
        private bool IsConnect()
        {
            if (dbCon == null) Connect();
            if (dbCon != null && dbCon.State == System.Data.ConnectionState.Closed) dbCon.Open();
            return true;
        }
        public List<string> GetListOfTable()
        {
            List<string> tableList = new List<string>();
            if (dbCon != null)
            {
                if (IsConnect())
                {
                    string query = "select table_name FROM information_schema.tables WHERE TABLE_TYPE NOT LIKE 'VIEW' AND table_schema = '" + DBName + "'; ";
                    var cmd = new MySqlCommand(query, dbCon);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        tableList.Add(tableName);
                    }
                    dbCon.Close();
                }
            }
            return tableList;
        }
        public List<string> GetListOfView()
        {
            List<string> tableList = new List<string>();
            if (dbCon != null)
            {
                if (IsConnect())
                {
                    string query = "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_TYPE LIKE 'VIEW' AND TABLE_SCHEMA = '" + DBName + "'; ";
                    var cmd = new MySqlCommand(query, dbCon);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        tableList.Add(tableName);
                    }
                    dbCon.Close();
                }
            }
            return tableList;
        }
        public List<ColumnModel> GetTableColumns(string tableName)
        {
            List<ColumnModel> columnList = new List<ColumnModel>();
            if (dbCon != null)
            {
                if (IsConnect() && !string.IsNullOrEmpty(tableName))
                {
                    string query = "SHOW COLUMNS FROM " + DBName + "." + tableName;
                    var cmd = new MySqlCommand(query, dbCon);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var x = new ColumnModel()
                        {
                            Field = reader.IsDBNull(0) ? null : reader.GetString(0),
                            TypeName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            IsNull = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Key = reader.IsDBNull(3) ? null : reader.GetString(3),
                            DefaultValue = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Extra = reader.IsDBNull(5) ? null : reader.GetString(5),
                        };
                        columnList.Add(x);
                    }
                    reader.Close();
                    string columnFK = "select COLUMN_NAME, CONSTRAINT_NAME, REFERENCED_COLUMN_NAME, REFERENCED_TABLE_NAME from information_schema.KEY_COLUMN_USAGE where TABLE_NAME = '" + tableName + "' AND TABLE_SCHEMA='" + DBName + "'; ";
                    var cmd2 = new MySqlCommand(columnFK, dbCon);
                    var reader2 = cmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        var x = new FKDetails()
                        {
                            COLUMN_NAME = reader2.IsDBNull(0) ? null : reader2.GetString(0),
                            CONSTRAINT_NAME = reader2.IsDBNull(1) ? null : reader2.GetString(1),
                            REFERENCED_COLUMN_NAME = reader2.IsDBNull(2) ? null : reader2.GetString(2),
                            REFERENCED_TABLE_NAME = reader2.IsDBNull(3) ? null : reader2.GetString(3),
                        };
                        if (!string.IsNullOrEmpty(x.REFERENCED_TABLE_NAME))
                        {
                            if (columnList.Where(i => i.Field.Equals(x.COLUMN_NAME, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() != null)
                            {
                                columnList.Where(i => i.Field.Equals(x.COLUMN_NAME, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().FKDetails = x;
                            }
                        }
                    }
                    reader2.Close();
                    dbCon.Close();
                }
            }
            return columnList;
        }

        public List<PrimaryKeyClass> GetPrimaryKey(string tableName)
        {
            List<PrimaryKeyClass> primaryKeys = new List<PrimaryKeyClass>();
            List<ColumnModel> columnList = null;
            if (dicColumn.ContainsKey(tableName))
            {
                columnList = dicColumn[tableName];
            }
            else
            {
                columnList = GetTableColumns(tableName);
                dicColumn.Add(tableName, columnList);
            }
            var priKeyList = columnList.Where(i => i.Key == "PRI").ToList();
            if (priKeyList != null)
            {
                foreach (var priKey in priKeyList)
                {
                    primaryKeys.Add(new PrimaryKeyClass()
                    {
                        DataType = priKey.TypeName,
                        FieldName = priKey.Field
                    });
                }
            }

            if (primaryKeys.Count() == 0)
            {
                foreach (var c in columnList)
                {
                    if (c.Extra == "auto_increment")
                    {
                        primaryKeys.Add(new PrimaryKeyClass()
                        {
                            DataType = c.TypeName,
                            FieldName = c.Field
                        });
                    }
                }
            }
            if (primaryKeys.Count() == 0 && columnList.Count > 0)
            {
                var intColumn = columnList.FirstOrDefault(i => i.TypeName.ToLower().Contains("int"));
                if (intColumn != null)
                {
                    primaryKeys.Add(new PrimaryKeyClass()
                    {
                        DataType = intColumn.TypeName,
                        FieldName = intColumn.Field
                    });
                }
                else
                {
                    primaryKeys.Add(new PrimaryKeyClass()
                    {
                        DataType = columnList[0].TypeName,
                        FieldName = columnList[0].Field
                    });
                }
            }

            return primaryKeys;
        }

        public SelectQueryData GetSelectQueryData(string tableName)
        {

            SelectQueryData data = new SelectQueryData();
            List<PrimaryKeyClass> primaryKeyList = null;

            if (dicPrimaryKey.ContainsKey(tableName))
            {
                primaryKeyList = dicPrimaryKey[tableName];
            }
            else
            {
                primaryKeyList = GetPrimaryKey(tableName);
                dicPrimaryKey.Add(tableName, primaryKeyList);
            }

            List<string> selectColumnList = new List<string>();
            List<JoinColumnClass> joinColumns = new List<JoinColumnClass>();
            List<FKColumnClass> fKColumns = new List<FKColumnClass>();
            var columnList = dicColumn[tableName];
            data.ColumnList = columnList;
            List<string> alphaList = new List<string>();
            var random = new Random();
            foreach (var c in columnList)
            {
                selectColumnList.Add(c.Field);
                if (c.FKDetails != null)
                {
                    var refColumns = GetTableColumns(c.FKDetails.REFERENCED_TABLE_NAME);
                    if (refColumns != null && refColumns.Count > 0)
                    {
                        var refColumnName = refColumns.Where(i => i.IsNull.ToUpper() == "NO" && i.Key != "PRI" && i.TypeName.Contains("char")).FirstOrDefault();
                        if (refColumnName == null)
                        {
                            refColumnName = refColumns.Where(i => i.Key != "PRI" && i.TypeName.Contains("char")).FirstOrDefault();
                        }
                        if (refColumnName == null)
                        {
                            refColumnName = refColumns.Where(i => i.Key != "PRI").FirstOrDefault();
                        }
                        if (refColumnName != null && fKColumns.Where(i => i.FieldName2 == refColumnName.Field).Count() == 0)
                        {
                            var asChar = Helper.GetTableCharacter(alphaList, c.FKDetails.REFERENCED_TABLE_NAME);
                            alphaList.Add(asChar);
                            fKColumns.Add(new FKColumnClass()
                            {
                                LocalField = c.Field,
                                DataTypeLocal = c.TypeName,
                                TableName2 = c.FKDetails.REFERENCED_TABLE_NAME,
                                FieldName2 = refColumnName.Field,
                                TableName1 = tableName,
                                TableChar2 = asChar,
                                FieldName1 = c.FKDetails.REFERENCED_COLUMN_NAME,
                                DataType2 = refColumnName.TypeName,
                                //FieldName = refColumnName.Field,
                                //TableChar= asChar
                            });
                            joinColumns.Add(new JoinColumnClass()
                            {
                                TableName2 = c.FKDetails.REFERENCED_TABLE_NAME,
                                FieldName2 = c.FKDetails.REFERENCED_COLUMN_NAME,
                                TableName1 = tableName,
                                TableChar2 = asChar,
                                FieldName1 = c.Field,
                                Column2Data = refColumnName,
                                Column1Data = c

                            });
                            selectColumnList.Add(refColumnName.Field);
                        }
                    }
                }
            }
            data.FKColumnData = fKColumns;
            data.JoinQueryData = joinColumns;
            data.SelectColumnList = selectColumnList;
            if (primaryKeyList.Count > 0)
                data.PrimaryKeys = primaryKeyList;
            else
            {

                var colIndex = columnList.First();
                if (colIndex != null)
                {
                    if (data.PrimaryKeys == null)
                    {
                        data.PrimaryKeys = new List<PrimaryKeyClass>();
                    }
                    data.PrimaryKeys.Add(new PrimaryKeyClass()
                    {
                        DataType = colIndex.TypeName,
                        FieldName = colIndex.Field
                    });
                }
            }
            return data;

        }
        public InsertUpdateQueryData GetInsertUpdateQueryData(string tableName)
        {
            InsertUpdateQueryData data = new InsertUpdateQueryData();
            List<PrimaryKeyClass> primaryKeyList = null;
            if (dicPrimaryKey.ContainsKey(tableName))
            {
                primaryKeyList = dicPrimaryKey[tableName];
            }
            else
            {
                primaryKeyList = GetPrimaryKey(tableName);
                dicPrimaryKey.Add(tableName, primaryKeyList);
            }
            data.PrimaryKeys = primaryKeyList;
            var columnList = dicColumn[tableName];
            data.ColumnList = columnList;
            var insertColumnList = new List<InsertUpdateClass>();
            var updateColumnList = new List<InsertUpdateClass>();
            foreach (var c in columnList)
            {
                if (c.Extra == "auto_increment")
                {

                }
                else if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    insertColumnList.Add(new InsertUpdateClass()
                    {
                        DataType = c.TypeName,
                        FieldName = c.Field,
                        isRequired = c.IsNull != null && c.IsNull.ToLower() == "no",
                        DefaultValue = c.DefaultValue,
                    });

                    updateColumnList.Add(new InsertUpdateClass()
                    {
                        DataType = c.TypeName,
                        FieldName = c.Field,
                        isRequired = c.IsNull != null && c.IsNull.ToLower() == "no",
                        DefaultValue = c.DefaultValue,
                    });
                }
            }
            data.InsertColumnList = insertColumnList;
            data.UpdateColumnList = updateColumnList;
            return data;
        }

        public FinalQueryData BuildLaravelQuery(string tableName)
        {
            FinalQueryData data = new FinalQueryData();
            var selectQueryData = GetSelectQueryData(tableName);
            var insertUpdateQueryData = GetInsertUpdateQueryData(tableName);

            var selectOneQuery = "SELECT {fkColumns} t.* FROM " + tableName + " t {joinQuery} WHERE {selectPrimaryKey} LIMIT 0,1";
            var selectAllQuery = "SELECT {fkColumns} t.* FROM " + tableName + " t {joinQuery} LIMIT ?, ?";
            var searchQuery = "SELECT {fkColumns} t.* FROM " + tableName + " t {joinQuery} WHERE {searchCondition} LIMIT ?,?";
            var searchRecordCountQuery = "SELECT count(*) TotalCount FROM " + tableName + " t {joinQuery} WHERE {searchCondition} ";
            var selectAllRecordCountQuery = "SELECT count(*) TotalCount FROM " + tableName + " t {joinQuery} ";
            var joinString = string.Empty;
            var fkColumnsString = string.Empty;
            var searchCondition = string.Empty;
            var deleteQuery = "DELETE FROM " + tableName + " Where {deletePrimaryKey}";
            string primaryKeyStringForController = "";
            string createPropertyList = "";
            string propertyList = "";
            string importantFieldForCreate = "";

            foreach (var c in selectQueryData.ColumnList)
            {
                if (c.DefaultValue == "CURRENT_TIMESTAMP")
                {
                    propertyList = propertyList + Environment.NewLine + "this." + c.Field + " = new Date();";
                    createPropertyList = createPropertyList + Environment.NewLine + c.Field + ": new Date(),";
                }
                else if (c.Extra == "auto_increment")
                {
                    propertyList = propertyList + Environment.NewLine + "this." + c.Field + " = 0;";
                    createPropertyList = createPropertyList + Environment.NewLine + "" + c.Field + ":0,";
                }
                else if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    propertyList = propertyList + Environment.NewLine + "this." + c.Field + " = " + tableName + "." + c.Field + ";";
                    createPropertyList = createPropertyList + Environment.NewLine + c.Field + ":req.body." + c.Field + ",";

                    if (c.IsNull == "NO")
                    {
                        if (string.IsNullOrEmpty(importantFieldForCreate))
                        {
                            importantFieldForCreate = importantFieldForCreate + "!createObj." + c.Field;
                        }
                        else
                        {
                            importantFieldForCreate = importantFieldForCreate + " || " + "!createObj." + c.Field;
                        }
                    }
                }

                if (c.Extra != "auto_increment")
                {
                    searchCondition = searchCondition + " OR LOWER(t." + c.Field + ") LIKE CONCAT('%','\"+searchKey+\"','%')";
                }

            }
            List<string> fkValidation = new List<string>();
            foreach (var f in selectQueryData.FKColumnData)
            {
                fkColumnsString = fkColumnsString + " " + f.TableChar2 + "." + f.FieldName2 + " as " + f.FieldName1 + "_Value,";
                propertyList = propertyList + Environment.NewLine + "this." + f.FieldName1 + "_Value = " + tableName + "." + f.FieldName1 + "_Value;";
            }

            foreach (var j in selectQueryData.JoinQueryData)
            {
                joinString = joinString + " join " + j.TableName2 + " " + j.TableChar2 + " on t." + j.FieldName1 + " = " + j.TableChar2 + "." + j.FieldName2 + " ";
            }
            var selectPrimaryKey = "";
            var deletePrimaryKey = "";
            var primaryKeyString = "";
            var primaryKeyCommaString = "";
            if (selectQueryData.PrimaryKeys != null && selectQueryData.PrimaryKeys.Count > 1)
            {
                foreach (var p in selectQueryData.PrimaryKeys)
                {
                    if (string.IsNullOrEmpty(primaryKeyString))
                    {
                        primaryKeyString = p.FieldName + "= ?";
                        primaryKeyCommaString = p.FieldName;
                    }
                    else
                    {
                        primaryKeyString = primaryKeyString + " AND " + p.FieldName + "= ?";
                        primaryKeyCommaString = primaryKeyCommaString + "," + p.FieldName;
                    }
                    if (!string.IsNullOrEmpty(selectPrimaryKey))
                    {
                        selectPrimaryKey = selectPrimaryKey + " AND ";
                    }
                    if (!string.IsNullOrEmpty(deletePrimaryKey))
                    {
                        deletePrimaryKey = deletePrimaryKey + " AND ";
                    }
                    if (string.IsNullOrEmpty(primaryKeyStringForController))
                    {
                        primaryKeyStringForController = "req.params." + p.FieldName;
                    }
                    else
                    {
                        primaryKeyStringForController = primaryKeyStringForController + ",req.params." + p.FieldName;
                    }
                    selectPrimaryKey = selectPrimaryKey + "t." + p.FieldName + "= ?";
                    deletePrimaryKey = deletePrimaryKey + p.FieldName + "=?";

                }
                selectPrimaryKey = selectPrimaryKey.Trim();
                deletePrimaryKey = deletePrimaryKey.Trim();
                primaryKeyString = primaryKeyString.Trim();
                primaryKeyStringForController = primaryKeyStringForController.Trim();
            }
            else if (selectQueryData.PrimaryKeys != null && selectQueryData.PrimaryKeys.Count == 1)
            {
                string pKeyOne = selectQueryData.PrimaryKeys[0].FieldName;
                selectPrimaryKey = "t." + pKeyOne + "= ?";
                deletePrimaryKey = pKeyOne + "=?";
                primaryKeyString = pKeyOne + "= ?";
                primaryKeyCommaString = pKeyOne;
                primaryKeyStringForController = "req.params." + pKeyOne;
            }
            else
            {
                string pKeyOne = selectQueryData.ColumnList[0].Field;
                selectPrimaryKey = "t." + pKeyOne + "= ?";
                deletePrimaryKey = pKeyOne + "=?";
                primaryKeyString = pKeyOne + "= ?";
                primaryKeyCommaString = pKeyOne;
                primaryKeyStringForController = "req.params." + pKeyOne;
            }

            selectAllQuery = selectAllQuery.Replace("{tableName}", tableName);
            selectAllQuery = selectAllQuery.Replace("{joinQuery}", joinString);
            selectAllQuery = selectAllQuery.Replace("{fkColumns}", fkColumnsString);

            selectOneQuery = selectOneQuery.Replace("{tableName}", tableName);
            selectOneQuery = selectOneQuery.Replace("{joinQuery}", joinString);
            selectOneQuery = selectOneQuery.Replace("{fkColumns}", fkColumnsString);
            selectOneQuery = selectOneQuery.Replace("{selectPrimaryKey}", selectPrimaryKey);


            searchQuery = searchQuery.Replace("{tableName}", tableName);
            searchQuery = searchQuery.Replace("{joinQuery}", joinString);
            searchQuery = searchQuery.Replace("{fkColumns}", fkColumnsString);
            searchQuery = searchQuery.Replace("{searchCondition}", searchCondition.Trim().TrimStart('O').TrimStart('R'));

            selectAllRecordCountQuery = selectAllRecordCountQuery.Replace("{tableName}", tableName);
            selectAllRecordCountQuery = selectAllRecordCountQuery.Replace("{joinQuery}", joinString);
            selectAllRecordCountQuery = selectAllRecordCountQuery.Replace("{fkColumns}", fkColumnsString);

            searchRecordCountQuery = searchRecordCountQuery.Replace("{tableName}", tableName);
            searchRecordCountQuery = searchRecordCountQuery.Replace("{joinQuery}", joinString);
            searchRecordCountQuery = searchRecordCountQuery.Replace("{fkColumns}", fkColumnsString);
            searchRecordCountQuery = searchRecordCountQuery.Replace("{searchCondition}", searchCondition.Trim().TrimStart('O').TrimStart('R'));

            deleteQuery = deleteQuery.Replace("{deletePrimaryKey}", deletePrimaryKey);



            string updateQuery = string.Empty;
            string updateQueryParam = "";
            string insertQuery = "INSERT INTO " + tableName + " set ?";
            string updateParameter = "[";
            updateQuery = "UPDATE " + tableName + " SET {updateQueryParam} WHERE {updateWereParam}";

            foreach (var c in insertUpdateQueryData.UpdateColumnList)
            {
                updateParameter = updateParameter + " " + tableName + "." + c.FieldName + ",";
                if (string.IsNullOrEmpty(updateQueryParam))
                {
                    updateQueryParam = updateQueryParam + c.FieldName + " = ?";
                }
                else
                {
                    updateQueryParam = updateQueryParam + "," + c.FieldName + " = ?";
                }

            }

            if (string.IsNullOrEmpty(importantFieldForCreate))
            {
                importantFieldForCreate = "true";
            }
            else
            {
                importantFieldForCreate = importantFieldForCreate.Trim().TrimStart('|').TrimStart('|').TrimEnd('|').TrimEnd('|');
            }
            data.PrimaryKeyControllerString = primaryKeyStringForController;
            data.PrimaryKeys = selectQueryData.PrimaryKeys;
            data.SearchQuery = searchQuery;
            data.SelectAllQuery = selectAllQuery;
            data.SelectOneQuery = selectOneQuery;
            data.SelectAllRecordCountQuery = selectAllRecordCountQuery;
            data.SearchRecordCountQuery = searchRecordCountQuery;
            data.SelectQueryData = selectQueryData;
            data.PropertyListString = propertyList;
            data.CreatePropertyListString = createPropertyList;
            data.PrimaryKeyString = primaryKeyString;
            data.PrimaryKeyCommaString = primaryKeyCommaString;
            updateQuery = updateQuery.Replace("{updateQueryParam}", updateQueryParam);
            updateQuery = updateQuery.Replace("{updateWereParam}", primaryKeyString);
            data.UpdateQuery = updateQuery;
            data.InsertQuery = insertQuery;
            updateParameter = updateParameter.TrimEnd(',') + "," + primaryKeyCommaString + "]";
            data.UpdateParam = updateParameter;
            data.UpdateParam = updateParameter;
            data.InsertParam = "";
            data.DeleteQuery = deleteQuery;
            data.RequiredFieldString_CreateUpdate = importantFieldForCreate;
            return data;
        }
        public FinalQueryData BuildQueryNodeJS(string tableName)
        {
            FinalQueryData data = new FinalQueryData();
            var selectQueryData = GetSelectQueryData(tableName);
            var insertUpdateQueryData = GetInsertUpdateQueryData(tableName);

            var selectOneQuery = "SELECT {fkColumns} t.* FROM " + tableName + " t {joinQuery} WHERE {selectPrimaryKey} LIMIT 0,1";
            var selectByFKQuery = "SELECT {fkColumns} t.* FROM " + tableName + " t {joinQuery} WHERE {selectFKKey} LIMIT ?, ?";
            var selectAllQuery = "SELECT {fkColumns} t.* FROM " + tableName + " t {joinQuery} LIMIT ?, ?";
            var searchQuery = "SELECT {fkColumns} t.* FROM " + tableName + " t {joinQuery} WHERE {searchCondition} LIMIT ?, ?";
            var searchRecordCountQuery = "SELECT count(*) TotalCount FROM " + tableName + " t {joinQuery} WHERE {searchCondition} ";
            var selectAllRecordCountQuery = "SELECT count(*) TotalCount FROM " + tableName + " t {joinQuery} ";
            var selectByFKCountQuery = "SELECT count(*) TotalCount FROM " + tableName + " t {joinQuery} WHERE {selectFKKey}";
            var joinString = string.Empty;
            var fkColumnsString = string.Empty;
            var searchCondition = string.Empty;
            var deleteQuery = "DELETE FROM " + tableName + " Where {deletePrimaryKey}";
            string primaryKeyStringForController = "";
            string createPropertyList = "";
            string propertyList = "";
            string importantFieldForCreate = "";

            foreach (var c in selectQueryData.ColumnList)
            {
                if (c.DefaultValue == "CURRENT_TIMESTAMP")
                {
                    propertyList = propertyList + Environment.NewLine + "this." + c.Field + " = new Date();";
                    createPropertyList = createPropertyList + Environment.NewLine + c.Field + ": new Date(),";
                }
                else if (c.Extra == "auto_increment")
                {
                    propertyList = propertyList + Environment.NewLine + "this." + c.Field + " = 0;";
                    createPropertyList = createPropertyList + Environment.NewLine + "" + c.Field + ":0,";
                }
                else if (c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP")
                {
                    propertyList = propertyList + Environment.NewLine + "this." + c.Field + " = " + tableName + "." + c.Field + ";";
                    createPropertyList = createPropertyList + Environment.NewLine + c.Field + ":req.body." + c.Field + ",";

                    if (c.IsNull == "NO")
                    {
                        if (string.IsNullOrEmpty(importantFieldForCreate))
                        {
                            importantFieldForCreate = importantFieldForCreate + "!createObj." + c.Field;
                        }
                        else
                        {
                            importantFieldForCreate = importantFieldForCreate + " || " + "!createObj." + c.Field;
                        }
                    }
                }

                if (c.Extra != "auto_increment")
                {
                    searchCondition = searchCondition + " OR LOWER(t." + c.Field + ") LIKE `+SqlString.escape('%'+key+'%')+`";
                }

            }
            List<string> fkValidation = new List<string>();
            foreach (var f in selectQueryData.FKColumnData)
            {
                fkColumnsString = fkColumnsString + " " + f.TableChar2 + "." + f.FieldName2 + " as " + f.FieldName1 + "_Value,";
                propertyList = propertyList + Environment.NewLine + "this." + f.FieldName1 + "_Value = " + tableName + "." + f.FieldName1 + "_Value;";
            }

            foreach (var j in selectQueryData.JoinQueryData)
            {
                if (j.Column1Data != null && j.Column1Data.IsNull.ToUpper() == "NO")
                {
                    joinString = joinString + " join " + j.TableName2 + " " + j.TableChar2 + " on t." + j.FieldName1 + " = " + j.TableChar2 + "." + j.FieldName2 + " ";
                }
                else
                {
                    joinString = joinString + " left join " + j.TableName2 + " " + j.TableChar2 + " on t." + j.FieldName1 + " = " + j.TableChar2 + "." + j.FieldName2 + " ";
                }
            }
            var selectPrimaryKey = "";
            var deletePrimaryKey = "";
            var primaryKeyString = "";
            var primaryKeyCommaString = "";
            if (selectQueryData.PrimaryKeys != null && selectQueryData.PrimaryKeys.Count > 1)
            {
                foreach (var p in selectQueryData.PrimaryKeys)
                {
                    if (string.IsNullOrEmpty(primaryKeyString))
                    {
                        primaryKeyString = p.FieldName + "= ${" + p.FieldName + "}";
                        primaryKeyCommaString = p.FieldName;
                    }
                    else
                    {
                        primaryKeyString = primaryKeyString + " AND " + p.FieldName + "= ${" + p.FieldName + "}";
                        primaryKeyCommaString = primaryKeyCommaString + "," + p.FieldName;
                    }
                    if (!string.IsNullOrEmpty(selectPrimaryKey))
                    {
                        selectPrimaryKey = selectPrimaryKey + " AND ";
                    }
                    if (!string.IsNullOrEmpty(deletePrimaryKey))
                    {
                        deletePrimaryKey = deletePrimaryKey + " AND ";
                    }
                    if (string.IsNullOrEmpty(primaryKeyStringForController))
                    {
                        primaryKeyStringForController = "req.params." + p.FieldName;
                    }
                    else
                    {
                        primaryKeyStringForController = primaryKeyStringForController + ",req.params." + p.FieldName;
                    }
                    selectPrimaryKey = selectPrimaryKey + "t." + p.FieldName + "=?";
                    deletePrimaryKey = deletePrimaryKey + p.FieldName + "=?";

                }
                selectPrimaryKey = selectPrimaryKey.Trim();
                deletePrimaryKey = deletePrimaryKey.Trim();
                primaryKeyString = primaryKeyString.Trim();
                primaryKeyStringForController = primaryKeyStringForController.Trim();
            }
            else if (selectQueryData.PrimaryKeys != null && selectQueryData.PrimaryKeys.Count == 1)
            {
                string pKeyOne = selectQueryData.PrimaryKeys[0].FieldName;
                selectPrimaryKey = "t." + pKeyOne + "=?";
                deletePrimaryKey = pKeyOne + "= ? ";
                primaryKeyString = pKeyOne + "= ?";
                primaryKeyCommaString = pKeyOne;
                primaryKeyStringForController = "req.params." + pKeyOne;
            }
            else
            {
                string pKeyOne = selectQueryData.ColumnList[0].Field;
                selectPrimaryKey = "t." + pKeyOne + "= ?";
                deletePrimaryKey = pKeyOne + "=?";
                primaryKeyString = pKeyOne + "=?";
                primaryKeyCommaString = pKeyOne;
                primaryKeyStringForController = "req.params." + pKeyOne;
            }
            data.SelectByFKQuery = new List<ExtraQuery>();

            foreach (var f in selectQueryData.FKColumnData)
            {
                var eQuery = new ExtraQuery();

                var fkQuery = selectByFKQuery;
                fkQuery = fkQuery.Replace("{selectFKKey}", "t." + f.LocalField + "= ?");
                fkQuery = fkQuery.Replace("{tableName}", tableName);
                fkQuery = fkQuery.Replace("{joinQuery}", joinString);
                fkQuery = fkQuery.Replace("{fkColumns}", fkColumnsString);

                var fkCountQuery = selectByFKCountQuery;
                fkCountQuery = fkCountQuery.Replace("{selectFKKey}", "t." + f.LocalField + "= ?");
                fkCountQuery = fkCountQuery.Replace("{tableName}", tableName);
                fkCountQuery = fkCountQuery.Replace("{joinQuery}", joinString);
                fkCountQuery = fkCountQuery.Replace("{fkColumns}", fkColumnsString);

                eQuery.ColumnName = f.LocalField;
                eQuery.DataType = f.DataTypeLocal;
                eQuery.SelectQuery = fkQuery;
                eQuery.SelectCountQuery = fkCountQuery;
                data.SelectByFKQuery.Add(eQuery);
            }
            selectAllQuery = selectAllQuery.Replace("{tableName}", tableName);
            selectAllQuery = selectAllQuery.Replace("{joinQuery}", joinString);
            selectAllQuery = selectAllQuery.Replace("{fkColumns}", fkColumnsString);

            selectOneQuery = selectOneQuery.Replace("{tableName}", tableName);
            selectOneQuery = selectOneQuery.Replace("{joinQuery}", joinString);
            selectOneQuery = selectOneQuery.Replace("{fkColumns}", fkColumnsString);
            selectOneQuery = selectOneQuery.Replace("{selectPrimaryKey}", selectPrimaryKey);


            searchQuery = searchQuery.Replace("{tableName}", tableName);
            searchQuery = searchQuery.Replace("{joinQuery}", joinString);
            searchQuery = searchQuery.Replace("{fkColumns}", fkColumnsString);
            searchQuery = searchQuery.Replace("{searchCondition}", searchCondition.Trim().TrimStart('O').TrimStart('R'));

            selectAllRecordCountQuery = selectAllRecordCountQuery.Replace("{tableName}", tableName);
            selectAllRecordCountQuery = selectAllRecordCountQuery.Replace("{joinQuery}", joinString);
            selectAllRecordCountQuery = selectAllRecordCountQuery.Replace("{fkColumns}", fkColumnsString);

            searchRecordCountQuery = searchRecordCountQuery.Replace("{tableName}", tableName);
            searchRecordCountQuery = searchRecordCountQuery.Replace("{joinQuery}", joinString);
            searchRecordCountQuery = searchRecordCountQuery.Replace("{fkColumns}", fkColumnsString);
            searchRecordCountQuery = searchRecordCountQuery.Replace("{searchCondition}", searchCondition.Trim().TrimStart('O').TrimStart('R'));

            deleteQuery = deleteQuery.Replace("{deletePrimaryKey}", deletePrimaryKey);



            string updateQueryParam = "";
            string insertQuery = "INSERT INTO " + tableName + " set ?";
            string updateParameter = "[";
            string updateQuery = "UPDATE " + tableName + " SET ? WHERE {updateWereParam}";
            string updatePatchQuery = "UPDATE " + tableName + " SET ? WHERE {updateWereParam}";
            string updatePatchParameter = "[" + tableName;
            foreach (var c in insertUpdateQueryData.UpdateColumnList)
            {
                updateParameter = updateParameter + " " + tableName + "." + c.FieldName + ",";
                if (string.IsNullOrEmpty(updateQueryParam))
                {
                    updateQueryParam = updateQueryParam + c.FieldName + " = ?";
                }
                else
                {
                    updateQueryParam = updateQueryParam + "," + c.FieldName + " = ?";
                }

            }

            if (string.IsNullOrEmpty(importantFieldForCreate))
            {
                importantFieldForCreate = "true";
            }
            else
            {
                importantFieldForCreate = importantFieldForCreate.Trim().TrimStart('|').TrimStart('|').TrimEnd('|').TrimEnd('|');
            }
            data.PrimaryKeyControllerString = primaryKeyStringForController;
            data.PrimaryKeys = selectQueryData.PrimaryKeys;
            data.SearchQuery = searchQuery;
            data.SelectAllQuery = selectAllQuery;
            data.SelectOneQuery = selectOneQuery;
            data.SelectAllRecordCountQuery = selectAllRecordCountQuery;
            data.SearchRecordCountQuery = searchRecordCountQuery;
            data.SelectQueryData = selectQueryData;
            data.PropertyListString = propertyList;
            data.CreatePropertyListString = createPropertyList;
            data.PrimaryKeyString = primaryKeyString;
            data.PrimaryKeyCommaString = primaryKeyCommaString;
            updateQuery = updateQuery.Replace("{updateQueryParam}", updateQueryParam);
            updateQuery = updateQuery.Replace("{updateWereParam}", primaryKeyString);
            updatePatchQuery = updatePatchQuery.Replace("{updateWereParam}", primaryKeyString);
            data.UpdateQuery = updateQuery;
            data.UpdatePatchQuery = updatePatchQuery;
            updateParameter = updateParameter.TrimEnd(',') + "," + primaryKeyCommaString + "]";
            updatePatchParameter = updatePatchParameter.TrimEnd(',') + "," + primaryKeyCommaString + "]";
            data.UpdatePatchParam = updatePatchParameter;
            data.UpdateParam = updateParameter;
            data.InsertQuery = insertQuery;
            data.InsertParam = "";
            data.DeleteQuery = deleteQuery;
            data.RequiredFieldString_CreateUpdate = importantFieldForCreate;
            return data;
        }
        public FinalDataPHP BuildQueryPHP(string tableName)
        {
            FinalDataPHP data = new FinalDataPHP();
            data.TableName = tableName;
            data.TableModuleName = ti.ToTitleCase(tableName);
            var selectQueryData = GetSelectQueryData(tableName);
            data.SelectQueryData = selectQueryData;
            data.PrimaryKeys = selectQueryData.PrimaryKeys;
            string tableVariable = "\". $this->table_name .\"";

            #region SelectAll
            string selectAllQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} LIMIT \".$offset.\" , \". $this->no_of_records_per_page.\""; ;
            var joinQuery = string.Empty;
            var fkColumns = string.Empty;
            foreach (var c in selectQueryData.JoinQueryData)
            {
                if (c.Column1Data != null && c.Column1Data.IsNull.ToUpper() == "NO")
                {
                    joinQuery = joinQuery + " join " + c.TableName2 + " " + c.TableChar2 + " on t." + c.FieldName1 + " = " + c.TableChar2 + "." + c.FieldName2 + " ";
                }
                else
                {
                    joinQuery = joinQuery + " left join " + c.TableName2 + " " + c.TableChar2 + " on t." + c.FieldName1 + " = " + c.TableChar2 + "." + c.FieldName2 + " ";

                }

                fkColumns = fkColumns + " " + c.TableChar2 + "." + c.Column2Data.Field + ",";
            }
            selectAllQuery = selectAllQuery.Replace("{tableName}", tableName);
            selectAllQuery = selectAllQuery.Replace("{joinQuery}", joinQuery);
            selectAllQuery = selectAllQuery.Replace("{fkColumns}", fkColumns);
            data.SelectAllQuery = "$query = \"" + selectAllQuery + "\";";
            #endregion

            #region SelectOne
            string selectOneSetValues = "";
            string selectOneBindValue = "$stmt->bindParam(1, $this->{primaryKey});";
            string selectOneQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE t.{primaryKey} = ? LIMIT 0,1";
            string pKey = "ERROR_NOPRIMARYKEYFOUND";
            if (selectQueryData.PrimaryKeys != null && selectQueryData.PrimaryKeys.Count > 0)
            {
                pKey = selectQueryData.PrimaryKeys.First().FieldName;
                data.PrimaryKeyString = pKey;
            }
            selectOneQuery = selectOneQuery.Replace("{tableName}", tableName);
            selectOneQuery = selectOneQuery.Replace("{joinQuery}", joinQuery);
            selectOneQuery = selectOneQuery.Replace("{fkColumns}", fkColumns);
            selectOneQuery = selectOneQuery.Replace("{primaryKey}", pKey);
            selectOneQuery = "$query = \"" + selectOneQuery + "\";";
            foreach (var sField in selectQueryData.SelectColumnList)
            {
                selectOneSetValues = selectOneSetValues + Environment.NewLine + "$this->" + sField + " = $row['" + sField + "'];";
            }
            data.SelectOneSetValues = selectOneSetValues;
            selectOneBindValue = selectOneBindValue.Replace("{primaryKey}", pKey);
            data.SelectOneBindValue = selectOneBindValue;
            data.SelectOneQuery = selectOneQuery;
            #endregion

            #region Delete
            string deleteQuery = "";
            string whereDelete = "";
            string deleteSanitize = "";
            string deleteBind = "";
            int deleteBindCount = 1;
            foreach (var p in data.PrimaryKeys)
            {
                if (!string.IsNullOrEmpty(whereDelete))
                {
                    whereDelete = whereDelete + " AND " + p.FieldName + " = ?";
                    deleteSanitize = deleteSanitize + "$this->" + p.FieldName + "=htmlspecialchars(strip_tags($this->" + p.FieldName + "));" + Environment.NewLine;
                    deleteBind = deleteBind + "$stmt->bindParam(" + deleteBindCount + ", $this->" + p.FieldName + ");" + Environment.NewLine;
                }
                else
                {
                    whereDelete = p.FieldName + " = ?";
                    deleteSanitize = "$this->" + p.FieldName + "=htmlspecialchars(strip_tags($this->" + p.FieldName + "));" + Environment.NewLine;
                    deleteBind = "$stmt->bindParam(" + deleteBindCount + ", $this->" + p.FieldName + ");" + Environment.NewLine;
                }
                deleteBindCount++;
            }
            deleteQuery = "$query = \"DELETE FROM \" . $this->table_name . \" WHERE {whereDelete} \";";
            deleteQuery = deleteQuery.Replace("{whereDelete}", whereDelete);
            deleteBind = deleteBind.Replace("{primaryKey}", pKey);
            data.DeleteQuery = deleteQuery;
            data.DeleteSanitize = deleteSanitize;
            data.DeleteBind = deleteBind;
            #endregion

            #region SearchByColumn
            string searchQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE {searchCondition} LIMIT \".$offset.\" , \". $this->no_of_records_per_page.\"";
            string searchQueryByColumn = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE \".$where.\" LIMIT \".$offset.\" , \". $this->no_of_records_per_page.\"";
            string searchCountQueryByColumn = "SELECT count(1) as total FROM " + tableVariable + " t {joinQuery} WHERE \".$where.\"";
            string searchCountQuery = "SELECT count(1) as total FROM " + tableVariable + " t {joinQuery} WHERE {searchCondition}";
            string searchCondition = "";
            string searchBindValue = "";
            int sBindCount = 1;
            foreach (var s in selectQueryData.ColumnList)
            {
                string sField = s.Field;
                if (sField != pKey)
                {
                    if (string.IsNullOrEmpty(searchCondition))
                    {
                        searchCondition = "LOWER(t." + sField + ")" + " LIKE ?";
                    }
                    else
                    {
                        searchCondition = searchCondition + " OR " + "LOWER(t." + sField + ")" + " LIKE ? ";
                    }
                    searchBindValue = searchBindValue + Environment.NewLine + "$stmt->bindParam(" + sBindCount + ", $searchKey);";
                    sBindCount++;
                }
            }
            searchQuery = searchQuery.Replace("{tableName}", tableName);
            searchQuery = searchQuery.Replace("{joinQuery}", joinQuery);
            searchQuery = searchQuery.Replace("{fkColumns}", fkColumns);
            searchQuery = searchQuery.Replace("{searchCondition}", searchCondition);
            searchQuery = "$query = \"" + searchQuery + "\";";

            searchQueryByColumn = searchQueryByColumn.Replace("{tableName}", tableName);
            searchQueryByColumn = searchQueryByColumn.Replace("{joinQuery}", joinQuery);
            searchQueryByColumn = searchQueryByColumn.Replace("{fkColumns}", fkColumns);
            searchQueryByColumn = searchQueryByColumn.Replace("{searchCondition}", searchCondition);
            searchQueryByColumn = "$query = \"" + searchQueryByColumn + "\";";

            searchCountQuery = searchCountQuery.Replace("{tableName}", tableName);
            searchCountQuery = searchCountQuery.Replace("{joinQuery}", joinQuery);
            searchCountQuery = searchCountQuery.Replace("{fkColumns}", fkColumns);
            searchCountQuery = searchCountQuery.Replace("{searchCondition}", searchCondition);
            searchCountQuery = "$query = \"" + searchCountQuery + "\";";

            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{tableName}", tableName);
            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{joinQuery}", joinQuery);
            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{fkColumns}", fkColumns);
            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{searchCondition}", searchCondition);
            searchCountQueryByColumn = "$query = \"" + searchCountQueryByColumn + "\";";

            data.SearchQueryByColumn = searchQueryByColumn;
            data.SearchQuery = searchQuery;
            data.SearchCountQueryByColumn = searchCountQueryByColumn;
            data.SearchBindValue = searchBindValue;
            data.SearchCountQuery = searchCountQuery;
            #endregion

            #region SelectFK
            var fkQueryDic = new Dictionary<string, string>();
            var selectFKQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE {fkKeyWhere} LIMIT \".$offset.\" , \". $this->no_of_records_per_page.\"";
            foreach (var fk in selectQueryData.FKColumnData)
            {
                try
                {
                    string selectFKFinalQuery = selectFKQuery;
                    selectFKFinalQuery = selectFKFinalQuery.Replace("{tableName}", tableName);
                    selectFKFinalQuery = selectFKFinalQuery.Replace("{joinQuery}", joinQuery);
                    selectFKFinalQuery = selectFKFinalQuery.Replace("{fkColumns}", fkColumns);
                    string fkKeyWhere = "t." + fk.LocalField + " = ?";
                    string fkBindValue = "$stmt->bindParam(1, $this->" + fk.LocalField + ");" + Environment.NewLine;
                    selectFKFinalQuery = selectFKFinalQuery.Replace("{fkKeyWhere}", fkKeyWhere);
                    selectFKFinalQuery = "$query = \"" + selectFKFinalQuery + "\";" + Environment.NewLine;
                    selectFKFinalQuery = selectFKFinalQuery + Environment.NewLine + "$stmt = $this->conn->prepare( $query );" + Environment.NewLine;
                    selectFKFinalQuery = selectFKFinalQuery + fkBindValue;

                    string functionStr = "function readBy" + Helper.RemoveSpecialCharacters(fk.LocalField) + "(){" + Environment.NewLine;
                    functionStr = functionStr + Environment.NewLine + "if (isset($_GET[\"pageNo\"]))";
                    functionStr = functionStr + Environment.NewLine + "{";
                    functionStr = functionStr + Environment.NewLine + "$this->pageNo =$_GET[\"pageNo\"]; } ";
                    functionStr = functionStr + Environment.NewLine + "$offset = ($this->pageNo - 1) * $this->no_of_records_per_page;";
                    functionStr = functionStr + Environment.NewLine + selectFKFinalQuery;
                    functionStr = functionStr + Environment.NewLine + "$stmt->execute();";
                    functionStr = functionStr + Environment.NewLine + "return $stmt;";
                    functionStr = functionStr + Environment.NewLine + "}";
                    if (fkQueryDic.ContainsKey(fk.FieldName1))
                        fkQueryDic[fk.LocalField] = functionStr;
                    else
                        fkQueryDic.Add(fk.LocalField, functionStr);
                }
                catch (Exception ex)
                {

                }
                data.FKQueryDic = fkQueryDic;
            }
            #endregion

            #region InsertUpdate
            var insertUpdateData = GetInsertUpdateQueryData(tableName);
            string insertQuery = "INSERT INTO \".$this->table_name.\" SET ";
            string updateQuery = "UPDATE \".$this->table_name.\" SET {fieldName} WHERE {primaryKey} = :{primaryKey}";
            string sanitize = string.Empty;
            string bindvalues = string.Empty;
            string fieldName = string.Empty;
            foreach (var c in insertUpdateData.InsertColumnList)
            {
                sanitize = sanitize + Environment.NewLine + "$this->" + c.FieldName + "=htmlspecialchars(strip_tags($this->" + c.FieldName + "));";
                bindvalues = bindvalues + Environment.NewLine + "$stmt->bindParam(\":" + c.FieldName + "\", $this->" + c.FieldName + ");";
                fieldName = fieldName + "," + c.FieldName + "=:" + c.FieldName;
            }
            fieldName = fieldName.Trim(',');
            insertQuery = "$query =\"" + insertQuery + fieldName.Trim(',') + "\";";
            updateQuery = updateQuery.Replace("{fieldName}", fieldName);
            updateQuery = updateQuery.Replace("{primaryKey}", pKey);
            updateQuery = "$query =\"" + updateQuery + "\";";
            string updateSanitize = sanitize;
            updateSanitize = updateSanitize + Environment.NewLine + "$this->" + pKey + "=htmlspecialchars(strip_tags($this->" + pKey + "));";
            string updateBindValue = bindvalues;
            updateBindValue = updateBindValue + Environment.NewLine + "$stmt->bindParam(\":" + pKey + "\", $this->" + pKey + ");";
            data.InsertQuery = insertQuery;
            data.UpdateQuery = updateQuery;
            data.InsertSanitize = sanitize;
            data.InsertBindValues = bindvalues;
            data.UpdateSanitize = updateSanitize;
            data.UpdateBindValues = updateBindValue;
            #endregion

            #region LoginQuery
            string selectLoginBindValue = "$stmt->bindParam(1, $this->{userColumn});" + Environment.NewLine + "$stmt->bindParam(2, $this->{passwordColumn});";
            var selectLoginQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE t.{userColumn} = ? AND t.{passwordColumn}=? LIMIT 0,1";
            selectLoginQuery = selectLoginQuery.Replace("{tableName}", tableName);
            selectLoginQuery = selectLoginQuery.Replace("{joinQuery}", joinQuery);
            selectLoginQuery = selectLoginQuery.Replace("{fkColumns}", fkColumns);
            selectLoginQuery = selectLoginQuery.Replace("{primaryKey}", pKey);
            selectLoginQuery = "$query = \"" + selectLoginQuery + "\";";
            data.SelectLoginQuery = selectLoginQuery;
            #endregion

            #region ObjectProperty
            string objectProperties = string.Empty;
            foreach (var c in selectQueryData.ColumnList)
            {
                if (!objectProperties.Contains("public $" + c.Field + ";"))
                    objectProperties = objectProperties + Environment.NewLine + "public $" + c.Field + ";";
            }
            foreach (var c in selectQueryData.JoinQueryData)
            {
                if (!objectProperties.Contains("public $" + c.Column2Data.Field + ";"))
                    objectProperties = objectProperties + Environment.NewLine + "public $" + c.Column2Data.Field + ";";
            }
            data.ObjectProperties = objectProperties;
            data.SelectLoginBindValue = selectLoginBindValue;
            #endregion     

            return data;
        }
    }
}
