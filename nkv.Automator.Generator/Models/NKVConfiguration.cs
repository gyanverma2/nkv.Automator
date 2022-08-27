namespace nkv.Automator.Models
{
    public class NKVConfiguration
    {
        public NKVAuthTableConfig AuthTableConfig { get; set; } = null!;
        public NKVAdminPanelPermissionConfig AdminPanelConfig { get; set; } = null!;
    }
    public class NKVAuthTableConfig
    {
        public bool IsEmail { get; set; }
        public bool IsSkipAuth { get; set; }
        public string AuthTableName { get; set; } = null!;
        public string UsernameColumnName { get; set; } = null!;
        public string PasswordColumnName { get; set; } = null!;
     
    }
    public class NKVAdminPanelPermissionConfig
    {
        public string SuperAdminUsername { get; set; } = null!;
        public string SuperAdminPassword { get; set; } = null!;
        public string AdminUsername { get; set; } = null!;
        public string AdminPassword { get; set; } = null!;
        public string GuestUsername { get; set; } = null!;
        public string GuestPassword { get; set; } = null!;
        public List<string> ImageColumns { get; set; } = null!;
        public List<string> FileColumns { get; set; } = null!;
    }
    public class NKVMessage
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; } 
        public NKVMessage(string message, bool isSuccess=true)
        {
            Message = message;
            IsSuccess = isSuccess;
        }
    }
}
