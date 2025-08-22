using System.Security.Cryptography;
using System.Text;

namespace CodeverseWPF.Utils
{
    class Hashing
    {
        private const int KeySize = 100;
        private const int Iterations = 1000;
        private const string SecurityKey = "ComplexKeyHere_12121";

        public static string HashingPassword(string password)
        {
            MD5 md5 = MD5.Create();

            byte[] bytePassword = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(bytePassword);

            StringBuilder sb = new StringBuilder();
            foreach (var a in hash) sb.Append(a.ToString("X2"));

            return sb.ToString();
        }

        public static string EncryptPassword(string password, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, Iterations))
            {
                byte[] keyBytes = rfc2898DeriveBytes.GetBytes(KeySize / 8);
                return Convert.ToBase64String(keyBytes);
            }
        }

        public static string DecryptPassword(string encryptedPassword, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] encryptedPasswordBytes = Convert.FromBase64String(encryptedPassword);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes("", saltBytes, Iterations))
            {
                byte[] keyBytes = rfc2898DeriveBytes.GetBytes(KeySize / 8);

                using (AesManaged aesManaged = new AesManaged())
                {
                    aesManaged.KeySize = KeySize;
                    aesManaged.Key = keyBytes;
                    aesManaged.Mode = CipherMode.ECB;
                    aesManaged.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aesManaged.CreateDecryptor())
                    {
                        byte[] decryptedPasswordBytes = decryptor.TransformFinalBlock(encryptedPasswordBytes, 0, encryptedPasswordBytes.Length);
                        return Encoding.UTF8.GetString(decryptedPasswordBytes);
                    }
                }
            }
        }

        public static string Encrypt(string PlainText)
        {
            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);

            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
            objMD5CryptoService.Clear();

            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            objTripleDESCryptoService.Key = securityKeyArray;
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;


            var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
            objTripleDESCryptoService.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string CipherText)
        {
            byte[] toEncryptArray = Convert.FromBase64String(CipherText);
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();

            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
            objMD5CryptoService.Clear();

            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            objTripleDESCryptoService.Key = securityKeyArray;
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            objTripleDESCryptoService.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
