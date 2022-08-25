using nkv.Automator.Models;

namespace nkv.Automator.Generator.Models
{
    public class FinalDataPHP : FinalQueryData
    {
        public string SelectOneSetValues { get; set; } = null!;
        public string SelectOneBindValue { get; set; } = null!;
        public string DeleteSanitize { get; set; } = null!;
        public string DeleteBind { get; set; } = null!;


        public string SearchBindValue { get; set; } = null!;
        public Dictionary<string, string> FKQueryDic { get; set; } = null!;
        public string InsertSanitize { get; set; } = null!;
        public string InsertBindValues { get; set; } = null!;
        public string UpdateSanitize { get; set; } = null!;
        public string UpdateBindValues { get; set; } = null!;

        public string SelectLoginQuery { get; set; } = null!;
        public string ObjectProperties { get; set; } = null!;
        public string SelectLoginBindValue { get; set; } = null!;
        public string SelectLoginSetValues { get; set; } = null!;
    }
}
