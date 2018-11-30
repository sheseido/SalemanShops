using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.model.Dto
{
    public class ShopsModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 所属业务员Id
        /// </summary>
        public int SalemanId { get; set; }

        /// <summary>
        /// 所属业务员姓名
        /// </summary>
        public string SalemanName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string CreatedAt { get; set; }
    }
}
