using System.Text;
using System.Security.Cryptography;

namespace Eshop.Extension
{
    public static class HashMD5Pass
    {
        public static string PassToMD5(this string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] passHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder PassHash = new StringBuilder();
            foreach (byte b in passHash)
            {   
                PassHash.Append(String.Format("{0:x2}", b));
            }
            return PassHash.ToString();
        }
    }
}
