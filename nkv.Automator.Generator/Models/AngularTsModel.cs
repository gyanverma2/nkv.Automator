using nkv.Automator.Postman;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Generator.Models
{
    public class AngularTsInput<T>
    {
        static TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        public Dictionary<string, T> FinalDataDic { get; set; } = null!;
        public string DestinationFolder { get; set; } = null!;
        public List<PostmanModel> PostmanJson { get; set; } = null!;
    }
    public class AngularTSRepo
    {
        public AngularTSRepo()
        {
            getList = new List<string>();
            postParamList = new List<PRequestBody>();
        }
        public string functionType { get; set; }
        public string functionName { get; set; }
        public List<PRequestBody> postParamList { get; set; }
        public string finalURL { get; set; }
        public string methodName { get; set; }
        public List<string> getList { get; set; }

    }
    public class AngularTSTableData
    {
        public AngularTSTableData()
        {
            TableColumn = new List<PRequestRefBody>();
            PrimaryKey = new List<PRequestBody>();
        }
        public string TableName { get; set; }
        public List<PRequestRefBody> TableColumn { get; set; }
        public List<PRequestBody> PrimaryKey { get; set; }
    }
}
