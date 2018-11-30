using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.model.Dto
{
    public class SaveSalemanModel
    {
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
