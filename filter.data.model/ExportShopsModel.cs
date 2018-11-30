using filter.framework.utility.XPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.model
{
    public class ExportShopsModel
    {
        [ExcelColumnName(ColumnName = "店铺名")]
        public string Name { get; set; }

        [ExcelColumnName(ColumnName = "联系人")]
        public string Contact { get; set; }

        [ExcelColumnName(ColumnName = "联系方式")]
        public string Mobile { get; set; }

        [ExcelColumnName(ColumnName = "业务员")]
        public string SalemanName { get; set; }
    }
}
