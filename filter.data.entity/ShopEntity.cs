using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.entity
{
    [Table("shopinfo")]
    public class ShopEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
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
    }
}
