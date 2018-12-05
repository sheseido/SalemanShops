using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.entity
{
    [Table("waybillinfo")]
    public class WaybillEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 运单日期
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 店铺Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 业务员Id
        /// </summary>
        public int SalemanId { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 目的城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public double Weight { get; set; }
    }
}
