using System.Security.Cryptography;
using System.Text;

namespace CheckInProject.App.Utils
{
    public static class PasswordUtils
    {
        public static string CreatePasswordMD5(string password)
        {
            var passwordText = $"NobodyUseMD5ForEncryption{password}";
            var passwordBytes = Encoding.UTF8.GetBytes(passwordText);
            var resultBytes = MD5.HashData(passwordBytes);
            StringBuilder builder = new StringBuilder();
            foreach (var item in resultBytes)
            {
                builder.Append(item.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
