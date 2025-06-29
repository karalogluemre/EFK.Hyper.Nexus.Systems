using System.Security.Cryptography;
using System.Text;

namespace EFK.System.Persistence.Injection.Config.AppByteService
{
    public class AesService : IAesService
    {
        private static string Key = "TxTgiEKyChZo1TTT";
        private static byte[] IV =
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
        };
        public string Encrypt(string clearText)
        {
            string enc = "";
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = Encoding.UTF8.GetBytes(Key);
                encryptor.IV = IV;
                using MemoryStream ms = new MemoryStream();
                using CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(Encoding.UTF8.GetBytes(clearText));
                cs.FlushFinalBlock();
                enc = Convert.ToBase64String(ms.ToArray());
            }
            return enc;
        }
        public string Decrypt(string cipherText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = IV;
            using MemoryStream input = new(Convert.FromBase64String(cipherText));
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream output = new();
            cryptoStream.CopyTo(output);
            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}
