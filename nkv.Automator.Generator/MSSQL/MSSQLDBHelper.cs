using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using nkv.Automator.Models;
using nkv.Automator.Generator.Models;
using nkv.Automator.Utility;
using System.Data.Common;
using MySqlX.XDevAPI;

namespace nkv.Automator.MSSQL
{
    public class MSSQLDBHelper
    {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        public string ConnectionString { get; set; }
        public SqlConnection dbCon { get; set; } = null!;
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DBName { get; set; }
        Dictionary<string, List<ColumnModel>> dicColumn = new Dictionary<string, List<ColumnModel>>();
        Dictionary<string, List<PrimaryKeyClass>> dicPrimaryKey = new Dictionary<string, List<PrimaryKeyClass>>();
        public MSSQLDBHelper(string host, string port, string username, string password, string dbName, bool isWindowsAuth)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            DBName = dbName;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            if (!string.IsNullOrEmpty(port))
            {
                builder.DataSource = host + "," + port;
            }
            else
            {
                builder.DataSource = host;
            }
            builder.UserID = username;
            if (!isWindowsAuth)
                builder.Password = password;
            else
                builder.IntegratedSecurity = true;
            builder.InitialCatalog = dbName;
            ConnectionString = builder.ConnectionString;
        }
        public bool Connect()
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            dbCon = connection;
            return true;
        }
        private bool IsConnect()
        {
            if (dbCon == null) Connect();
            if (dbCon != null && dbCon.State == System.Data.ConnectionState.Closed)
            {
                dbCon.ConnectionString = ConnectionString;
                dbCon.Open();
            }
            return true;
        }
        public List<string> GetIdentityColumn(string tableName)
        {
            List<string> identityColumn = new List<string>();
            if (IsConnect())
            {
                using (SqlCommand command = new SqlCommand("select COLUMN_NAME, TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where COLUMNPROPERTY(object_id(TABLE_SCHEMA+'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 AND TABLE_NAME= '" + tableName + "' order by TABLE_NAME", dbCon))
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        identityColumn.Add(reader.GetString(0));
                    }
                }
            }
            return identityColumn;
        }
        public List<TableInformation> GetTableInformation(string tableName)
        {
            List<TableInformation> tableInformation = new List<TableInformation>();
            if (IsConnect())
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '" + tableName + "'", dbCon))
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        TableInformation tc = new TableInformation()
                        {
                            TABLE_CATALOG = reader.GetString(0),
                            TABLE_SCHEMA = reader.GetString(1),
                            TABLE_NAME = reader.GetString(2),
                            COLUMN_NAME = reader.GetString(3),
                            ORDINAL_POSITION = reader.GetInt32(4),
                            IS_NULLABLE = reader.GetString(6),
                            DATA_TYPE = reader.GetString(7),
                            COLUMN_DEFAULT = !reader.IsDBNull(5) ? reader.GetString(5) : null
                        };
                        tableInformation.Add(tc);
                    }
                }
            }
            return tableInformation;
        }
        private void SetANSIWarning()
        {
            if (IsConnect())
            {
                using (SqlCommand command = new SqlCommand("SET ANSI_WARNINGS OFF", dbCon))
                    command.ExecuteNonQuery();

            }

        }
        public List<SPParams> SP_GetParams(string procedureName)
        {
            List<SPParams> info = new List<SPParams>();
            if (IsConnect())
            {
                using (SqlCommand command = new SqlCommand("SELECT 'ParameterName' = name,'IsOutput' = is_output,'Type' = type_name(user_type_id),'Length' = max_length,'Precision' = case when type_name(system_type_id) = 'uniqueidentifier' then precision else OdbcPrec(system_type_id, max_length, precision) end,'Scale' = OdbcScale(system_type_id, scale),'ParamOrder'= parameter_id, 'DefaultValue' = default_value,'IsNullable' = is_nullable, 'Collation' = convert(sysname, case when system_type_id in (35, 99, 167, 175, 231, 239)then ServerProperty('collation') end)from sys.parameters where object_id = object_id('" + procedureName + "')", dbCon))
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        SPParams tc = new SPParams();
                        tc.ParameterName = !reader.IsDBNull(0) ? reader.GetString(0) : null;
                        tc.IsOutput = !reader.IsDBNull(1) ? reader.GetBoolean(1) : false;
                        tc.Type = !reader.IsDBNull(2) ? reader.GetString(2) : "varchar";
                        tc.Length = !reader.IsDBNull(3) ? reader.GetInt16(3) : 0;
                        tc.Precision = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0;
                        tc.Scale = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0;
                        tc.ParamOrder = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0;
                        tc.DefaultValue = !reader.IsDBNull(7) ? reader.GetString(7) : null;
                        tc.IsNullable = !reader.IsDBNull(8) ? reader.GetBoolean(8) : false;
                        tc.Collation = !reader.IsDBNull(9) ? reader.GetString(9) : null;
                        info.Add(tc);
                    }
                }

            }
            return info;

        }
        public List<TableConstraints> GetTableConstraints(string tableName)
        {
            List<TableConstraints> tableInformation = new List<TableConstraints>();
            if (IsConnect())
            {
                using (SqlCommand command = new SqlCommand("SELECT	ccu.COLUMN_NAME,tc.CONSTRAINT_TYPE,	tc.CONSTRAINT_NAME,ccu.TABLE_NAME,kcu.table_name as RefTableName,kcu.column_name AS TargetColumn FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS ccu ON ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME LEFT JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc ON ccu.CONSTRAINT_NAME = rc.CONSTRAINT_NAME LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu ON kcu.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME WHERE tc.TABLE_NAME ='" + tableName + "'", dbCon))
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        TableConstraints tc = new TableConstraints()
                        {
                            COLUMN_NAME = reader.GetString(0),
                            CONSTRAINT_TYPE = reader.GetString(1),
                            CONSTRAINT_NAME = reader.GetString(2),
                            CurrentTableName = reader.IsDBNull(3) ? null : reader.GetString(3),
                            REFTable = reader.IsDBNull(4) ? null : reader.GetString(4),
                            TargetColumn = reader.IsDBNull(5) ? null : reader.GetString(5),
                        };
                        tableInformation.Add(tc);
                    }
                }

            }
            return tableInformation;

        }
        public List<string> GetListOfTable()
        {
            List<string> tableList = new List<string>();
            if (IsConnect())
            {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM " + DBName + ".INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", dbCon))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableList.Add(reader.GetString(2));
                        }
                    }
                
            }
            return tableList;
        }
        public List<string> View_GetListOfTable()
        {
            List<string> tableList = new List<string>();
            if (IsConnect())
            {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM " + DBName + ".INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'VIEW'", dbCon))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableList.Add(reader.GetString(2));
                        }
                    }
                
            }
            return tableList;
        }
        public List<ColumnModel> GetTableColumns(string tableName)
        {
            List<ColumnModel> columnList = new List<ColumnModel>();
            var tableColumnDetails = GetTableInformation(tableName);
            var identityColumn = GetIdentityColumn(tableName);
            if (tableColumnDetails != null)
            {
                foreach (var col in tableColumnDetails)
                {
                    var defaultVal = "";
                    if (col.COLUMN_DEFAULT != null)
                    {
                        if (col.COLUMN_DEFAULT.ToLower().Contains("date"))
                        {
                            defaultVal = "CURRENT_TIMESTAMP";
                        }
                        else
                        {
                            defaultVal = col.COLUMN_DEFAULT;
                        }
                    }
                    var x = new ColumnModel()
                    {
                        Field = col.COLUMN_NAME,
                        TypeName = col.DATA_TYPE,
                        IsNull = col.IS_NULLABLE,
                        Key = "",
                        DefaultValue = defaultVal,
                        Extra = "",
                        FKDetails = null
                    };
                    if (identityColumn.Any(i => i == x.Field))
                    {
                        x.Extra = "auto_increment";
                    }
                    columnList.Add(x);
                }
            }
            var tableConstraints = GetTableConstraints(tableName);
            var constraints = tableConstraints.Where(i => i.CONSTRAINT_TYPE == "PRIMARY KEY").ToList();
            if (constraints != null && constraints.Count > 0)
            {
                foreach (var c in constraints)
                {
                    var columnDetail = columnList.Where(i => i.Field == c.COLUMN_NAME).FirstOrDefault();
                    if (columnDetail != null)
                    {
                        columnDetail.Key = "PRI";
                        columnDetail.Extra = "auto_increment";
                    }
                }
            }
            var fkConstraints = tableConstraints.Where(i => i.CONSTRAINT_TYPE == "FOREIGN KEY").ToList();

            if (fkConstraints != null && fkConstraints.Count > 0)
            {
                foreach (var c in fkConstraints)
                {
                    var columnDetail = columnList.Where(i => i.Field == c.COLUMN_NAME).FirstOrDefault();
                    if (columnDetail != null)
                    {
                        columnDetail.FKDetails = new FKDetails()
                        {
                            COLUMN_NAME = c.COLUMN_NAME,
                            CONSTRAINT_NAME = c.CONSTRAINT_NAME,
                            REFERENCED_COLUMN_NAME = c.TargetColumn,
                            REFERENCED_TABLE_NAME = c.REFTable
                        };
                    }
                }
            }
            return columnList;
        }
        public List<ProcedureDetals> GetAllProcedure(string databaseName)

        {
            List<ProcedureDetals> pInformation = new List<ProcedureDetals>();
            if (IsConnect())
            {
                    using (SqlCommand command = new SqlCommand("SELECT SPECIFIC_NAME,ROUTINE_NAME,ROUTINE_TYPE,ROUTINE_DEFINITION  FROM " + databaseName + ".INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' AND SPECIFIC_CATALOG='" + databaseName + "'", dbCon))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            ProcedureDetals tc = new ProcedureDetals()
                            {
                                SPECIFIC_NAME = reader.GetString(0),
                                ROUTINE_NAME = reader.GetString(1),
                                ROUTINE_TYPE = reader.GetString(2),
                                ROUTINE_DEFINITION = reader.GetString(3)
                            };
                            pInformation.Add(tc);
                        }
                    }
                
            }
            return pInformation;
        }
        public DataSet ExecuteProcedure(string procedureName, List<ParamDetails> paramList, out SPError exception)
        {
            SetANSIWarning();
            exception = null;
            if (IsConnect())
            {
                try
                {
                    SqlCommand command = new SqlCommand(procedureName, dbCon);
                    command.CommandType = CommandType.StoredProcedure;
                    List<ParamDetails> outParam = new List<ParamDetails>();
                    foreach (var p in paramList)
                    {
                        if (p.IsOutputParam)
                        {
                            command.Parameters.AddWithValue(p.ParamName, p.GeneratedDefaultValue).Direction = ParameterDirection.Output;
                            outParam.Add(p);
                        }
                        else
                        {
                            command.Parameters.AddWithValue(p.ParamName, p.GeneratedDefaultValue);
                        }
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (outParam.Count > 0)
                        {
                            DataTable dt = new DataTable();
                            dt.Clear();
                            foreach (var k in outParam)
                            {
                                dt.Columns.Add(k.ParamName);
                                DataRow dRow = dt.NewRow();
                                dRow[k.ParamName] = command.Parameters[k.ParamName].Value != null ? command.Parameters[k.ParamName].Value : null;
                                dt.Rows.Add(dRow);
                            }
                            dt.TableName = "OutParam_Table";
                            ds.Tables.Add(dt);
                        }
                        return ds;
                    }
                }
                catch (Exception ex)
                {
                    exception = new SPError()
                    {
                        SNo = 0,
                        SPName = procedureName,
                        ErrorMsg = ex.Message,
                        Remark = "Error executing the procedure, Please check your procedure parameters if the data type is defied properly. You can also write the code manually or send us the procedure to debug and fix."
                    };

                }

            }
            return null;
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
            string selectAllQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} order by t.{primaryKey} OFFSET \". $offset.\" ROWS FETCH NEXT \".$this->no_of_records_per_page.\" ROWS ONLY";
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
           
            #endregion

            #region SelectOne
            string selectOneSetValues = "";
            string selectOneBindValue = "$stmt->bindParam(1, $this->{primaryKey});";
            string selectOneQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE t.{primaryKey} = ? order by t.{primaryKey} OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";
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

            selectAllQuery = selectAllQuery.Replace("{tableName}", tableName);
            selectAllQuery = selectAllQuery.Replace("{joinQuery}", joinQuery);
            selectAllQuery = selectAllQuery.Replace("{fkColumns}", fkColumns);
            selectAllQuery = selectAllQuery.Replace("{primaryKey}", pKey);
            data.SelectAllQuery = "$query = \"" + selectAllQuery + "\";";
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
            string searchQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE {searchCondition} order by t.{primaryKey} OFFSET \". $offset.\" ROWS FETCH NEXT \".$this->no_of_records_per_page.\" ROWS ONLY";
            string searchQueryByColumn = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE \".$where.\" order by t.{primaryKey} OFFSET \". $offset.\" ROWS FETCH NEXT \".$this->no_of_records_per_page.\" ROWS ONLY";
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
            searchQuery = searchQuery.Replace("{primaryKey}", pKey);
            searchQuery = searchQuery.Replace("{searchCondition}", searchCondition);
            searchQuery = "$query = \"" + searchQuery + "\";";

            searchQueryByColumn = searchQueryByColumn.Replace("{tableName}", tableName);
            searchQueryByColumn = searchQueryByColumn.Replace("{joinQuery}", joinQuery);
            searchQueryByColumn = searchQueryByColumn.Replace("{fkColumns}", fkColumns);
            searchQueryByColumn = searchQueryByColumn.Replace("{primaryKey}", pKey);
            searchQueryByColumn = searchQueryByColumn.Replace("{searchCondition}", searchCondition);
            searchQueryByColumn = "$query = \"" + searchQueryByColumn + "\";";

            searchCountQuery = searchCountQuery.Replace("{tableName}", tableName);
            searchCountQuery = searchCountQuery.Replace("{joinQuery}", joinQuery);
            searchCountQuery = searchCountQuery.Replace("{fkColumns}", fkColumns);
            searchCountQuery = searchCountQuery.Replace("{searchCondition}", searchCondition);
            searchCountQuery = searchCountQuery.Replace("{primaryKey}", pKey);
            searchCountQuery = "$query = \"" + searchCountQuery + "\";";

            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{tableName}", tableName);
            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{joinQuery}", joinQuery);
            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{fkColumns}", fkColumns);
            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{searchCondition}", searchCondition);
            searchCountQueryByColumn = searchCountQueryByColumn.Replace("{primaryKey}", pKey);
            searchCountQueryByColumn = "$query = \"" + searchCountQueryByColumn + "\";";
           

            data.SearchQueryByColumn = searchQueryByColumn;
            data.SearchQuery = searchQuery;
            data.SearchCountQueryByColumn = searchCountQueryByColumn;
            data.SearchBindValue = searchBindValue;
            data.SearchCountQuery = searchCountQuery;
            #endregion

            #region SelectFK
            var fkQueryDic = new Dictionary<string, string>();
            var selectFKQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE {fkKeyWhere} order by t.{primaryKey} OFFSET \". $offset.\" ROWS FETCH NEXT \".$this->no_of_records_per_page.\" ROWS ONLY";
            foreach (var fk in selectQueryData.FKColumnData)
            {
                try
                {
                    string selectFKFinalQuery = selectFKQuery;
                    selectFKFinalQuery = selectFKFinalQuery.Replace("{tableName}", tableName);
                    selectFKFinalQuery = selectFKFinalQuery.Replace("{joinQuery}", joinQuery);
                    selectFKFinalQuery = selectFKFinalQuery.Replace("{fkColumns}", fkColumns);
                    selectFKFinalQuery = selectFKFinalQuery.Replace("{primaryKey}", pKey);
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
            var insertUpdateQueryData = GetInsertUpdateQueryData(tableName);
            data.InsertUpdateQueryData = insertUpdateQueryData;
            string insertQuery = "INSERT INTO \".$this->table_name.\" ({columnList}) Values ({columnParamList})";
            string updateQuery = "UPDATE \".$this->table_name.\" SET {fieldName} WHERE {primaryKey} = :{primaryKey}";
            string sanitize = string.Empty;
            string bindvalues = string.Empty;
            string fieldName = string.Empty;
            string columnList = string.Empty;
            string columnParamList = string.Empty;
            foreach (var c in insertUpdateQueryData.InsertColumnList)
            {
                sanitize = sanitize + Environment.NewLine + "$this->" + c.FieldName + "=htmlspecialchars(strip_tags($this->" + c.FieldName + "));";
                bindvalues = bindvalues + Environment.NewLine + "$stmt->bindParam(\":" + c.FieldName + "\", $this->" + c.FieldName + ");";
                fieldName = fieldName + "," + c.FieldName + "=:" + c.FieldName;
                columnList = columnList + "," + c.FieldName;
                columnParamList = columnParamList + ",:" + c.FieldName;
            }
            fieldName = fieldName.Trim(',');
            columnList = columnList.Trim(',');
            columnParamList = columnParamList.Trim(',');
            insertQuery = insertQuery.Replace("{columnList}", columnList);
            insertQuery = insertQuery.Replace("{columnParamList}", columnParamList);
            insertQuery = "$query =\"" + insertQuery + "\";";
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
            var selectLoginQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE t.{userColumn} = ? AND t.{passwordColumn}=? order by t.{primaryKey} OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";
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
