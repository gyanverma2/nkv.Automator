using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Models
{
    public class ColumnModel
    {
        public string Field { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public string IsNull { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string DefaultValue { get; set; } = null!;
        public string Extra { get; set; } = null!;
        public FKDetails? FKDetails { get; set; }
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
        public string FieldName { get; set; } = null!;
        public string DataType { get; set; } = null!;
    }

    public class InsertUpdateClass
    {
        public string FieldName { get; set; } = null!;
        public string DataType { get; set; } = null!;
        public bool isRequired { get; set; } 
        public string DefaultValue { get; set; } = null!;
    }
    public class ExtraQuery
    {
        public string ColumnName { get; set; } = null!;
        public string DataType { get; set; } = null!;
        public string SelectQuery { get; set; } = null!;
        public string SelectCountQuery { get; set; } = null!;
    }

    public class FKColumnClass
    {
        public string LocalField { get; set; } = null!;
        public string DataTypeLocal { get; set; } = null!;
        public string TableName1 { get; set; } = null!;
        public string FieldName1 { get; set; } = null!;
        public string TableChar2 { get; set; } = null!;
        public string TableName2 { get; set; } = null!;
        public string FieldName2 { get; set; } = null!;
        public string DataType2 { get; set; } = null!;
    }

    public class JoinColumnClass
    {
        public string TableName1 { get; set; } = null!;
        public string FieldName1 { get; set; } = null!;
        public string TableChar2 { get; set; } = null!;
        public string TableName2 { get; set; } = null!;
        public string FieldName2 { get; set; } = null!;
        public ColumnModel Column2Data { get; set; } = null!;
        public ColumnModel Column1Data { get; set; } = null!;
    }

    public class SelectQueryData
    {
        public List<ColumnModel> ColumnList { get; set; } = null!;
        public List<PrimaryKeyClass> PrimaryKeys { get; set; } = null!;
        public List<JoinColumnClass> JoinQueryData { get; set; } = null!;
        public List<FKColumnClass> FKColumnData { get; set; } = null!;
        public List<string> SelectColumnList { get; set; } = null!;
    }

    public class InsertUpdateQueryData
    {
        public List<ColumnModel> ColumnList { get; set; } = null!;
        public List<PrimaryKeyClass> PrimaryKeys { get; set; } = null!;
        public List<InsertUpdateClass> InsertColumnList { get; set; } = null!;
        public List<InsertUpdateClass> UpdateColumnList { get; set; } = null!;
    }

    public class FinalQueryData
    {
        public string TableName { get; set; } = null!;
        public string TableModuleName { get; set; } = null!;
        public SelectQueryData SelectQueryData { get; set; } = null!;
        public InsertUpdateQueryData InsertUpdateQueryData { get; set; } = null!;
        public List<PrimaryKeyClass> PrimaryKeys { get; set; } = null!;
        public string PrimaryKeyString { get; set; } = null!;
        public string PrimaryKeyCommaString { get; set; } = null!;
        public string SelectAllQuery { get; set; } = null!;
        public List<ExtraQuery> SelectByFKQuery { get; set; } = null!;
        public string SelectOneQuery { get; set; } = null!;
        public string DeleteQuery { get; set; } = null!;
        public string SearchQuery { get; set; } = null!;
        public string SearchCountQuery { get; set; } = null!;
        public string SearchQueryByColumn { get; set; } = null!;
        public string SearchCountQueryByColumn { get; set; } = null!;
        public string SelectAllRecordCountQuery { get; set; } = null!;
        public string SearchRecordCountQuery { get; set; } = null!;
        public string PropertyListString { get; set; } = null!;
        public string CreatePropertyListString { get; set; } = null!;
        public string PrimaryKeyControllerString { get; set; } = null!;
        public string InsertQuery { get; set; } = null!;
        public string UpdateQuery { get; set; } = null!;
        public string UpdatePatchQuery { get; set; } = null!;
        public string UpdateParam { get; set; } = null!;
        public string UpdatePatchParam { get; set; } = null!;
        public string InsertParam { get; set; } = null!;
        public string RequiredFieldString_CreateUpdate { get; set; } = null!;

    }
   
    public class FKDetails
    {
        public string COLUMN_NAME { get; set; } = null!;
        public string CONSTRAINT_NAME { get; set; } = null!;
        public string REFERENCED_COLUMN_NAME { get; set; } = null!;
        public string REFERENCED_TABLE_NAME { get; set; } = null!;
    }
}
