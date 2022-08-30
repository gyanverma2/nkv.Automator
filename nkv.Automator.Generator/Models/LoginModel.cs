using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Models
{
    public class LoginModel
    {
        public string MacID { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string APIKEY { get; set; } = null!;
    }
    public class RegisterModel
    {
        public string MacID { get; set; } = null!;
        public string LicenceNumber { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string Source { get; set; } = null!;
        public string SoftwareVersion { get; set; } = null!;
        public string ToolName { get; set; } = null!;
        public string ItemName { get; set; } = null!;
    }
    public class LicenceProductModel
    {

        public string ProductName { get; set; } = null!;
        public string PurchaseCode { get; set; } = null!;
        public string PublicID { get; set; } = null!;
        public int NumberOfDays { get; set; }
        public DateTime? CreatedOn { get; set; } = null!;
        public string licenceID { get; set; } = null!;
        public string MacID { get; set; } = null!;
        public string ProductNumber { get; set; } = null!;
        public string ValidTill { get; set; } = null!;
        public int ProductID { get; set; }
        public string ProductTitle { get; set; } = null!;
        public int DatabaseTypeId { get; set; }
        public string DatabaseTypeName { get; set; } = null!;
    }
    public class APIResponseRecord<T>
    {
        public T records { get; set; }
    }
    public class APIResponse<T>
    {
        public string status { get; set; } = null!;
        public int code { get; set; }
        public string message { get; set; } = null!;
        public T document { get; set; }
        public string licence { get; set; } = null!;
    }
}
