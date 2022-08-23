using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Postman
{
    public class PostmanModel
    {
        public List<PRequestBody> Body { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<string> Host { get; set; } = null!;
        public List<string> Path { get; set; } = null!;
        public string Method { get; set; } = null!;
        public string TableName { get; set; } = null!;
    }

    public class PRequestBody
    {
        public PRequestBody(string propName, string propDataType, bool isRequred, string defaultValue)
        {
            IsRequred = isRequred;
            PropName = propName;
            PropDataType = propDataType;
            DefaultValue = defaultValue;
        }
        public string PropName { get; set; }
        public string PropDataType { get; set; }
        public bool IsRequred { get; set; }
        public string DefaultValue { get; set; }
    }

    public class PRequestRefBody
    {
        public PRequestRefBody(string actualColName, string actualTableName, string propName, string propDataType, string refColName, string refTableName, bool isRequred, string defaultValue)
        {
            ActualColumnName = actualColName;
            ActualTableName = actualTableName;
            PropName = propName;
            PropDataType = propDataType;
            RefTableName = refTableName;
            RefColumnName = refColName;
            IsRequred = isRequred;
            DefaultValue = defaultValue;
        }
        public string ActualColumnName { get; set; }
        public string ActualTableName { get; set; }
        public string PropName { get; set; }
        public string PropDataType { get; set; }
        public string RefTableName { get; set; }
        public string RefColumnName { get; set; }
        public bool IsRequred { get; set; }
        public string DefaultValue { get; set; }
    }
}
