using System;

namespace filter.framework.core.Db
{
    /// <summary>
    /// 数据库表属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
    }
}
