using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using nkv.Automator.Generator.Models;
using nkv.Automator.Models;
using nkv.Automator.Utility;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.PGSQL
{
    public class PGSQLDBHelper
    {
        public string ConnectionString { get; set; }
        public string Host { get; set; }
        public string SchemaName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DBName { get; set; }
        public PGSQLDBHelper(string host, string schema, string port, string username, string password, string dbName)
        {
            Host = host;
            SchemaName = schema;
            Port =int.Parse(port);
            Username = username;
            Password = password;
            DBName = dbName;
            ConnectionString = "Host=" + Host + ";Port=" + Port + ";Username=" + Username + ";Password=" + Password + ";Database=" + DBName + "";
        }
        public bool Connect()
        {
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            return true;
        }

        public List<TableConstraint> GetTableConstraints(string tableName)
        {
            List<TableConstraint> tableConstraints = new List<TableConstraint>();

            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            string sql = "SELECT tc.table_schema, tc.constraint_name, tc.table_name, kcu.column_name, ccu.table_schema AS foreign_table_schema, ccu.table_name AS foreign_table_name, ccu.column_name AS foreign_column_name, pgc.contype FROM information_schema.table_constraints AS tc JOIN information_schema.key_column_usage AS kcu ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema JOIN information_schema.constraint_column_usage AS ccu ON ccu.constraint_name = tc.constraint_name AND ccu.table_schema = tc.table_schema JOIN pg_constraint AS pgc ON pgc.conname=tc.constraint_name WHERE tc.table_name='" + tableName + "';";
            using var cmd = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                TableConstraint c = new TableConstraint()
                {
                    table_schema = rdr.GetString(0),
                    constraint_name = rdr.GetString(1),
                    table_name = rdr.GetString(2),
                    column_name = rdr.GetString(3),
                    foreign_table_schema = rdr.GetString(4),
                    foreign_table_name = rdr.GetString(5),
                    foreign_column_name = rdr.GetString(6),
                    contype = rdr.GetChar(7).ToString(),

                };
                tableConstraints.Add(c);
            }
            return tableConstraints;
        }
        public List<string> GetSchema()
        {
            List<string> schemaList = new List<string>();
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            string sql = "select schema_name from information_schema.schemata;";
            using var cmd = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                schemaList.Add(rdr.GetString(0));
            }
            return schemaList;
        }

        public List<string> GetTables()
        {
            List<string> tableList = new List<string>();
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            string sql = "SELECT table_name FROM information_schema.tables WHERE table_schema='" + SchemaName + "'";
            using var cmd = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                tableList.Add(rdr.GetString(0));
            }
            var listOfView = GetListOfView();
            foreach (var t in listOfView)
            {
                tableList.Add("View - " + t);
            }
            return tableList;
        }
        public List<string> GetListOfView()
        {
            List<string> tableList = new List<string>();
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            string sql = "SELECT table_name FROM information_schema.views WHERE table_schema='" + SchemaName + "'";
            using var cmd = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                tableList.Add(rdr.GetString(0));
            }
            return tableList;
        }
        public List<ColumnModel> GetColumns(string tableName)
        {
            List<TableConstraint> tableConstraints = GetTableConstraints(tableName);
            List<ColumnModel> columnModels = new List<ColumnModel>();

            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            string sql = "SELECT column_name,data_type,is_nullable,udt_name,column_default FROM information_schema.columns WHERE table_name = '" + tableName + "'  AND table_schema = '" + SchemaName + "' ";
            using var cmd = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            var primaryKey = tableConstraints.Where(i => i.contype.ToLower() == "p").ToList();
            while (rdr.Read())
            {
                ColumnModel c = new ColumnModel()
                {
                    DefaultValue = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                    FKDetails = null,
                    Field = rdr.GetString(0),
                    TypeName = rdr.GetString(3),
                    Key = "",
                    IsNull = rdr.IsDBNull(2) ? "NO" : rdr.GetString(2).ToUpper(),
                };
                if (c.DefaultValue != null && c.DefaultValue.Contains("now"))
                {
                    c.DefaultValue = "CURRENT_TIMESTAMP";
                }
                if (primaryKey != null && primaryKey.Count > 0)
                {
                    var pKey = primaryKey.Where(i => i.column_name == c.Field).FirstOrDefault();
                    if (pKey != null)
                    {
                        c.Key = "PRI";
                    }
                    else
                    {
                        c.Key = "";
                    }
                }
                if (c.DefaultValue != null && c.DefaultValue.Contains("_seq"))
                {
                    c.Extra = "auto_increment";
                }
                if (tableConstraints != null && tableConstraints.Count > 0)
                {
                    var foreignKey = tableConstraints.Where(i => i.contype.ToLower() != "p" && i.column_name == c.Field).FirstOrDefault();
                    if (foreignKey != null)
                    {
                        FKDetails fk = new FKDetails()
                        {
                            COLUMN_NAME = c.Field,
                            CONSTRAINT_NAME = foreignKey.contype,
                            REFERENCED_COLUMN_NAME = foreignKey.foreign_column_name,
                            REFERENCED_TABLE_NAME = foreignKey.foreign_table_name
                        };
                        c.FKDetails = fk;
                    }
                }
                columnModels.Add(c);
            }
            return columnModels;
        }
        public List<PrimaryKeyClass> GetPrimaryKey(List<ColumnModel> columns)
        {
            if(!columns.Any())
                return new List<PrimaryKeyClass>();
            List<PrimaryKeyClass> primaryKeys = new List<PrimaryKeyClass>();
            var primeColumns = columns.Where(i => i.Key == "PRI").ToList();
            if (primeColumns.Any())
            {
                foreach(var p in primeColumns)
                {
                    primaryKeys.Add(new PrimaryKeyClass() { DataType = p.TypeName, FieldName = p.Field });
                }
            }
            else
            {
                var autoIncrementColumns = columns.Where(i => i.Extra == "auto_increment").FirstOrDefault();
                if (autoIncrementColumns != null)
                {
                    primaryKeys.Add(new PrimaryKeyClass() { DataType = autoIncrementColumns.TypeName, FieldName = autoIncrementColumns.Field });
                }
            }
            if (!primaryKeys.Any())
            {
                var intColumn = columns.FirstOrDefault(i => i.TypeName.ToLower().Contains("int"));
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
                        DataType = columns[0].TypeName,
                        FieldName = columns[0].Field
                    });
                }
            }
            return primaryKeys;
        }
        
        public SelectQueryData GetSelectQueryData(string tableName, List<ColumnModel> columnList)
        {
            SelectQueryData data = new SelectQueryData();
            List<string> selectColumnList = new List<string>();
            List<JoinColumnClass> joinColumns = new List<JoinColumnClass>();
            List<FKColumnClass> fKColumns = new List<FKColumnClass>();
            data.ColumnList = columnList;
            List<string> alphaList = new List<string>();
            foreach (var c in columnList)
            {
                selectColumnList.Add(c.Field);
                if (c.FKDetails != null)
                {
                    var refColumns = GetColumns(c.FKDetails.REFERENCED_TABLE_NAME);
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
                            var asChar = Helper.GetTableCharacter(alphaList,c.FKDetails.REFERENCED_TABLE_NAME);
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
            data.PrimaryKeys = GetPrimaryKey(columnList);
            return data;
        }
        public InsertUpdateQueryData GetInsertUpdateQueryData(string tableName, List<ColumnModel> columnList)
        {
            InsertUpdateQueryData data = new InsertUpdateQueryData();

            data.PrimaryKeys = GetPrimaryKey(columnList);
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

        public FinalDataPHP BuildQuery(string tableName)
        {
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            FinalDataPHP data = new FinalDataPHP();
            data.TableName = tableName;
            data.TableModuleName = ti.ToTitleCase(tableName);
            var columnList = GetColumns(tableName);
            var selectQueryData = GetSelectQueryData(tableName, columnList);
            data.SelectQueryData = selectQueryData;
            data.PrimaryKeys = selectQueryData.PrimaryKeys;
            string tableVariable = "\". $this->table_name .\"";

            #region SelectAll
            string selectAllQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} offset \".$offset.\" limit \". $this->no_of_records_per_page.\""; ;
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
            string selectLoginSetValues = "";
            string selectOneBindValue = "$stmt->bindParam(1, $this->{primaryKey});";
            string selectOneQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE t.{primaryKey} = ? offset 0 limit 1";
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
            string searchQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE {searchCondition} offset \".$offset.\" limit \". $this->no_of_records_per_page.\"";
            string searchQueryByColumn = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE \".$where.\" offset \".$offset.\" limit \". $this->no_of_records_per_page.\"";
            string searchCountQueryByColumn = "SELECT count(1) as total FROM " + tableVariable + " t {joinQuery} WHERE \".$where.\"";
            string searchCountQuery = "SELECT count(1) as total FROM " + tableVariable + " t {joinQuery} WHERE {searchCondition}";
            string searchCondition = "";
            string searchBindValue = "";
            int sBindCount = 1;
            foreach (var s in selectQueryData.ColumnList)
            {
                string sField = s.Field;
                if (sField != pKey && s.TypeName.ToLower().Contains("char"))
                {
                    if (string.IsNullOrEmpty(searchCondition))
                    {
                        searchCondition = "lower(t." + sField + ") LIKE CONCAT('%', CAST(:search_term as VARCHAR), ' %')";
                    }
                    else
                    {
                        searchCondition = searchCondition + " OR " + "lower(t." + sField + ") LIKE CONCAT('%', CAST(:search_term as VARCHAR), ' %')";
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
            var selectFKQuery = "SELECT {fkColumns} t.* FROM " + tableVariable + " t {joinQuery} WHERE {fkKeyWhere} offset \".$offset.\" limit \". $this->no_of_records_per_page.\"";
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
            var insertUpdateData = GetInsertUpdateQueryData(tableName,columnList);
            data.InsertUpdateQueryData = insertUpdateData;
            string insertQuery = "INSERT INTO \".$this->table_name.\" ";
            string updateQuery = "UPDATE \".$this->table_name.\" SET {fieldName} WHERE {primaryKey} = :{primaryKey}";
            string sanitize = string.Empty;
            string bindvalues = string.Empty;
            string fieldName = string.Empty;
            string fieldNameUpdate = string.Empty;
            string fieldValue = string.Empty;
            foreach (var c in insertUpdateData.InsertColumnList)
            {
                sanitize = sanitize + Environment.NewLine + "$this->" + c.FieldName + "=htmlspecialchars(strip_tags($this->" + c.FieldName + "));";
                bindvalues = bindvalues + Environment.NewLine + "$stmt->bindParam(\":" + c.FieldName + "\", $this->" + c.FieldName + ");";
                fieldName = fieldName + "," + c.FieldName;
                fieldValue = fieldValue + "," +":"+ c.FieldName;
                fieldNameUpdate= fieldNameUpdate + "," + c.FieldName + "=:"+ c.FieldName;
            }
            fieldName = fieldName.Trim(',');
            fieldValue = fieldValue.Trim(',');
            insertQuery = "$query =\"" + insertQuery +"("+ fieldName + ") Values ("+fieldValue+") \";";
            updateQuery = updateQuery.Replace("{fieldName}", fieldNameUpdate.Trim(','));
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
            var selectLoginQuery = "SELECT {fkColumns} {tableColumn} FROM " + tableVariable + " t {joinQuery} WHERE t.{userColumn} = ? AND t.{passwordColumn}=? offset 0 limit 1";
            selectLoginQuery = selectLoginQuery.Replace("{tableName}", tableName);
            selectLoginQuery = selectLoginQuery.Replace("{joinQuery}", joinQuery);
            selectLoginQuery = selectLoginQuery.Replace("{fkColumns}", fkColumns);
            selectLoginQuery = selectLoginQuery.Replace("{primaryKey}", pKey);
            string tableColumn = string.Empty;
            foreach (var c in columnList)
            {
                if(c.TypeName!= "bytea")
                {
                    tableColumn = tableColumn + ",t." + c.Field;
                    selectLoginSetValues = selectLoginSetValues + Environment.NewLine + "$this->" + c.Field + " = $row['" + c.Field + "'];";
                }
            }
            tableColumn = tableColumn.Trim(',');
            data.SelectLoginSetValues = selectLoginSetValues;
            selectLoginQuery = selectLoginQuery.Replace("{tableColumn}", tableColumn);
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
