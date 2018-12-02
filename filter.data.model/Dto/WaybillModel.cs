using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.model.Dto
{
    public class WaybillModel
    {
        public int Id { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 运单日期
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string SalemanName { get; set; }

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
        public string Weight { get; set; }

        public string CreatedAt { get; set; }
    }
}
