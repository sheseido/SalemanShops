using System.ComponentModel;

namespace filter.framework.web
{
    /// <summary>
    /// WebApi返回结果基础类
    /// </summary>
    public class ResultBase
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 结果标识
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        public void SetResult(bool result, string message = null)
        {
            this.Result = result;
            this.Message = message;
        }

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultBase BuildResult(bool result, string message = null, int code = 0)
        {
            ResultBase resultBase = new ResultBase();
            resultBase.SetResult(result, message);
            resultBase.Code = code;
            return resultBase;
        }

        public static ResultBase Sucess(string message = null)
        {
            return BuildResult(true, message);
        }

        public static ResultBase Fail(string message = null, int code = 0)
        {
            return BuildResult(false, message, code);
        }

        public static ResultBase Fail(Enum_ResultBaseCode resultBaseCode)
        {
            var code = (int)resultBaseCode;
            return BuildResult(false, utility.EnumConverter.ToEnumDescription<Enum_ResultBaseCode>(code), code);
        }
    }

    /// <summary>
    /// WebApi返回结果基础类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ResultBase<T> : ResultBase
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        public static ResultBase<T> BuildResult(bool result, T data, string message = null, int code = 0)
        {
            ResultBase<T> resultBase = new ResultBase<T>();
            resultBase.SetResult(result, message);
            resultBase.Data = data;
            resultBase.Code = code;
            return resultBase;
        }

        public static ResultBase<T> Sucess(T data)
        {
            return BuildResult(true, data);
        }

        public new static ResultBase<T> Fail(string message = null, int code = 0)
        {
            return BuildResult(false, default(T), message, code);
        }

        public new static ResultBase<T> Fail(Enum_ResultBaseCode resultBaseCode)
        {
            var code = (int)resultBaseCode;
            return BuildResult(false, default(T), utility.EnumConverter.ToEnumDescription<Enum_ResultBaseCode>(code), code);
        }
    }

    public enum Enum_ResultBaseCode
    {
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Fail = 401,

        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        ParamError = 402,

        /// <summary>
        /// 参数缺失
        /// </summary>
        [Description("参数缺失")]
        ParamLackError = 403,

        /// <summary>
        /// 数据不存在
        /// </summary>
        [Description("数据不存在")]
        DataNotFoundError = 404,

        //Token业务段
        TokenCode_Success = 0,         //成功
        TokenCode_ParameterError = 100,
        TokenCode_RequestOvertime = 200,
        TokenCode_TokenOvertime = 201,
        TokenCode_DeviceError = 300,
        TokenCode_TokenError = 400,
        TokenCode_Error = 900,

        //用户业务段
        Customer_Exist = 10001, //会员已存在
    }
}
