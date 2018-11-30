using System;

namespace filter.framework.core.Db
{
    /// <summary>
    /// 更新语句忽略标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UpdateIgnoreAttribute : Attribute
    {

    }
}
