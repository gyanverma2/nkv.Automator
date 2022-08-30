using MySql.Data.MySqlClient;
using nkv.Automator.Models;
using nkv.Automator.Utility;

namespace nkv.Automator.MySQL
{
    public class MySQLDBHelper
    {
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
    }
}
