using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.utility.XPort
{
    public class ExcelColumnIndexAttribute : Attribute
    {
        public int ColumnIndex { get; set; }
    }
}
