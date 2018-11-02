using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace Hasher
{
    public class HashString
    {
        public static string getSHA256Hash(string source)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);

            string hashStr = string.Empty;

            foreach (byte by in hash)
            {
                hashStr += string.Format("{0:x2}", by);
            }

            return hashStr;
        }

        
        public static string getMD5Hash(string source)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(source);
                byte[] hash = md5.ComputeHash(bytes);

                string hashStr = string.Empty;

                foreach(byte by in hash)
                {
                    hashStr += string.Format("{0:x2}", by);
                }

                return hashStr;
            }
        }

    }
}
