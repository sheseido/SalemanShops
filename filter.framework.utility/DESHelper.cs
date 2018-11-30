using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace filter.framework.utility
{
    public class DESHelper
    {
        private const string DEFAULT_KEY = "tiggerGd";

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="content">要加密的数据</param>
        /// <param name="key">密钥</param>
        /// <returns>返回Base64编码后的加密字符串</returns>
        public static string Encrypt(string content, string key = null)
        {
            if (key == null)
            {
                key = DEFAULT_KEY;
            }
            try
            {
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();//des进行加密
                desc.Mode = CipherMode.ECB;
                desc.Padding = PaddingMode.PKCS7;
                byte[] keyByte = Encoding.UTF8.GetBytes(key);
                byte[] data = Encoding.UTF8.GetBytes(content);
                using (MemoryStream ms = new MemoryStream())
                {
                    CryptoStream cs = new CryptoStream(ms, desc.CreateEncryptor(keyByte, keyByte), CryptoStreamMode.Write);
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="content">要解密的数据</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Decrypt(string content, string key = null)
        {
            if (key == null)
            {
                key = DEFAULT_KEY;
            }
            try
            {
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                desc.Mode = CipherMode.ECB;
                desc.Padding = PaddingMode.PKCS7;
                byte[] keyByte = Encoding.UTF8.GetBytes(key);
                byte[] data = Convert.FromBase64String(content);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, desc.CreateDecryptor(keyByte, keyByte), CryptoStreamMode.Write);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }
    }
}
