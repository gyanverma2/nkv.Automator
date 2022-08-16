using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Models
{
    public class LoginModel
    {
        public string MacID { get; set; }
        public string UserEmail { get; set; }
        public string APIKEY { get; set; }
    }
    public class RegisterModel
    {
        public string MacID { get; set; }
        public string LicenceNumber { get; set; }
        public string UserEmail { get; set; }
        public string Source { get; set; }
        public string SoftwareVersion { get; set; }
        public string ToolName { get; set; }
    }
    public class LicenceProductModel
    {
        public string ProductName { get; set; }
        public int NumberOfDays { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string licenceID { get; set; }
        public string MacID { get; set; }
        public string ProductNumber { get; set; }
        public string ValidTill { get; set; }
        public string ProductID { get; set; }
        public string ProductTitle { get; set; }
        public int DatabaseTypeId { get; set; }
        public string DatabaseTypeName { get; set; }
    }
    public class APIResponseRecord<T>
    {
        public T records { get; set; }
    }
    public class APIResponse<T>
    {
        public string status { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public T document { get; set; }
        public string licence { get; set; }
    }
}
