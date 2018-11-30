using System.Reflection;

namespace filter.framework.utility.XPort
{
    internal class ExcelColumnMapping
    {
        internal PropertyInfo Property { get; set; }
        internal int ColumnIndex { get; set; }

        internal string ColumnName { get; set; }
    }
}
