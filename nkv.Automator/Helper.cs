using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
namespace nkv.Automator
{
    internal static class Helper
    {
        internal static string GetSystemName()
        {
            return Environment.MachineName;
        }
        internal static bool IsValidEmail(string email)
        {
            Regex mRegxExpression;
            if (email.Trim() != string.Empty)
            {
                mRegxExpression = new Regex(@"^([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$");

                if (mRegxExpression.IsMatch(email.Trim()))
                {
                    return true;
                }
            }
            return false;
        }
        internal static DateTime ParseUnixDateTime(double unixTimeStamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dt = dt.AddSeconds(unixTimeStamp).ToLocalTime();
            return dt;
        }
        internal static string Decrypt(string text, string key)
        {
            byte[] cipher = Convert.FromBase64String(text);
            byte[] btkey = Encoding.ASCII.GetBytes(key);
            RijndaelManaged aes128 = new RijndaelManaged();
            aes128.Mode = CipherMode.ECB;
            aes128.Padding = PaddingMode.PKCS7;
            ICryptoTransform decryptor = aes128.CreateDecryptor(btkey, null);
            MemoryStream ms = new MemoryStream(cipher);
            CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

            byte[] plain = new byte[cipher.Length];
            int decryptcount = cs.Read(plain, 0, plain.Length);

            ms.Close();
            cs.Close();
            return Encoding.UTF8.GetString(plain, 0, decryptcount);
        }
    }
   
}
