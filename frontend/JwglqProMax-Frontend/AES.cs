using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;


namespace JwglqProMax_Frontend
{
    public class AesEncryption
    {
        // 密钥长度必须是16、24或32字节
        private static readonly string Key = "ASKWPckepcSMEPCm";

        public static string Encrypt(string data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                byte[] inputBuffer = Encoding.UTF8.GetBytes(data);
                byte[] outputBuffer = new byte[inputBuffer.Length]; // 修改这里，将outputBuffer的长度设置为与输入数据相同
                aes.CreateEncryptor().TransformBlock(inputBuffer, 0, inputBuffer.Length, outputBuffer, 0);

                return Convert.ToBase64String(outputBuffer);
            }
        }

        public static string Decrypt(string encryptedData)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                byte[] inputBuffer = Convert.FromBase64String(encryptedData);
                byte[] outputBuffer = new byte[inputBuffer.Length]; // 修改这里，将outputBuffer的长度设置为与输入数据相同
                aes.CreateDecryptor().TransformBlock(inputBuffer, 0, inputBuffer.Length, outputBuffer, 0);

                return Encoding.UTF8.GetString(outputBuffer);
            }
        }
    }

    //public class Program
    //{
    //    public static void Main()
    //    {
    //        string data = "需要加密的数据";
    //        string encryptedData = AesEncryption.Encrypt(data);
    //        Console.WriteLine("加密后的数据： " + encryptedData);

    //        string decryptedData = AesEncryption.Decrypt(encryptedData);
    //        Console.WriteLine("解密后的数据： " + decryptedData);
    //    }
    //}
}
