using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.utility
{
    /// <summary>
    /// 哈希加密
    /// </summary>
    public sealed class HashHelper
    {
        private static MD5 md5MD5Algorithm;
        private static SHA1 sha1Algorithm;

        /// <summary>
        /// MD5算法
        /// </summary>
        private static MD5 MD5Algorithm
        {
            get
            {
                if (md5MD5Algorithm == null)
                {
                    md5MD5Algorithm = new MD5CryptoServiceProvider();
                }
                return md5MD5Algorithm;
            }
        }

        private static SHA1 SHA1Algorithm
        {
            get
            {
                if (sha1Algorithm == null)
                {
                    sha1Algorithm = new SHA1CryptoServiceProvider();
                }
                return sha1Algorithm;
            }
        }


        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns>加密后的Base64编码</returns>
        public static string MD5(string content)
        {
            var result = GetHash(content, MD5Algorithm);
            return result == null ? string.Empty : Convert.ToBase64String(result);
        }


        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns>加密后返回十六进制数据</returns>
        public static string MD5Hex(string content)
        {
            var result = GetHash(content, MD5Algorithm);
            return result == null ? string.Empty : ToHex(result);
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns>加密后的Base64编码</returns>
        public static string SHA1(string content)
        {
            var result = GetHash(content, SHA1Algorithm);
            return result == null ? string.Empty : Convert.ToBase64String(result);
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns>加密后的十六进制数据</returns>
        public static string SHA1Hex(string content)
        {
            var result = GetHash(content, MD5Algorithm);
            return result == null ? string.Empty : ToHex(result);
        }

        private static byte[] GetHash(string content, HashAlgorithm hashAlgorithm)
        {
            if (hashAlgorithm != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(content);
                return hashAlgorithm.ComputeHash(data);
            }
            return null;
        }

        private static string ToHex(byte[] bytes)
        {
            if (bytes != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
            return null;
        }
    }
}
