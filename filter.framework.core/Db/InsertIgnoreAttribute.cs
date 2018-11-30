using System;

namespace filter.framework.core.Db
{
    /// <summary>
    /// 新增语句忽略标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InsertIgnoreAttribute : Attribute
    {
    }
}