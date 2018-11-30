using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace filter.framework.utility
{
    public class Utils
    {
        #region 生成随机字母或数字
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns></returns>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random(DateTime.Now.Ticks.GetHashCode());
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        public static string GetCheckCode(int codeCount)
        {
            string str = string.Empty;
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        /// <summary>
        /// 根据日期和随机码生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumber()
        {
            string num = DateTime.Now.ToString("yyyyMMddHHmmss");//yyyyMMddHHmmssms
            return num + Number(4).ToString();
        }

        /// <summary>
        /// 根据日期和随机码生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumber(int num, bool sleep = false)
        {
            if (num == 0)
                num = 1;
            string strnum = DateTime.Now.ToString("yyyyMMddHHmmss");//yyyyMMddHHmmssms
            return strnum + Number(num, sleep).ToString();
        }
        private static int Next(int numSeeds, int length)
        {
            byte[] buffer = new byte[length];
            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            Gen.GetBytes(buffer);
            uint randomResult = 0x0;//这里用uint作为生成的随机数  
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)buffer[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds);
        }
        #endregion

        public static string GetSha1(String str)
        {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[] 
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(str);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");
            return hash;
        }

        /// <summary>
        /// 判断输入的字符串是否是一个合法的手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string input, string express)
        {
            //匹配手机号如果最后一位是换行符,就匹配不出来,这里用Length=11 先判断手机位数
            if (string.IsNullOrEmpty(input) || input.Length != 11)
                return false;
            Regex regex = new Regex("^1\\d{10}$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 隐藏手机号码中间4位
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static string HideMobileMiddleNumber(string mobile)
        {
            Regex re = new Regex("(\\d{3})(\\d{4})(\\d{4})", RegexOptions.None);
            mobile = re.Replace(mobile, "$1****$3");
            return mobile;
        }

        /// <summary>  
        /// 返回一个指定范围内的随机数。  
        /// </summary>  
        /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>  
        /// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。 maxValue 必须大于或等于 minValue。</param>  
        /// <returns>一个大于等于 minValue 且小于 maxValue 的 Decimal，即：返回的值范围包括 minValue 但不包括 maxValue。 如果 minValue 等于 maxValue，则返回 minValue。</returns>  
        public static decimal NextDecimal(decimal minValue, decimal maxValue)
        {
            if (minValue == maxValue)
            {
                return minValue;
            }
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("“minValue”不能大于 maxValue。", "minValue");
            }
            decimal d;
            byte[] buffer = new byte[4];
            var _rand = new Random();
            do
            {
                _rand.NextBytes(buffer);
                int lo = BitConverter.ToInt32(buffer, 0);// 96 位整数的低 32 位。  
                _rand.NextBytes(buffer);
                int mid = BitConverter.ToInt32(buffer, 0);// 96 位整数的中间 32 位。  
                _rand.NextBytes(buffer);
                int hi = BitConverter.ToInt32(buffer, 0);// 96 位整数的高 32 位。  
                bool isNegative = _rand.Next(2) == 0;// 正或负  
                byte scale = (byte)_rand.Next(29);// 10 的指数（0 到 28 之间）。  
                d = new decimal(lo, mid, hi, isNegative, scale);
            } while ((d >= minValue && d < maxValue) == false);
            return d;
        }
    }
}
