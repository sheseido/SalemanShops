using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.entity
{
    [Table("salemaninfo")]
    public class SalemanEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }
    }
}
