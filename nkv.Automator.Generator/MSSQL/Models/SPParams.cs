using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.MSSQL
{
    public class SPParams
    {
        public string ParameterName { get; set; }
        public string Type { get; set; }
        public int Length { get; set; }
        public int Scale { get; set; }
        public int ParamOrder { get; set; }
        public int Precision { get; set; }
        public string DefaultValue { get; set; }
        public string Collation { get; set; }
        public bool IsOutput { get; set; }
        public bool IsNullable { get; set; }
    }

    public class ProcedureDetals
    {
        public string SPECIFIC_NAME { get; set; }
        public string ROUTINE_NAME { get; set; }
        public string ROUTINE_TYPE { get; set; }
        public string ROUTINE_DEFINITION { get; set; }
    }

    public class ParamDetails
    {
        public string ParamName { get; set; }
        public string ParamDataType { get; set; }
        public int ParamSize { get; set; }
        public string ParamDefaultValue { get; set; }
        public object GeneratedDefaultValue { get; set; }
        public bool IsOutputParam { get; set; }
        public bool IsNullabel { get; set; }
    }

    public class SPError
    {
        public int SNo { get; set; }
        public string SPName { get; set; }
        public string ErrorMsg { get; set; }
        public string Remark { get; set; }
    }
}
