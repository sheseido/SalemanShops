using filter.framework.utility;
using System.Web.Mvc;

namespace filter.framework.web
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public class ExceptionHandlerFilterAttribute : HandleErrorAttribute
    {
        private LogHelper mLogHelper;

        public ExceptionHandlerFilterAttribute()
        {
            mLogHelper = new LogHelper();
        }


        public override void OnException(ExceptionContext exceptionContext)
        {
            base.OnException(exceptionContext);
            //错误记录
            mLogHelper.Fatal(exceptionContext.Exception.Message, exceptionContext.Exception);
        }
    }
}
