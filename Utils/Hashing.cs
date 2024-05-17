using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeverseWPF.Utils
{
    class Hashing
    {
        public static string HashingPassword(string password)
        {
            MD5 md5 = MD5.Create();

            byte[] bytePassword = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(bytePassword);

            StringBuilder sb = new StringBuilder();
            foreach (var a in hash) sb.Append(a.ToString("X2"));

            return sb.ToString();
        }
    }
}
