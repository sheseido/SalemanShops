using filter.framework.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace filter.framework.web
{
    /// <summary>
    /// MVC基础控制器
    /// </summary>
    public abstract class BaseController : Controller
    {
        private LogHelper mLogHelper;

        public LogHelper LogHelper
        {
            get
            {
                if (mLogHelper == null)
                {
                    mLogHelper = new LogHelper();
                }
                return mLogHelper;
            }
        }

        protected virtual string BuildAuth(string content)
        {
            return AuthHelper.Build(content);
        }

        protected Dictionary<string, string> RequestDictionary
        {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (string key in Request.QueryString.AllKeys)
                {
                    dic.Add(key, Request.QueryString[key]);
                }
                return dic;
            }
        }
    }
}
