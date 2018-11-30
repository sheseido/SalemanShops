using System;
using System.Collections.Generic;
using System.Linq;

namespace filter.framework.core.Db
{
    /// <summary>
    /// 逻辑删除标识
    /// </summary>
    public class LogicalDeleteAttribute : Attribute
    {
        private List<string> mOtherColumn;

        /// <summary>
        /// 删除时，附加更新的列
        /// </summary>
        public string OtherColumn { get; set; }

        /// <summary>
        /// 获取附加更新的列
        /// </summary>
        /// <returns></returns>
        public List<string> GetColumns()
        {
            if (OtherColumn != null && mOtherColumn?.Count == null)
            {
                mOtherColumn = OtherColumn.Split(',').Select(c => c).ToList();
            }
            return mOtherColumn;
        }
    }
}
