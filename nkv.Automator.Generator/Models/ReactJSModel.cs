using nkv.Automator.Postman;

namespace nkv.Automator.Generator.Models
{
    public class ReactJSInput<T>
    {
        public Dictionary<string, T> FinalDataDic { get; set; }
        public string DestinationFolder { get; set; }
        public List<PostmanModel> PostmanJson { get; set; }
    }
    public class ReactJSRepo
    {
        public ReactJSRepo()
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
    public class ReactJSTableData
    {
        public ReactJSTableData()
        {
            TableColumn = new List<PRequestRefBody>();
            PrimaryKey = new List<PRequestBody>();
        }
        public string TableName { get; set; }
        public List<PRequestRefBody> TableColumn { get; set; }
        public List<PRequestBody> PrimaryKey { get; set; }
    }
}
