using filter.framework.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.web
{
    /// <summary>
    /// 授权生成器
    /// </summary>
    public class AuthHelper
    {
        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Build(string content)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(content))
            {
                result = string.Empty;
            }
            else
            {
                var date = DateTime.Now;
                result = DESHelper.Encrypt(string.Join("|", new object[] { content, date.ToString("yyyy-MM-dd HH:mm:ss") }));
            }
            return result;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="content"></param>
        /// <param name="checkDate">是否校验时间</param>
        /// <returns></returns>
        public static string Get(string content, bool checkDate = true)
        {
            string result = string.Empty;
            var auth = DESHelper.Decrypt(content);
            var token = auth.Split('|');
            if (token.Length >= 1)
            {
                if (checkDate)
                {
                    var date = DateTime.Parse(token[1]);
                    if ((DateTime.Now - date).Seconds < 12 * 60 * 60)
                    {
                        result = token[0];
                    }
                }
                else
                {
                    result = token[0];
                }
            }
            return result;
        }
    }
}
