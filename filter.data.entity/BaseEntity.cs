using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.entity
{
    public class BaseEntity
    {
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public int? UpdatedBy { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 删除日期
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// 删除人
        /// </summary>
        public int? DeletedBy { get; set; }
    }
}
