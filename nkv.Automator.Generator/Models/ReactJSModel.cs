using nkv.Automator.Postman;

namespace nkv.Automator.Generator.Models
{
    public class ReactJSInput<T>
    {
        public Dictionary<string, T> FinalDataDic { get; set; } = null!;
        public string DestinationFolder { get; set; } = null!;
        public List<PostmanModel> PostmanJson { get; set; } = null!;
    }
    public class ReactJSRepo
    {
        public ReactJSRepo()
        {
            getList = new List<string>();
            postParamList = new List<PRequestBody>();
        }
        public string functionType { get; set; } = null!;
        public string functionName { get; set; } = null!;
        public List<PRequestBody> postParamList { get; set; } = null!;
        public string finalURL { get; set; } = null!;
        public string methodName { get; set; } = null!;
        public List<string> getList { get; set; } = null!;

    }
    public class ReactJSTableData
    {
        public ReactJSTableData()
        {
            TableColumn = new List<PRequestRefBody>();
            PrimaryKey = new List<PRequestBody>();
        }
        public string TableName { get; set; } = null!;
        public List<PRequestRefBody> TableColumn { get; set; } = null!;
        public List<PRequestBody> PrimaryKey { get; set; } = null!;
    }
}
