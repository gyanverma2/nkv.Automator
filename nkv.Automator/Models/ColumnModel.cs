using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Models
{
    public class ColumnModel
    {
        public string Field { get; set; }
        public string TypeName { get; set; }
        public string IsNull { get; set; }
        public string Key { get; set; }
        public string DefaultValue { get; set; }
        public string Extra { get; set; }
        public FKDetails FKDetails { get; set; }
        public ColumnModel()
        {
            FKDetails = null;
        }
        public ColumnModel(string field, string typename, string isnull, string key, string defaultValue, string extra)
        {
            Field = field;
            TypeName = typename;
            IsNull = isnull;
            Key = key;
            DefaultValue = defaultValue;
            Extra = extra;
        }
    }
    public class PrimaryKeyClass
    {
        public string FieldName { get; set; }
        public string DataType { get; set; }
    }

    public class InsertUpdateClass
    {
        public string FieldName { get; set; }
        public string DataType { get; set; }
        public bool isRequired { get; set; }
        public string DefaultValue { get; set; }
    }
    public class ExtraQuery
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string SelectQuery { get; set; }
        public string SelectCountQuery { get; set; }
    }

    public class FKColumnClass
    {
        public string LocalField { get; set; }
        public string DataTypeLocal { get; set; }
        public string TableName1 { get; set; }
        public string FieldName1 { get; set; }
        public string TableChar2 { get; set; }
        public string TableName2 { get; set; }
        public string FieldName2 { get; set; }
        //public string TableChar { get; set; }
        //public string FieldName { get; set; }
        public string DataType2 { get; set; }
    }

    public class JoinColumnClass
    {
        public string TableName1 { get; set; }
        public string FieldName1 { get; set; }
        public string TableChar2 { get; set; }
        public string TableName2 { get; set; }
        public string FieldName2 { get; set; }
        public ColumnModel Column2Data { get; set; }
        public ColumnModel Column1Data { get; set; }
    }

    public class SelectQueryData
    {
        public List<ColumnModel> ColumnList { get; set; }
        public List<PrimaryKeyClass> PrimaryKeys { get; set; }
        public List<JoinColumnClass> JoinQueryData { get; set; }
        public List<FKColumnClass> FKColumnData { get; set; }
        public List<string> SelectColumnList { get; set; }
    }

    public class InsertUpdateQueryData
    {
        public List<ColumnModel> ColumnList { get; set; }
        public List<PrimaryKeyClass> PrimaryKeys { get; set; }
        public List<InsertUpdateClass> InsertColumnList { get; set; }
        public List<InsertUpdateClass> UpdateColumnList { get; set; }
    }

    public class FinalQueryData
    {
        public string TableName { get; set; }
        public SelectQueryData SelectQueryData { get; set; }
        public InsertUpdateQueryData InsertUpdateQueryData { get; set; }
        public List<PrimaryKeyClass> PrimaryKeys { get; set; }
        public string PrimaryKeyString { get; set; }
        public string PrimaryKeyCommaString { get; set; }
        public string SelectAllQuery { get; set; }
        public List<ExtraQuery> SelectByFKQuery { get; set; }
        public string SelectOneQuery { get; set; }
        public string DeleteQuery { get; set; }
        public string SearchQuery { get; set; }
        public string SelectAllRecordCountQuery { get; set; }
        public string SearchRecordCountQuery { get; set; }
        public string PropertyListString { get; set; }
        public string CreatePropertyListString { get; set; }
        public string PrimaryKeyControllerString { get; set; }
        public string InsertQuery { get; set; }
        public string UpdateQuery { get; set; }
        public string UpdatePatchQuery { get; set; }
        public string UpdateParam { get; set; }
        public string UpdatePatchParam { get; set; }
        public string InsertParam { get; set; }
        public string RequiredFieldString_CreateUpdate { get; set; }
    }
    public class FKDetails
    {
        public string COLUMN_NAME { get; set; }
        public string CONSTRAINT_NAME { get; set; }
        public string REFERENCED_COLUMN_NAME { get; set; }
        public string REFERENCED_TABLE_NAME { get; set; }
    }
}
