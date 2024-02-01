using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Secuirty
{
    public class AES_Security
    {
        private const int KeySize = 128;
        private byte[] privateKey128 =  { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
        
        private const int KeySize16 = 16;
        private byte[] privateKey16 =  { 0x4F, 0x6B, 0x40, 0x31, 0x32, 0x33, 0x40, 0x21, 0x53, 0x6F, 0x6D, 0x65, 0x52, 0x61, 0x6E, 0x64 };

        private readonly string key;
        public AES_Security()
        {
            //this.key = privateKey;
        }
        public string Encrypt(string plaintext)
        {
            byte[] keyBytes = privateKey128;
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.Key = keyBytes;
                aes.GenerateIV();

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] encryptedBytes = memoryStream.ToArray();
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
        }

        public string Decrypt(string encryptedText)
        {
            byte[] keyBytes = privateKey128;
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.Key = keyBytes;

                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(encryptedBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] decryptedBytes = memoryStream.ToArray();
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
        }
    }
}
