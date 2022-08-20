using nkv.Automator.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.PGSQL
{
    internal class PGSQLDBHelper
    {
        string ConnectionString { get; set; }
        string Host { get; set; }
        string SchemaName { get; set; }
        int Port { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string DBName { get; set; }
        internal PGSQLDBHelper(string host, string schema, string port, string username, string password, string dbName)
        {
            Host = host;
            SchemaName = schema;
            Port =int.Parse(port);
            Username = username;
            Password = password;
            DBName = dbName;
            ConnectionString = "Host=" + Host + ";Port=" + Port + ";Username=" + Username + ";Password=" + Password + ";Database=" + DBName + "";
        }
        internal bool Connect()
        {
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            return true;
        }

        internal List<TableConstraint> GetTableConstraints(string tableName)
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
        internal List<string> GetSchema()
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

        internal List<string> GetTables(string schemaName)
        {
            List<string> schemaList = new List<string>();
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            string sql = "SELECT table_name FROM information_schema.tables WHERE table_schema='" + schemaName + "'";
            using var cmd = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                schemaList.Add(rdr.GetString(0));
            }
            return schemaList;
        }

        internal List<ColumnModel> GetColumns(string tableName, string schemaName)
        {
            List<TableConstraint> tableConstraints = GetTableConstraints(tableName);
            List<ColumnModel> columnModels = new List<ColumnModel>();

            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();
            string sql = "SELECT column_name,data_type,is_nullable,udt_name,column_default FROM information_schema.columns WHERE table_name = '" + tableName + "'  AND table_schema = '" + schemaName + "' ";
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
    }
}
