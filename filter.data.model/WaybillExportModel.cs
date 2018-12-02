using filter.framework.utility.XPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.model
{
    public class WaybillExportModel
    {
        [ExcelColumnName(ColumnName = "日期")]
        public string Time { get; set; }

        [ExcelColumnName(ColumnName = "运单号")]
        public string Code { get; set; }

        [ExcelColumnName(ColumnName = "客户名称")]
        public string ShopName { get; set; }

        [ExcelColumnName(ColumnName = "结算对象")]
        public string SalemanName { get; set; }

        [ExcelColumnName(ColumnName = "目的地")]
        public string Province { get; set; }

        [ExcelColumnName(ColumnName = "城市")]
        public string City { get; set; }

        [ExcelColumnName(ColumnName = "结算重量")]
        public double Weight { get; set; }
    }

    public class WaybillExportWithPriceModel
    {
        [ExcelColumnName(ColumnName = "日期")]
        public string Time { get; set; }

        [ExcelColumnName(ColumnName = "运单号")]
        public string Code { get; set; }

        [ExcelColumnName(ColumnName = "客户名称")]
        public string ShopName { get; set; }

        [ExcelColumnName(ColumnName = "结算对象")]
        public string SalemanName { get; set; }

        [ExcelColumnName(ColumnName = "目的地")]
        public string Province { get; set; }

        [ExcelColumnName(ColumnName = "城市")]
        public string City { get; set; }

        [ExcelColumnName(ColumnName = "结算重量")]
        public double Weight { get; set; }

        [ExcelColumnName(ColumnName = "结算单价")]
        public decimal SettlementPrice { get; set; }

        [ExcelColumnName(ColumnName = "创建时间")]
        public string CreatedAt { get; set; }
    }
}
